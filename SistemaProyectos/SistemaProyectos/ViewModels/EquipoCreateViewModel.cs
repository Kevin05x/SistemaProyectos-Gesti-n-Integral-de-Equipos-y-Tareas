using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaProyectos.Models;

namespace SistemaProyectos.ViewModels;
public class EquipoCreateViewModel
{
    // Modelo principal del equipo
    public Equipo Equipo { get; set; } = new Equipo();

    // Usuario seleccionado en el <select>
    public int? UsuarioSeleccionadoId { get; set; }

    // Lista de usuarios disponibles (para el dropdown)
    public List<SelectListItem> UsuariosDisponibles { get; set; } = new List<SelectListItem>();

    // Lista de usuarios añadidos al equipo (se mostrarán en la tabla)
    [ValidateNever]
    public List<Usuario> UsuariosSeleccionados { get; set; } = new List<Usuario>();
}

