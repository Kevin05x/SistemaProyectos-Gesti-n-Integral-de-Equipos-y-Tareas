using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaProyectos.Models;

namespace SistemaProyectos.ViewModels;

public class ProyectoCreateViewModel
{
    public Proyecto Proyecto { get; set; } = new Proyecto();

    public string ProyectoFechaInicio { get; set; }
    public string ProyectoFechaFin { get; set; }

    // Usamos la tarea temporal aquí
    public TareaTemporalViewModel Tarea { get; set; } = new TareaTemporalViewModel();

    public List<Tarea> TareasAcumuladas { get; set; } = new List<Tarea>();

    // Selects
    public List<SelectListItem> EquiposDisponibles { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> UsuariosDisponibles { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> PrioridadesDisponibles { get; set; } = new List<SelectListItem>();
}