using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SistemaProyectos.Data;
using SistemaProyectos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SistemaProyectos.Controllers
{
    [Authorize(Roles = "Administrador, Empleado")]
    public class TareasController : Controller
    {
        private readonly AppDbContext _context;

        private readonly AcumuladorTareas _acumulador;

        public TareasController(AppDbContext context, AcumuladorTareas acumulador)
        {
            _context = context;
            _acumulador = acumulador;
        }
        //GET: KanBan

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> KanBan(string proyecto = "")
        {
            var tareas = _context.Tarea
                .Include(t => t.Proyecto)
                .Include(t => t.Usuario)
                .AsQueryable();

            if (!string.IsNullOrEmpty(proyecto))
            {
                tareas = tareas.Where(t => t.Proyecto.NombreProyecto == proyecto);
            }

            ViewBag.ProyectoSeleccionado = proyecto;
            ViewBag.Proyectos = _context.Proyecto
                .Select(p => p.NombreProyecto)
                .Distinct()
                .ToList();

            return View(await tareas.ToListAsync());
        }

        // GET: Tareas
        [Authorize(Roles = "Administrador, Empleado")]
        public async Task<IActionResult> Index(int page = 1, string busqueda = "", string estado = "", string proyecto = "", bool mias = false)
        {
            int registrosPorPagina = 10;

            var tareasQuery = _context.Tarea
                .Include(t => t.Proyecto)
                .Include(t => t.Usuario)
                .AsQueryable();

            // 🔹 Filtrar por tareas del usuario actual si "mias" está activado
            if (mias)
            {
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                tareasQuery = tareasQuery.Where(t => t.UsuarioID == userId);
            }

            // 🔹 Filtrar por estado
            if (!string.IsNullOrEmpty(estado))
            {
                tareasQuery = tareasQuery.Where(t => t.Estado == estado);
            }

            // 🔹 Filtrar por búsqueda en título
            if (!string.IsNullOrEmpty(busqueda))
            {
                tareasQuery = tareasQuery.Where(t => t.Titulo.Contains(busqueda));
            }

            // 🔹 Filtrar por proyecto
            if (!string.IsNullOrEmpty(proyecto))
            {
                tareasQuery = tareasQuery.Where(t => t.Proyecto.NombreProyecto.Contains(proyecto));
            }

            int totalRegistros = await tareasQuery.CountAsync();

            // Lista de Proyectos
            var proyectos = await _context.Proyecto.ToListAsync();
            ViewData["ProyectosLista"] = proyectos;


            var tareas = await tareasQuery
                .OrderBy(t => t.TareaID)
                .Skip((page - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            // Datos de paginación
            ViewBag.PaginaActual = page;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            int maxPaginasMostradas = 5;
            int inicio = Math.Max(1, page - maxPaginasMostradas / 2);
            int fin = Math.Min(ViewBag.TotalPaginas, inicio + maxPaginasMostradas - 1);

            ViewBag.PaginaInicio = inicio;
            ViewBag.PaginaFin = fin;

            // Guardar valores seleccionados para mantener filtros en la vista
            ViewBag.EstadoSeleccionado = estado;
            ViewBag.Busqueda = busqueda;
            ViewBag.ProyectoSeleccionado = proyecto;
            ViewBag.Mias = mias; // 👈 Para que la vista sepa si está filtrando "mis tareas"

            ViewBag.Proyectos = await _context.Proyecto
                .Select(p => p.NombreProyecto)
                .Distinct()
                .ToListAsync();

            return View(tareas);
        }

        // GET: Tareas/Create
        public IActionResult Create()
        {
            ViewBag.TareasAcumuladas = _acumulador.ObtenerTareas();
            ViewBag.Proyectos = new SelectList(_context.Proyecto, "ProyectoID", "NombreProyecto");
            ViewBag.Usuarios = new SelectList(_context.Usuario, "UsuarioID", "NombreCompleto");
            return View(new Tarea());
        }

        // POST: Tareas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Acumular([Bind("TareaID,Titulo,Descripcion,Estado,FechaInicio,FechaFin,ProyectoID,UsuarioID,Prioridad,FechaRegistro")] Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                var hoy = DateTime.Now;
                tarea.FechaRegistro = DateTime.Now;

                // calcular estado
                if (hoy < tarea.FechaInicio)
                {
                    tarea.Estado = "Pendiente";
                }
                else if (hoy >= tarea.FechaInicio && hoy <= tarea.FechaFin)
                {
                    tarea.Estado = "En Progreso";
                }
                else if (hoy > tarea.FechaFin)
                {
                    tarea.Estado = "Finalizado";
                }

                if (tarea.TareaID == 0) // nueva
                {
                    _acumulador.AgregarTarea(tarea);
                }
                else // actualización
                {
                    _acumulador.ActualizarTarea(tarea);
                }

                return RedirectToAction(nameof(Create));
            }

            // recargamos combos y la tabla
            ViewBag.Proyectos = new SelectList(_context.Proyecto, "ProyectoID", "NombreProyecto", tarea.ProyectoID);
            ViewBag.Usuarios = new SelectList(_context.Usuario, "UsuarioID", "NombreCompleto", tarea.UsuarioID);
            ViewBag.TareasAcumuladas = _acumulador.ObtenerTareas();

            // volvemos a la vista Create
            return View("Create", tarea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAll()
        {
            var tareas = _acumulador.ObtenerTareasParaBD(); // Prepara las tareas sin IDs temporales

            if (tareas.Any())
            {
                // Guardamos en la BD, EF generará los IDs reales
                _context.Tarea.AddRange(tareas);
                await _context.SaveChangesAsync();

                // Limpiamos el acumulador en memoria
                _acumulador.Limpiar();

                // Redirigimos al listado
                return RedirectToAction(nameof(Index));
            }

            // Si no hay tareas acumuladas, volvemos a la vista Create con mensaje
            ViewBag.Proyectos = new SelectList(_context.Proyecto, "ProyectoID", "NombreProyecto");
            ViewBag.Usuarios = new SelectList(_context.Usuario, "UsuarioID", "NombreCompleto");
            ViewBag.TareasAcumuladas = _acumulador.ObtenerTareas();
            ViewBag.MensajeError = "No hay tareas acumuladas para guardar.";

            // Devolvemos un objeto Tarea vacío para que el formulario funcione
            var tareaVacia = new Tarea();
            return View("Create", tareaVacia);
        }



        /// Edita tarea acumulada: la carga en el formulario
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var tarea = _acumulador.ObtenerTareas().FirstOrDefault(t => t.TareaID == id);
            if (tarea == null)
            {
                return RedirectToAction(nameof(Create));
            }

            // combos
            ViewBag.Proyectos = new SelectList(_context.Proyecto, "ProyectoID", "NombreProyecto", tarea.ProyectoID);
            ViewBag.Usuarios = new SelectList(_context.Usuario, "UsuarioID", "NombreCompleto", tarea.UsuarioID);
            ViewBag.TareasAcumuladas = _acumulador.ObtenerTareas();

            ViewBag.ModoEdicion = true; // aquí marcamos edición

            return View("Create", tarea);
        }


        // Elimina una tarea acumulada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            _acumulador.EliminarTarea(id);
            return RedirectToAction(nameof(Create));
        }



        // GET: Tareas/Details/5
        [Authorize(Roles = "Administrador, Empleado")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tarea
                .Include(t => t.Proyecto)
                .Include(t => t.Comentarios)
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(m => m.TareaID == id);

            var Comentarios = await _context.Comentario
                .Where(c => c.TareaID == id)
                .Include(c => c.Usuario)
                .ToListAsync();

            if (tarea == null)
            {
                return NotFound();
            }

            return View(tarea);
        }



        // GET: Tareas/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tarea.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }

            ViewBag.Proyectos = new SelectList(_context.Proyecto, "ProyectoID", "NombreProyecto", tarea.ProyectoID);
            ViewBag.Usuarios = new SelectList(_context.Usuario, "UsuarioID", "NombreCompleto", tarea.UsuarioID);

            return View(tarea);
        }

        // POST: Tareas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("TareaID,Titulo,Descripcion,Estado,FechaInicio,FechaFin,ProyectoID,UsuarioID,Prioridad")] Tarea tarea)
        {
            if (id != tarea.TareaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 🔹 Si Estado llega nulo o vacío, lo calculamos automáticamente
                    if (string.IsNullOrEmpty(tarea.Estado))
                    {
                        var hoy = DateTime.Now;
                        if (hoy < tarea.FechaInicio)
                        {
                            tarea.Estado = "Pendiente";
                        }
                        else if (hoy >= tarea.FechaInicio && hoy <= tarea.FechaFin)
                        {
                            tarea.Estado = "En Progreso";
                        }
                        else if (hoy > tarea.FechaFin)
                        {
                            tarea.Estado = "Finalizado";
                        }
                    }

                    // 🔹 Actualizamos la fecha de registro
                    tarea.FechaRegistro = DateTime.Now;

                    _context.Update(tarea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TareaExists(tarea.TareaID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            // Recargar listas de select
            ViewBag.Proyectos = new SelectList(_context.Proyecto, "ProyectoID", "NombreProyecto", tarea.ProyectoID);
            ViewBag.Usuarios = new SelectList(_context.Usuario, "UsuarioID", "NombreCompleto", tarea.UsuarioID);

            return View(tarea);
        }




        // GET: Tareas/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tarea
                .Include(t => t.Proyecto)
                .Include(t => t.Usuario)
                .Include(t => t.Comentarios)
                    .ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.TareaID == id);

            if (tarea == null)
            {
                return NotFound();
            }

            return View(tarea);
        }


        // POST: Tareas/Delete/5
        //Modificado
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Buscamos la tarea incluyendo sus comentarios
            var tarea = await _context.Tarea
                .Include(t => t.Comentarios)
                .FirstOrDefaultAsync(t => t.TareaID == id);

            if (tarea != null)
            {
                if (tarea.Comentarios != null && tarea.Comentarios.Any())
                {
                    _context.Comentario.RemoveRange(tarea.Comentarios);
                }

                _context.Tarea.Remove(tarea);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }//

        private bool TareaExists(int id)
        {
            return _context.Tarea.Any(e => e.TareaID == id);
        }


        [Authorize(Roles = "Administrador, Empleado")]
        public IActionResult Comentar(int tareaId)
        {
            var tarea = _context.Tarea
                .Include(t => t.Usuario)
                .FirstOrDefault(t => t.TareaID == tareaId);

            if (tarea == null) return NotFound();

            var comentario = new Comentario
            {
                TareaID = tarea.TareaID
            };

            return View(comentario);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador, Empleado")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comentar(Comentario comentario)
        {
            if (ModelState.IsValid)
            {
                comentario.FechaRegistro = DateTime.Now;
                comentario.UsuarioID = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

                _context.Comentario.Add(comentario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Tareas", new { id = comentario.TareaID });
            }

            return View(comentario);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador, Empleado")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarFinalizada(int id)
        {
            var tarea = await _context.Tarea.FindAsync(id);

            if (tarea == null)
            {
                return NotFound();
            }

            // Obtengo el usuario logueado
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            // Solo el asignado o el admin pueden marcar
            if (tarea.UsuarioID != userId && !User.IsInRole("Administrador"))
            {
                return Forbid();
            }

            tarea.Estado = "Finalizado";
            _context.Update(tarea);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}