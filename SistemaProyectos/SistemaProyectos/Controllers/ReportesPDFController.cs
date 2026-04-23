using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaProyectos.Data;
using SistemaProyectos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaProyectos.Controllers
{
    public class ReportesPDFController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesPDFController(AppDbContext context)
        {
            _context = context;
        }

        // Nueva acción para mostrar la vista con el select
        public IActionResult Index()
        {
            CargarProyectosEnViewBag();
            return View();
        }

        public IActionResult ProyectosPDF(string accion = "Todos", string estado = null, string proyectoNombre = null)
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;

                var query = _context.Proyecto
                    .Include(p => p.Equipo)
                    .AsQueryable();

                if (accion == "ProyectoNombre")
                {
                    if (!string.IsNullOrEmpty(proyectoNombre))
                        query = query.Where(p => p.NombreProyecto == proyectoNombre);
                    else
                    {
                        TempData["MensajeError"] = "Debes seleccionar un proyecto válido.";
                        return RedirectToAction("Index");
                    }
                }
                else if (accion == "Estado" && !string.IsNullOrEmpty(estado))
                    query = query.Where(p => p.Estado == estado);

                var proyectos = query.ToList();

                if (!proyectos.Any())
                {
                    TempData["MensajeError"] = "No se encontraron proyectos con los criterios especificados.";
                    return RedirectToAction("Index");
                }

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        string titulo = accion switch
                        {
                            "ProyectoNombre" => $"Reporte del Proyecto: {proyectoNombre}",
                            "Estado" => $"📚 Reporte de Proyectos ({estado})",
                            _ => "📚 Reporte de Todos los Proyectos"
                        };

                        page.Header().Text(titulo)
                            .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            string[] encabezados = {
                                "ProyectoID", "NombreProyecto", "Descripcion",
                                "FechaInicio", "FechaFin", "Estado", "EquipoID", "NombreEquipo"
                            };

                            foreach (var encabezado in encabezados)
                                table.Cell().Element(CellStyle).Text(encabezado).Bold();

                            foreach (var p in proyectos)
                            {
                                table.Cell().Element(CellStyle).Text(p.ProyectoID.ToString());
                                table.Cell().Element(CellStyle).Text(p.NombreProyecto ?? "");
                                table.Cell().Element(CellStyle).Text(p.Descripcion ?? "");
                                table.Cell().Element(CellStyle).Text(p.FechaInicio.ToString("dd/MM/yyyy"));
                                table.Cell().Element(CellStyle).Text(p.FechaFin.ToString("dd/MM/yyyy"));
                                table.Cell().Element(CellStyle).Text(p.Estado ?? "");
                                table.Cell().Element(CellStyle).Text(p.EquipoID.ToString());
                                table.Cell().Element(CellStyle).Text(p.Equipo?.NombreEquipo ?? "Sin equipo");
                            }
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                    });
                });

                var stream = new MemoryStream();
                document.GeneratePdf(stream);
                stream.Position = 0;

                return File(stream, "application/pdf", "ReporteProyectos.pdf");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Ocurrió un error al generar el reporte: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult ProyectoConTareasPDF(string proyectoNombre)
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;

                var proyecto = _context.Proyecto
                    .Include(p => p.Tareas)
                        .ThenInclude(t => t.Usuario)
                    .FirstOrDefault(p => p.NombreProyecto == proyectoNombre);

                if (proyecto == null)
                {
                    TempData["MensajeError"] = "Proyecto no encontrado.";
                    return RedirectToAction("Index");
                }

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11));

                        page.Header()
                            .AlignCenter()
                            .Text($"Reporte del Proyecto: {proyecto.NombreProyecto}")
                            .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                        page.Content().Column(col =>
                        {
                            col.Spacing(5);

                            col.Item().Text($"ID: {proyecto.ProyectoID}");
                            col.Item().Text($"Descripción: {proyecto.Descripcion}");
                            col.Item().Text($"Fecha Inicio: {proyecto.FechaInicio:dd/MM/yyyy}");
                            col.Item().Text($"Fecha Fin: {proyecto.FechaFin:dd/MM/yyyy}");
                            col.Item().Text($"Estado: {proyecto.Estado}");
                            col.Item().Text($"EquipoID: {proyecto.EquipoID}");

                            col.Item().PaddingVertical(10).Text("Tareas del Proyecto")
                                .Bold().FontSize(14).FontColor(Colors.Black);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(50);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1.5f);
                                });

                                string[] headers = { "TareaID", "Título", "Descripción", "Estado", "Fecha Inicio", "Fecha Fin", "UsuarioID", "NombreUsuario", "Prioridad" };
                                foreach (var header in headers)
                                    table.Cell().Element(CellStyle).Text(header).Bold();

                                foreach (var tarea in proyecto.Tareas)
                                {
                                    table.Cell().Element(CellStyle).Text(tarea.TareaID.ToString());
                                    table.Cell().Element(CellStyle).Text(tarea.Titulo ?? "");
                                    table.Cell().Element(CellStyle).Text(tarea.Descripcion ?? "");
                                    table.Cell().Element(CellStyle).Text(tarea.Estado ?? "");
                                    table.Cell().Element(CellStyle).Text(tarea.FechaInicio.ToString("dd/MM/yyyy") ?? "");
                                    table.Cell().Element(CellStyle).Text(tarea.FechaFin.ToString("dd/MM/yyyy") ?? "");
                                    table.Cell().Element(CellStyle).Text(tarea.UsuarioID.ToString());
                                    table.Cell().Element(CellStyle).Text(tarea.Usuario?.NombreUsuario ?? "");
                                    table.Cell().Element(CellStyle).Text(tarea.Prioridad ?? "");
                                }
                            });
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                    });
                });

                var stream = new MemoryStream();
                document.GeneratePdf(stream);
                stream.Position = 0;

                return File(stream, "application/pdf", $"Proyecto_{proyecto.NombreProyecto}_Reporte.pdf");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Ocurrió un error al generar el reporte: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        private IContainer CellStyle(IContainer container) =>
            container.PaddingVertical(5).PaddingHorizontal(10).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

        private void CargarProyectosEnViewBag()
        {
            var proyectos = _context.Proyecto
                .OrderBy(p => p.NombreProyecto)
                .ToList();

            ViewBag.Proyectos = new SelectList(proyectos, "NombreProyecto", "NombreProyecto");
        }
    }
}