using System.ComponentModel.DataAnnotations;

namespace SistemaProyectos.Models;

public class Equipo
{
    [Key]
    public int EquipoID { get; set; }


    [Required(ErrorMessage = "El nombre del equipo es obligatorio")]
    [StringLength(200, MinimumLength = 3)]
    [Display(Name = "Nombre del equipo:")]
    public string NombreEquipo { get; set; }


    [Required(ErrorMessage = "La descripción del equipo es obligatorio")]
    [MinLength(5)]
    [Display(Name = "Descripción del equipo:")]
    public string Descripcion { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaRegistro { get; set; }
    public virtual ICollection<Proyecto>? Proyectos { get; set; }
    public virtual ICollection<DetalleEquipo>? DetalleEquipos { get; set; }
}
