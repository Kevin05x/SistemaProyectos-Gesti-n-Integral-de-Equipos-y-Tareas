using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaProyectos.Data;
using SistemaProyectos.Models;
using SistemaProyectos.ViewModels;
using System.Diagnostics;

namespace SistemaProyectos.Controllers
{
    [Authorize(Roles = "Administrador, Empleado")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.OcultarFooter = true;

            // Usuario logueado
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            var usuario = _context.Usuario
                .Include(u => u.Tareas)
                .Include(u => u.DetalleEquipos)
                    .ThenInclude(de => de.Equipo)
                .FirstOrDefault(u => u.UsuarioID == userId);

            if (usuario == null) return NotFound();

            // Dashboards de admin
            var proyectos = _context.Proyecto
                .Include(p => p.Tareas)
                .Select(p => new ProyectoPorcentaje
                {
                    NombreProyecto = p.NombreProyecto,
                    PorcentajeCompletado = Math.Round(
                        p.Tareas.Any()
                            ? (double)p.Tareas.Count(t => t.Estado == "Finalizado") / p.Tareas.Count() * 100
                            : 0, 2)
                }).ToList();

            var equiposConTareas = _context.Equipo
                .Select(equipo => new EquipoCnTareas
                {
                    NombreEquipo = equipo.NombreEquipo,
                    CantidadTareas = _context.DetalleEquipo
                        .Where(de => de.EquipoID == equipo.EquipoID)
                        .Join(
                            _context.Tarea,
                            de => de.UsuarioID,
                            t => t.UsuarioID,
                            (de, t) => t
                        ).Count()
                })
                .OrderByDescending(x => x.CantidadTareas)
                .Take(15)
                .ToList();

            var ultimosComentarios = _context.Comentario
                .Join(_context.Usuario,
                    c => c.UsuarioID,
                    u => u.UsuarioID,
                    (c, u) => new { Comentario = c, Usuario = u })
                .Join(_context.Tarea,
                    cu => cu.Comentario.TareaID,
                    t => t.TareaID,
                    (cu, t) => new ComentarioReciente
                    {
                        NombreUsuario = cu.Usuario.NombreCompleto,
                        NombreTarea = t.Titulo,
                        Contenido = cu.Comentario.Contenido,
                        FechaRegistro = cu.Comentario.FechaRegistro,
                    })
                .OrderByDescending(c => c.FechaRegistro)
                .Take(5)
                .ToList();

            var tareasVencidas = _context.Tarea
                .Join(_context.Usuario,
                    t => t.UsuarioID,
                    u => u.UsuarioID,
                    (t, u) => new { Tarea = t, Usuario = u })
                .ToList()
                .Select(x => new TareasFueraDePlazo
                {
                    NombreTareaRetrasada = x.Tarea.Titulo,
                    NombreUsuarioEncargado = x.Usuario.NombreCompleto,
                    DiasRetrasada = (DateTime.Now - x.Tarea.FechaFin).Days,
                })
                .OrderByDescending(x => x.DiasRetrasada)
                .Take(5)
                .ToList();

            // ViewModel combinado
            var vm = new HomeIndexViewModel
            {
                UsuarioID = usuario.UsuarioID,
                NombreCompleto = usuario.NombreCompleto,
                Correo = usuario.Correo,
                FechaRegistro = usuario.FechaRegistro,
                Tareas = usuario.Tareas?.Select(t => new TareaResumen
                {
                    TareaID = t.TareaID,
                    Titulo = t.Titulo,
                    Estado = t.Estado ?? "Pendiente"
                }).ToList() ?? new List<TareaResumen>(),
                Equipos = usuario.DetalleEquipos?.Select(de => new EquipoResumen
                {
                    EquipoID = de.Equipo.EquipoID,
                    NombreEquipo = de.Equipo.NombreEquipo
                }).ToList() ?? new List<EquipoResumen>(),

                Proyectos = proyectos,
                EquiposCnTareas = equiposConTareas,
                ComentariosRecientes = ultimosComentarios,
                TareasFueraDePlazo = tareasVencidas,
            };

            ViewBag.Usuario = User.Identity.Name;

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult HomeEmpleado()
        {
            // Obtener usuario logueado
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            var usuario = _context.Usuario
                .Include(u => u.Tareas)
                .Include(u => u.DetalleEquipos)
                    .ThenInclude(de => de.Equipo)
                .FirstOrDefault(u => u.UsuarioID == userId);

            if (usuario == null) return NotFound();

            var vm = new HomeUserIndex
            {
                UsuarioID = usuario.UsuarioID,
                NombreCompleto = usuario.NombreCompleto,
                Correo = usuario.Correo,
                FechaRegistro = usuario.FechaRegistro,
                Tareas = usuario.Tareas?.Select(t => new TareaResumen
                {
                    TareaID = t.TareaID,
                    Titulo = t.Titulo,
                    Estado = t.Estado ?? "Pendiente"
                }).ToList() ?? new List<TareaResumen>(),
                Equipos = usuario.DetalleEquipos?.Select(de => new EquipoResumen
                {
                    EquipoID = de.Equipo.EquipoID,
                    NombreEquipo = de.Equipo.NombreEquipo
                }).ToList() ?? new List<EquipoResumen>()
            };

            return View(vm);
        }
    }
}
