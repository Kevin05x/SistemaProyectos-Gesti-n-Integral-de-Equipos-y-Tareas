using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaProyectos.Data;
using SistemaProyectos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaProyectos.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public IActionResult Index(int page = 1, string busqueda = "", string rol = "", string equipo = "")
        {
            int registrosPorPagina = 10;

            var consulta = _context.Usuario
                .Include(u => u.DetalleEquipos)
                    .ThenInclude(de => de.Equipo)
                .AsQueryable();

            if (!string.IsNullOrEmpty(rol))
            {
                consulta = consulta.Where(u => u.Rol == rol);
            }

            if (!string.IsNullOrEmpty(busqueda))
            {
                consulta = consulta.Where(u => u.NombreCompleto.Contains(busqueda));
            }

            if (!string.IsNullOrEmpty(equipo))
            {
                consulta = consulta.Where(u => u.DetalleEquipos.Any(de => de.Equipo.NombreEquipo == equipo));
            }

            int totalRegistros = consulta.Count();

            var usuarios = consulta
                .OrderBy(u => u.UsuarioID)
                .Skip((page - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            ViewBag.PaginaActual = page;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            int maxPaginasMostradas = 5;
            int inicio = Math.Max(1, page - maxPaginasMostradas / 2);
            int fin = Math.Min(ViewBag.TotalPaginas, inicio + maxPaginasMostradas - 1);

            ViewBag.PaginaInicio = inicio;
            ViewBag.PaginaFin = fin;

            // Guardar valores seleccionados para mantener filtros en la vista
            ViewBag.Busqueda = busqueda;

            ViewBag.RolSeleccionado = rol;
            ViewBag.Roles = _context.Usuario.Select(u => u.Rol).Distinct().ToList();

            ViewBag.EquipoSeleccionado = equipo;
            ViewBag.Equipos = _context.Equipo.Select(e => e.NombreEquipo).Distinct().ToList();

            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.UsuarioID == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.EquiposDisponibles = await _context.Equipo
                .Select(e => new SelectListItem
                {
                    Value = e.EquipoID.ToString(),
                    Text = e.NombreEquipo
                })
                .ToListAsync();

            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NombreUsuario,NombreCompleto,Correo,Contrasena,Rol")] Usuario usuario, int? EquipoID)
        {
            if (ModelState.IsValid)
            {
                usuario.FechaRegistro = DateTime.Now; // Fecha automática

                //CREACIÓN DEL HASH
                var passwordHasher = new PasswordHasher<Usuario>();
                //ENCRIPTACIÓN DE LA CONTRASEÑA
                usuario.Contrasena = passwordHasher.HashPassword(usuario, usuario.Contrasena);

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                if (EquipoID.HasValue)
                {
                    var detalle = new DetalleEquipo
                    {
                        UsuarioID = usuario.UsuarioID,
                        EquipoID = EquipoID.Value
                    };

                    _context.DetalleEquipo.Add(detalle);
                    await _context.SaveChangesAsync();
                }


                return RedirectToAction(nameof(Index));
            }

            ViewBag.EquiposDisponibles = await _context.Equipo
                .Select(e => new SelectListItem
                {
                    Value = e.EquipoID.ToString(),
                    Text = e.NombreEquipo
                })
                .ToListAsync();


            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioID,NombreUsuario,NombreCompleto,Correo,Contrasena,Rol,FechaRegistro")] Usuario usuario)
        {
            if (id != usuario.UsuarioID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioDb = await _context.Usuario.FindAsync(id);
                    if (usuarioDb == null) return NotFound();

                    // Actualizar datos
                    usuarioDb.NombreUsuario = usuario.NombreUsuario;
                    usuarioDb.NombreCompleto = usuario.NombreCompleto;
                    usuarioDb.Correo = usuario.Correo;
                    usuarioDb.Rol = usuario.Rol;

                    if (!string.IsNullOrWhiteSpace(usuario.Contrasena) &&
                        usuario.Contrasena != usuarioDb.Contrasena)
                    {
                        var passwordHasher = new PasswordHasher<Usuario>();
                        usuarioDb.Contrasena = passwordHasher.HashPassword(usuarioDb, usuario.Contrasena);
                    }

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioID))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.UsuarioID == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuario
                .Include(u => u.Tareas)
                    .ThenInclude(t => t.Comentarios)
                .Include(u => u.Comentarios) // comentarios hechos directamente por el usuario
                .Include(u => u.DetalleEquipos)
                .FirstOrDefaultAsync(u => u.UsuarioID == id);

            if (usuario != null)
            {
                // 1. Eliminar comentarios de las tareas del usuario
                foreach (var tarea in usuario.Tareas)
                {
                    _context.Comentario.RemoveRange(tarea.Comentarios);
                }

                // 2. Eliminar comentarios hechos directamente por el usuario
                _context.Comentario.RemoveRange(usuario.Comentarios);

                // 3. Eliminar tareas del usuario
                _context.Tarea.RemoveRange(usuario.Tareas);

                // 4. Eliminar detalles de equipo
                _context.DetalleEquipo.RemoveRange(usuario.DetalleEquipos);

                // 5. Eliminar usuario
                _context.Usuario.Remove(usuario);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.UsuarioID == id);
        }
    }
}