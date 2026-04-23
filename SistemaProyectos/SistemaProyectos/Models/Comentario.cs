using System.ComponentModel.DataAnnotations;

namespace SistemaProyectos.Models;

public class Comentario
{
    [Key]
    public int ComentarioID { get; set; }


    [Required(ErrorMessage = "El contenido es obligatorio")]
    [MinLength(5)]
    [Display(Name = "Contenido:")]
    public string Contenido { get; set; }
    

    public int UsuarioID { get; set; }
    public int TareaID { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaRegistro { get; set; }

    public virtual Usuario? Usuario { get; set; }
    public virtual Tarea? Tarea { get; set; }

}
