using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaProyectos.Data;
using SistemaProyectos.Models;
using SistemaProyectos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaProyectos.Controllers
{
    [Authorize(Roles ="Administrador")]
    public class EquiposController : Controller
    {
        private readonly AppDbContext _context;

        public EquiposController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Equipos
        public async Task<IActionResult> Index(int page = 1, string busqueda = "")
        {
            int registrosPorPagina = 10;

            var consulta = _context.Equipo.AsQueryable();

            if (!string.IsNullOrEmpty(busqueda))
            {
                consulta = consulta.Where(e => e.NombreEquipo.Contains(busqueda));
            }

            // Total registros después de filtros y búsqueda
            int totalRegistros = consulta.Count();

            var equipos = consulta
                .OrderBy(e => e.EquipoID)
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


            ViewBag.Busqueda = busqueda;

            return View(equipos);
        }



        // GET: Equipos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipo
                .Where(e => e.EquipoID == id)
                .Select(e => new
                {
                    Equipo = e,
                    Integrantes = _context.DetalleEquipo
                        .Where(d => d.EquipoID == e.EquipoID)
                        .Select(d => d.Usuario)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (equipo == null)
            {
                return NotFound();
            }

            // Pasamos un ViewModel
            var viewModel = new EquipoDetailsViewModel
            {
                EquipoID = equipo.Equipo.EquipoID,
                NombreEquipo = equipo.Equipo.NombreEquipo,
                Descripcion = equipo.Equipo.Descripcion,
                FechaRegistro = equipo.Equipo.FechaRegistro,
                Integrantes = equipo.Integrantes
            };

            ViewBag.ListaEquipos = await _context.Equipo.ToListAsync();
            return View(viewModel);
        }

        // GET: Equipos/Create
        public IActionResult Create()
        {
            var vm = new EquipoCreateViewModel
            {
                Equipo = new Equipo(),
                UsuariosDisponibles = _context.Usuario
                    .Select(u => new SelectListItem
                    {
                        Value = u.UsuarioID.ToString(),
                        Text = u.NombreCompleto
                    }).ToList(),
                UsuariosSeleccionados = new List<Usuario>()
            };

            return View(vm);
        }

        // POST: Equipos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(EquipoCreateViewModel model, string action)
        {
            // Limpiar ModelState para acciones que no requieren validación
            if (action != "Crear")
            {
                ModelState.Clear();
            }

            switch (action)
            {
                // Añadir usuario
                case "Añadir":
                    if (model.UsuarioSeleccionadoId.HasValue)
                    {
                        var usuario = await _context.Usuario.FindAsync(model.UsuarioSeleccionadoId.Value);
                        if (usuario != null && !model.UsuariosSeleccionados.Any(u => u.UsuarioID == usuario.UsuarioID))
                        {
                            model.UsuariosSeleccionados.Add(new Usuario
                            {
                                UsuarioID = usuario.UsuarioID,
                                NombreCompleto = usuario.NombreCompleto,
                                Correo = usuario.Correo
                            });
                        }
                    }
                    break;

                // Crear equipo
                case "Crear":
                    if (ModelState.IsValid)
                    {
                        var hoy = DateTime.Now;

                        // Guardar equipo
                        model.Equipo.FechaRegistro = hoy;
                        _context.Equipo.Add(model.Equipo);
                        await _context.SaveChangesAsync();

                        // Guardar relaciones con usuarios
                        foreach (var usuarioSel in model.UsuariosSeleccionados)
                        {
                            var detalle = new DetalleEquipo
                            {
                                EquipoID = model.Equipo.EquipoID,
                                UsuarioID = usuarioSel.UsuarioID
                            };
                            _context.DetalleEquipo.Add(detalle);
                        }

                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    break;

                // Eliminar usuario
                default:
                    if (action.StartsWith("eliminar-"))
                    {
                        var idStr = action.Replace("eliminar-", "");
                        if (int.TryParse(idStr, out int usuarioId))
                        {
                            var usuarioEliminar = model.UsuariosSeleccionados.FirstOrDefault(u => u.UsuarioID == usuarioId);
                            if (usuarioEliminar != null)
                                model.UsuariosSeleccionados.Remove(usuarioEliminar);
                        }
                    }
                    break;
            }

            // Recargar usuarios disponibles para el select
            model.UsuariosDisponibles = await _context.Usuario
                .Select(u => new SelectListItem
                {
                    Value = u.UsuarioID.ToString(),
                    Text = u.NombreCompleto
                }).ToListAsync();

            return View(model);
        }


        // GET: Equipos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipo.FindAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }
            return View(equipo);
        }

        // POST: Equipos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EquipoID,NombreEquipo,Descripcion,FechaRegistro")] Equipo equipo)
        {
            if (id != equipo.EquipoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var hoy = DateTime.Now;
                    equipo.FechaRegistro = hoy;

                    _context.Update(equipo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipoExists(equipo.EquipoID))
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
            return View(equipo);
        }

        // GET: Equipos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipo = await _context.Equipo
                .FirstOrDefaultAsync(m => m.EquipoID == id);

            if (equipo == null)
            {
                return NotFound();
            }

            // Traemos los integrantes del equipo
            ViewBag.Integrantes = await _context.DetalleEquipo
                .Where(d => d.EquipoID == id)
                .Select(d => d.Usuario)
                .ToListAsync();

            return View(equipo);
        }

        // POST: Equipos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipo = await _context.Equipo
                .Include(e => e.Proyectos)
                    .ThenInclude(p => p.Tareas)
                        .ThenInclude(t => t.Comentarios)
                .Include(e => e.DetalleEquipos)
                .FirstOrDefaultAsync(e => e.EquipoID == id);

            if (equipo != null)
            {
                // 1️⃣ Eliminar comentarios de todas las tareas de todos los proyectos
                foreach (var proyecto in equipo.Proyectos)
                {
                    foreach (var tarea in proyecto.Tareas)
                    {
                        _context.Comentario.RemoveRange(tarea.Comentarios);
                    }
                }

                // 2️⃣ Eliminar todas las tareas de los proyectos
                foreach (var proyecto in equipo.Proyectos)
                {
                    _context.Tarea.RemoveRange(proyecto.Tareas);
                }

                // 3️⃣ Eliminar proyectos del equipo
                _context.Proyecto.RemoveRange(equipo.Proyectos);

                // 4️⃣ Eliminar detalles de equipo
                _context.DetalleEquipo.RemoveRange(equipo.DetalleEquipos);

                // 5️⃣ Finalmente eliminar el equipo
                _context.Equipo.Remove(equipo);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }



        private bool EquipoExists(int id)
        {
            return _context.Equipo.Any(e => e.EquipoID == id);
        }
    }
}
