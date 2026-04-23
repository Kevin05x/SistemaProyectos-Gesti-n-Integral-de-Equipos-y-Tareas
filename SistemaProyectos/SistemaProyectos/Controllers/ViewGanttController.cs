using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaProyectos.Data;
using SistemaProyectos.Models;

namespace SistemaProyectos.Controllers
{
    [Authorize(Roles ="Administrador")]
    public class ViewGanttController : Controller
    {
        private readonly AppDbContext _context;

        public ViewGanttController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Gantt(int? proyectoId, string? vista)
        {
            var proyectos = _context.Proyecto
                .Select(p => new { p.ProyectoID, p.NombreProyecto })
                .ToList();

            ViewBag.Proyectos = new SelectList(proyectos, "ProyectoID", "NombreProyecto", proyectoId);

            if (proyectoId == null)
            {
                ViewBag.DefaultView = "Day";
                ViewBag.TareasJson = "[]";
                ViewBag.NombreProyecto = "";
                return View();
            }

            var proyecto = _context.Proyecto.FirstOrDefault(p => p.ProyectoID == proyectoId);
            ViewBag.NombreProyecto = proyecto?.NombreProyecto ?? "Proyecto no encontrado";

            var tareas = _context.Tarea
                .Where(t => t.ProyectoID == proyectoId)
                .Select(t => new
                {
                    id = t.TareaID,
                    name = t.Titulo,
                    start = t.FechaInicio.ToString("yyyy-MM-dd"),
                    end = t.FechaFin.ToString("yyyy-MM-dd"),
                    progress = 0,
                    estado = t.Estado
                })
                .ToList();

            var maxDias = tareas.Any()
                ? (DateTime.Parse(tareas.Max(t => t.end)) - DateTime.Parse(tareas.Min(t => t.start))).TotalDays
                : 0;

            var defaultView = (maxDias >= 60) ? "Month" :
                             (maxDias >= 14) ? "Week" : "Day";

            ViewBag.DefaultView = vista ?? defaultView;
            ViewBag.TareasJson = JsonSerializer.Serialize(tareas);
            ViewBag.ProyectoSeleccionado = proyectoId;

            return View();
        }
    }
}
