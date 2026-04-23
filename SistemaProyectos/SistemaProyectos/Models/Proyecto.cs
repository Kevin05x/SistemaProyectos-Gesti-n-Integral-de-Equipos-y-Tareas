using System.ComponentModel.DataAnnotations;

namespace SistemaProyectos.Models;

public class Proyecto : IValidatableObject
{
    [Key]
    public int ProyectoID { get; set; }


    [Required(ErrorMessage = "El nombre del proyecto es obligatorio")]
    [StringLength(200, MinimumLength = 3)]
    [Display(Name = "Nombre del proyecto:")]
    public string NombreProyecto { get; set; }


    [Required(ErrorMessage = "La descripción del proyecto es obligatorio")]
    [Display(Name = "Descripción del proyecto:")]
    [MinLength(5)]
    public string Descripcion { get; set; }


    [Required(ErrorMessage = "La fecha de inicio del proyecto es obligatorio")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio del proyecto:")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
    public DateTime FechaInicio { get; set; }


    [Required(ErrorMessage = "La fecha de fin del proyecto es obligatorio")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de fin del proyecto:")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
    public DateTime FechaFin { get; set; }


    [Display(Name = "Estado del proyecto:")]
    public string? Estado { get; set; }


    [Display(Name = "Equipo:")]
    [Required(ErrorMessage = "Elige un equipo")]
    public int? EquipoID { get; set; }


    [DataType(DataType.Date)]
    public DateTime? FechaRegistro { get; set; }


    //Valida que la fecha de inicio sea menor que la fecha fin
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FechaInicio >= FechaFin)
        {
            yield return new ValidationResult(
                "La fecha de inicio debe ser menor que la fecha de fin",
                new[] { nameof(FechaInicio), nameof(FechaFin) }
            );
        }
    }

    public virtual Equipo? Equipo { get; set; }
    public virtual ICollection<Tarea>? Tareas { get; set; }

}

