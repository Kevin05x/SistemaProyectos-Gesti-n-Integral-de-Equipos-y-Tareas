using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaProyectos.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using SistemaProyectos.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaProyectos.Controllers
{
    public class ReportesExcelController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesExcelController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {

            CargarProyectosEnViewBag();
            return View();
        }

        public IActionResult ProyectosExcel(string accion = "Todos", string estado = null, string proyectoNombre = null)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Islany Gavidia");

            var query = _context.Proyecto
                .Include(p => p.Equipo)
                .AsQueryable();

            if (accion == "ProyectoNombre")
            {
                if (!string.IsNullOrEmpty(proyectoNombre))
                {
                    query = query.Where(p => p.NombreProyecto == proyectoNombre);
                }
                else
                {
                    TempData["MensajeError"] = "Debes seleccionar un proyecto válido.";
                    return RedirectToAction("Index");
                }
            }
            else if (accion == "Estado" && !string.IsNullOrEmpty(estado))
            {
                query = query.Where(p => p.Estado == estado);
            }

            var proyectos = query.ToList();

            if (!proyectos.Any())
            {
                TempData["MensajeError"] = "No se encontraron proyectos con los criterios especificados.";
                return RedirectToAction("Index");
            }

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Proyectos");

            string titulo = accion switch
            {
                "ProyectoNombre" => $"Reporte del Proyecto: {proyectoNombre}",
                "Estado" => $"Reporte de Proyectos ({estado})",
                _ => "Reporte de Todos los Proyectos"
            };

            worksheet.Cells["A1"].Value = titulo;
            worksheet.Cells["A1:H1"].Merge = true;
            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["A1"].Style.Font.Size = 16;
            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            string[] encabezados = {
                "ProyectoID", "NombreProyecto", "Descripcion",
                "FechaInicio", "FechaFin", "Estado", "EquipoID", "NombreEquipo"
            };

            for (int i = 0; i < encabezados.Length; i++)
                worksheet.Cells[3, i + 1].Value = encabezados[i];

            using (var range = worksheet.Cells[3, 1, 3, encabezados.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            for (int i = 0; i < proyectos.Count; i++)
            {
                var fila = i + 4;
                var p = proyectos[i];

                worksheet.Cells[fila, 1].Value = p.ProyectoID;
                worksheet.Cells[fila, 2].Value = p.NombreProyecto;
                worksheet.Cells[fila, 3].Value = p.Descripcion;
                worksheet.Cells[fila, 4].Value = p.FechaInicio.ToString("dd/MM/yyyy");
                worksheet.Cells[fila, 5].Value = p.FechaFin.ToString("dd/MM/yyyy");
                worksheet.Cells[fila, 6].Value = p.Estado;
                worksheet.Cells[fila, 7].Value = p.EquipoID;
                worksheet.Cells[fila, 8].Value = p.Equipo?.NombreEquipo ?? "Sin equipo";

                var estadoCell = worksheet.Cells[fila, 6];
                estadoCell.Style.Fill.PatternType = ExcelFillStyle.Solid;

                if (p.Estado == "Pendiente")
                    estadoCell.Style.Fill.BackgroundColor.SetColor(Color.Orange);
                else if (p.Estado == "En Progreso")
                    estadoCell.Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                else if (p.Estado == "Completado")
                    estadoCell.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            }

            worksheet.Cells.AutoFitColumns();

            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ReporteProyectos.xlsx");
        }

        public IActionResult ProyectoConTareasExcel(string proyectoNombre)
        {
            if (string.IsNullOrEmpty(proyectoNombre))
            {
                TempData["MensajeError"] = "Debes seleccionar un proyecto válido.";
                return RedirectToAction("Index");
            }

            ExcelPackage.License.SetNonCommercialPersonal("Islany Gavidia");

            var proyecto = _context.Proyecto
                .Include(p => p.Tareas)
                    .ThenInclude(t => t.Usuario)
                .FirstOrDefault(p => p.NombreProyecto == proyectoNombre);

            if (proyecto == null)
            {
                TempData["MensajeError"] = "Proyecto no encontrado.";
                return RedirectToAction("Index");
            }

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Proyecto");

            worksheet.Cells["A1"].Value = $"Reporte del Proyecto: {proyectoNombre}";
            worksheet.Cells["A1:J1"].Merge = true;
            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["A1"].Style.Font.Size = 16;
            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells["A3"].Value = "Proyecto ID:";
            worksheet.Cells["B3"].Value = proyecto.ProyectoID;
            worksheet.Cells["A4"].Value = "Nombre Proyecto:";
            worksheet.Cells["B4"].Value = proyecto.NombreProyecto;
            worksheet.Cells["A5"].Value = "Descripción:";
            worksheet.Cells["B5"].Value = proyecto.Descripcion;
            worksheet.Cells["A6"].Value = "Fecha Inicio:";
            worksheet.Cells["B6"].Value = proyecto.FechaInicio.ToString("dd/MM/yyyy") ?? "";
            worksheet.Cells["A7"].Value = "Fecha Fin:";
            worksheet.Cells["B7"].Value = proyecto.FechaFin.ToString("dd/MM/yyyy") ?? "";
            worksheet.Cells["A8"].Value = "Estado:";
            worksheet.Cells["B8"].Value = proyecto.Estado;
            worksheet.Cells["A9"].Value = "EquipoID:";
            worksheet.Cells["B9"].Value = proyecto.EquipoID;

            int startRow = 12;
            string[] encabezadosTareas = {
        "TareaID", "Titulo", "Descripcion", "Estado",
        "Fecha Inicio", "Fecha Fin", "ProyectoID", "UsuarioID", "NombreUsuario", "Prioridad"
    };

            for (int i = 0; i < encabezadosTareas.Length; i++)
                worksheet.Cells[startRow, i + 1].Value = encabezadosTareas[i];

            using (var range = worksheet.Cells[startRow, 1, startRow, encabezadosTareas.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            for (int i = 0; i < proyecto.Tareas.Count; i++)
            {
                var fila = startRow + 1 + i;
                var tarea = proyecto.Tareas.ElementAt(i);

                worksheet.Cells[fila, 1].Value = tarea.TareaID;
                worksheet.Cells[fila, 2].Value = tarea.Titulo;
                worksheet.Cells[fila, 3].Value = tarea.Descripcion;
                worksheet.Cells[fila, 4].Value = tarea.Estado;
                worksheet.Cells[fila, 5].Value = tarea.FechaInicio.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cells[fila, 6].Value = tarea.FechaFin.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cells[fila, 7].Value = tarea.ProyectoID;
                worksheet.Cells[fila, 8].Value = tarea.UsuarioID;
                worksheet.Cells[fila, 9].Value = tarea.Usuario?.NombreUsuario ?? "";
                worksheet.Cells[fila, 10].Value = tarea.Prioridad;

                var estadoCell = worksheet.Cells[fila, 4];
                estadoCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                if (tarea.Estado == "Pendiente")
                    estadoCell.Style.Fill.BackgroundColor.SetColor(Color.Orange);
                else if (tarea.Estado == "En Progreso")
                    estadoCell.Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                else if (tarea.Estado == "Finalizado")
                    estadoCell.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
            }

            worksheet.Cells.AutoFitColumns();

            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Proyecto_{proyectoNombre}_Reporte.xlsx");
        }


        private void CargarProyectosEnViewBag()
        {
            var proyectos = _context.Proyecto
                .OrderBy(p => p.NombreProyecto)
                .ToList();

            ViewBag.Proyectos = new SelectList(proyectos, "NombreProyecto", "NombreProyecto");
        }
    }
}