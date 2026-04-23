using System.ComponentModel.DataAnnotations;

namespace SistemaProyectos.Models;

public class Tarea : IValidatableObject
{
    [Key]
    public int TareaID { get; set; }

    [Required(ErrorMessage = "El título de la tarea es obligatorio")]
    [StringLength(200, MinimumLength = 3)]
    [Display(Name = "Título de la tarea:")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "La descripción de la tarea es obligatorio")]
    [MinLength(5)]
    [Display(Name = "Descripción de la tarea:")]
    public string Descripcion { get; set; }

    [Display(Name = "Estado de la tarea:")]
    public string? Estado { get; set; }

    [Required(ErrorMessage = "La fecha de inicio de la tarea es obligatorio")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio de la tarea:")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin de la tarea es obligatorio")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de fin de la tarea:")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
    public DateTime FechaFin { get; set; }

    [Display(Name = "Proyecto:")]
    [Required(ErrorMessage = "Elige un proyecto")]
    public int? ProyectoID { get; set; }

    [Display(Name = "Usuario:")]
    [Required(ErrorMessage = "Elige un usuario")]
    public int? UsuarioID { get; set; }

    [Display(Name = "Prioridad de la tarea:")]
    [Required(ErrorMessage = "Elige la prioridad")]
    public string Prioridad { get; set; }

    [DataType(DataType.Date)]
    public DateTime? FechaRegistro { get; set; }

    public virtual Proyecto? Proyecto { get; set; }
    public virtual Usuario? Usuario { get; set; }
    public virtual ICollection<Comentario>? Comentarios { get; set; }

    // Validación: FechaInicio < FechaFin
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
}
