using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SistemaProyectos.Data;
using SistemaProyectos.Models;
using SistemaProyectos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaProyectos.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ProyectosController : Controller
    {
        private readonly AppDbContext _context;

        public ProyectosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Proyectos
        public IActionResult Index(int page = 1, string estado = "", string busqueda = "", string equipo = "")
        {
            int registrosPorPagina = 10;

            var proyectosQuery = _context.Proyecto.AsQueryable();

            // Filtro por estado
            if (!string.IsNullOrEmpty(estado))
            {
                proyectosQuery = proyectosQuery.Where(p => p.Estado == estado);
            }

            // Búsqueda por nombre
            if (!string.IsNullOrEmpty(busqueda))
            {
                proyectosQuery = proyectosQuery.Where(p => p.NombreProyecto.Contains(busqueda));
            }

            // Filtrar por equipo
            if (!string.IsNullOrEmpty(equipo))
            {
                proyectosQuery = proyectosQuery.Where(p => p.Equipo.NombreEquipo.Contains(equipo));
            }

            // Total registros después de filtros y búsqueda
            int totalRegistros = proyectosQuery.Count();

            ViewData["ProyectosLista"] = proyectosQuery
                    .OrderBy(p => p.NombreProyecto)
                    .ToList();

            // Paginación
            var proyectos = proyectosQuery
                .Include(p => p.Equipo)
                .OrderBy(p => p.ProyectoID)
                .Skip((page - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

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

            ViewBag.Equipos = _context.Equipo
                .Select(e => e.NombreEquipo)
                .Distinct()
                .ToList();
            ViewBag.EquipoSeleccionado = equipo;

            return View(proyectos);
        }




        // GET: Proyectos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Traer todas las tareas del proyecto con su usuario responsable
            var tareas = _context.Tarea
                                 .Include(t => t.Usuario)
                                 .Where(t => t.ProyectoID == id) // <-- filtrar por ProyectoID
                                 .ToList();
            ViewData["ListaTareas"] = tareas;

            // Traer el proyecto con su equipo
            var proyecto = await _context.Proyecto
                                         .Include(p => p.Equipo)
                                         .FirstOrDefaultAsync(p => p.ProyectoID == id);

            if (proyecto == null)
            {
                return NotFound();
            }

            // Asignar nombre del equipo para la vista
            ViewData["NombreEquipo"] = proyecto.Equipo != null
                                        ? proyecto.Equipo.NombreEquipo
                                        : "Sin equipo asignado";

            return View(proyecto);
        }


        // GET: Proyectos/Create
        public IActionResult Create()
        {
            var vm = new ProyectoCreateViewModel
            {
                EquiposDisponibles = _context.Equipo
                    .Select(e => new SelectListItem { Value = e.EquipoID.ToString(), Text = e.NombreEquipo })
                    .ToList(),
                UsuariosDisponibles = _context.Usuario
                    .Select(u => new SelectListItem { Value = u.UsuarioID.ToString(), Text = u.NombreCompleto })
                    .ToList(),
                PrioridadesDisponibles = new List<SelectListItem>
        {
            new SelectListItem{ Value="Alta", Text="Alta" },
            new SelectListItem{ Value="Media", Text="Media" },
            new SelectListItem{ Value="Baja", Text="Baja" }
        }
            };

            return View(vm);
        }

        // POST: Proyecto/Create
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create(ProyectoCreateViewModel model, string action)
        {
            var hoy = DateTime.Now;

            // ================= AGREGAR TAREA =================
            if (action == "AgregarTarea")
            {
                // Limpiar errores de ModelState de la tarea temporal
                ModelState.Remove("Tarea.Titulo");
                ModelState.Remove("Tarea.Descripcion");
                ModelState.Remove("Tarea.UsuarioID");
                ModelState.Remove("Tarea.Prioridad");
                ModelState.Remove("Tarea.FechaInicio");
                ModelState.Remove("Tarea.FechaFin");

                if (string.IsNullOrEmpty(model.Tarea.Titulo) ||
                    string.IsNullOrEmpty(model.Tarea.Descripcion) ||
                    !model.Tarea.UsuarioID.HasValue ||
                    string.IsNullOrEmpty(model.Tarea.Prioridad) ||
                    !model.Tarea.FechaInicio.HasValue ||
                    !model.Tarea.FechaFin.HasValue)
                {
                    TempData["ErrorTarea"] = "Debes llenar todos los campos de la tarea antes de agregarla.";
                }
                else
                {
                    // Calcular estado automáticamente
                    string estadoTarea = hoy < model.Tarea.FechaInicio ? "Pendiente"
                                         : hoy <= model.Tarea.FechaFin ? "En Progreso"
                                         : "Finalizado";

                    model.TareasAcumuladas ??= new List<Tarea>();
                    model.TareasAcumuladas.Add(new Tarea
                    {
                        Titulo = model.Tarea.Titulo,
                        Descripcion = model.Tarea.Descripcion,
                        UsuarioID = model.Tarea.UsuarioID,
                        Prioridad = model.Tarea.Prioridad,
                        FechaInicio = model.Tarea.FechaInicio.Value,
                        FechaFin = model.Tarea.FechaFin.Value,
                        Estado = estadoTarea,
                        FechaRegistro = hoy
                    });

                    model.Tarea = new TareaTemporalViewModel();
                }
            }

            // ================= ELIMINAR TAREA =================
            else if (action.StartsWith("eliminar-"))
            {
                ModelState.Remove("Tarea.Titulo");
                ModelState.Remove("Tarea.Descripcion");
                ModelState.Remove("Tarea.UsuarioID");
                ModelState.Remove("Tarea.Prioridad");
                ModelState.Remove("Tarea.FechaInicio");
                ModelState.Remove("Tarea.FechaFin");

                var indexStr = action.Replace("eliminar-", "");
                if (int.TryParse(indexStr, out int index) && index >= 0 && index < model.TareasAcumuladas.Count)
                {
                    model.TareasAcumuladas.RemoveAt(index);
                }
            }

            // ================= CREAR PROYECTO =================
            else if (action == "CrearProyecto")
            {
                ModelState.Remove("Tarea.Titulo");
                ModelState.Remove("Tarea.Descripcion");
                ModelState.Remove("Tarea.UsuarioID");
                ModelState.Remove("Tarea.Prioridad");
                ModelState.Remove("Tarea.FechaInicio");
                ModelState.Remove("Tarea.FechaFin");

                // Convertir fechas de string a DateTime
                if (!DateTime.TryParse(model.ProyectoFechaInicio, out DateTime fechaInicio) ||
                    !DateTime.TryParse(model.ProyectoFechaFin, out DateTime fechaFin) ||
                    string.IsNullOrEmpty(model.Proyecto.NombreProyecto) ||
                    string.IsNullOrEmpty(model.Proyecto.Descripcion))
                {
                    TempData["ErrorProyecto"] = "Debes llenar todos los campos del proyecto antes de crearlo.";
                }
                else
                {
                    model.Proyecto.FechaInicio = fechaInicio;
                    model.Proyecto.FechaFin = fechaFin;
                    model.Proyecto.FechaRegistro = hoy;

                    model.Proyecto.Estado = hoy < fechaInicio ? "Pendiente"
                                            : hoy <= fechaFin ? "En Progreso"
                                            : "Finalizado";

                    _context.Proyecto.Add(model.Proyecto);
                    await _context.SaveChangesAsync();

                    // Guardar tareas acumuladas
                    if (model.TareasAcumuladas != null)
                    {
                        foreach (var t in model.TareasAcumuladas)
                        {
                            t.ProyectoID = model.Proyecto.ProyectoID;
                            t.Estado = hoy < t.FechaInicio ? "Pendiente"
                                       : hoy <= t.FechaFin ? "En Progreso"
                                       : "Finalizado";
                            t.FechaRegistro = hoy;
                            _context.Tarea.Add(t);
                        }
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction("Index");
                }
            }

            // ================= RECARGAR LISTAS =================
            model.UsuariosDisponibles = _context.Usuario
                .Select(u => new SelectListItem { Value = u.UsuarioID.ToString(), Text = u.NombreCompleto })
                .ToList();

            model.PrioridadesDisponibles = new List<SelectListItem>
    {
        new SelectListItem { Value = "Alta", Text = "Alta" },
        new SelectListItem { Value = "Media", Text = "Media" },
        new SelectListItem { Value = "Baja", Text = "Baja" }
    };

            model.EquiposDisponibles = _context.Equipo
                .Select(e => new SelectListItem { Value = e.EquipoID.ToString(), Text = e.NombreEquipo })
                .ToList();

            return View(model);
        }


        // GET: Proyectos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proyecto = await _context.Proyecto.FindAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }


            ViewData["EquipoID"] = new SelectList(_context.Equipo, "EquipoID", "NombreEquipo", proyecto.EquipoID);

            return View(proyecto);
        }


        // POST: Proyectos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProyectoID,NombreProyecto,Descripcion,FechaInicio,FechaFin,Estado,EquipoID")] Proyecto proyecto)
        {
            if (id != proyecto.ProyectoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 🔹 Si Estado llega nulo, lo calculamos automáticamente
                    if (string.IsNullOrEmpty(proyecto.Estado))
                    {
                        if (DateTime.Now < proyecto.FechaInicio)
                        {
                            proyecto.Estado = "Pendiente";
                        }
                        else if (DateTime.Now >= proyecto.FechaInicio && DateTime.Now <= proyecto.FechaFin)
                        {
                            proyecto.Estado = "En progreso";
                        }
                        else if (DateTime.Now > proyecto.FechaFin)
                        {
                            proyecto.Estado = "Finalizado";
                        }
                    }
                    proyecto.FechaRegistro = DateTime.Now;

                    _context.Update(proyecto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProyectoExists(proyecto.ProyectoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EquipoID"] = new SelectList(_context.Equipo, "EquipoID", "EquipoID", proyecto.EquipoID);
            return View(proyecto);
        }


        // GET: Proyectos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Traer las tareas del proyecto con el usuario responsable
            var tareas = _context.Tarea
                                 .Include(t => t.Usuario)
                                 .Where(t => t.ProyectoID == id) // ojo, usar ProyectoID para todas las tareas de este proyecto
                                 .ToList();
            ViewData["ListaTareas"] = tareas;

            // Traer el proyecto con su equipo
            var proyecto = await _context.Proyecto
                                         .Include(p => p.Equipo)
                                         .FirstOrDefaultAsync(p => p.ProyectoID == id);

            if (proyecto == null)
            {
                return NotFound();
            }

            // Asignar nombre del equipo para la vista
            if (proyecto.Equipo != null)
            {
                ViewData["NombreEquipo"] = proyecto.Equipo.NombreEquipo;
            }
            else
            {
                ViewData["NombreEquipo"] = "Sin equipo asignado";
            }

            return View(proyecto);
        }


        // POST: Proyectos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Cargar el proyecto junto con sus tareas y los comentarios de cada tarea
            var proyecto = await _context.Proyecto
                .Include(p => p.Tareas)
                    .ThenInclude(t => t.Comentarios)
                .FirstOrDefaultAsync(p => p.ProyectoID == id);

            if (proyecto != null)
            {
                // Para cada tarea del proyecto, eliminar sus comentarios
                foreach (var tarea in proyecto.Tareas)
                {
                    _context.Comentario.RemoveRange(tarea.Comentarios);
                }

                // Luego eliminar todas las tareas del proyecto
                _context.Tarea.RemoveRange(proyecto.Tareas);

                // Finalmente eliminar el proyecto
                _context.Proyecto.Remove(proyecto);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProyectoExists(int id)
        {
            return _context.Proyecto.Any(e => e.ProyectoID == id);
        }
    }
}
