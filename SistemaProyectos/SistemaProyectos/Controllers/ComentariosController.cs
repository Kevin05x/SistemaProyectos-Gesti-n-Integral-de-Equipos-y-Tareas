using Microsoft.AspNetCore.Authorization;
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
    public class ComentariosController : Controller
    {
        private readonly AppDbContext _context;

        public ComentariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Comentarios
        public IActionResult Index(int page = 1, string busqueda = "", string tarea = "")
        {
            int registrosPorPagina = 10;

            var consulta = _context.Comentario
                .Include(c => c.Usuario)
                .Include(c => c.Tarea)
                .AsQueryable();

            // Filtro por nombre de tarea
            if (!string.IsNullOrEmpty(tarea))
            {
                consulta = consulta.Where(c => c.Tarea.Titulo.Contains(tarea));
            }

            // Filtro por búsqueda en contenido
            if (!string.IsNullOrEmpty(busqueda))
            {
                consulta = consulta.Where(c => c.Usuario.NombreCompleto.Contains(busqueda));
            }

            // Total de registros
            int totalRegistros = consulta.Count();

            // Paginación
            var comentarios = consulta
                .OrderBy(c => c.ComentarioID)
                .Skip((page - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            // Datos para la vista
            ViewBag.PaginaActual = page;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            int maxPaginasMostradas = 5;
            int inicio = Math.Max(1, page - maxPaginasMostradas / 2);
            int fin = Math.Min(ViewBag.TotalPaginas, inicio + maxPaginasMostradas - 1);

            ViewBag.PaginaInicio = inicio;
            ViewBag.PaginaFin = fin;

            // Mantener filtros en la vista
            ViewBag.TareaSeleccionada = tarea;
            ViewBag.Tareas = _context.Tarea
                .Select(t => t.Titulo)
                .Distinct()
                .ToList();

            ViewBag.Busqueda = busqueda;

            return View(comentarios);
        }


        // GET: Comentarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentario = await _context.Comentario
                .Include(c => c.Tarea)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.ComentarioID == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return View(comentario);
        }

        // GET: Comentarios/Create
        public IActionResult Create()
        {
            ViewData["TareaID"] = new SelectList(_context.Tarea, "TareaID", "TareaID");
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "UsuarioID", "UsuarioID");
            return View();
        }

        // POST: Comentarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ComentarioId,Contenido,UsuarioID,TareaID")] Comentario comentario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comentario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TareaID"] = new SelectList(_context.Tarea, "TareaID", "TareaID", comentario.TareaID);
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "UsuarioID", "UsuarioID", comentario.UsuarioID);
            return View(comentario);
        }

        // GET: Comentarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentario = await _context.Comentario.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }
            ViewData["TareaID"] = new SelectList(_context.Tarea, "TareaID", "TareaID", comentario.TareaID);
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "UsuarioID", "UsuarioID", comentario.UsuarioID);
            return View(comentario);
        }

        // POST: Comentarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ComentarioId,Contenido,UsuarioID,TareaID")] Comentario comentario)
        {
            if (id != comentario.ComentarioID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comentario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComentarioExists(comentario.ComentarioID))
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
            ViewData["TareaID"] = new SelectList(_context.Tarea, "TareaID", "TareaID", comentario.TareaID);
            ViewData["UsuarioID"] = new SelectList(_context.Usuario, "UsuarioID", "UsuarioID", comentario.UsuarioID);
            return View(comentario);
        }

        // GET: Comentarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentario = await _context.Comentario
                .Include(c => c.Tarea)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.ComentarioID == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return View(comentario);
        }

        // POST: Comentarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comentario = await _context.Comentario.FindAsync(id);
            if (comentario != null)
            {
                _context.Comentario.Remove(comentario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComentarioExists(int id)
        {
            return _context.Comentario.Any(e => e.ComentarioID == id);
        }
    }
}
