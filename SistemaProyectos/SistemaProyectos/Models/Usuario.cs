using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.ComponentModel.DataAnnotations;

namespace SistemaProyectos.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioID { get; set; }
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [Display(Name = "Nombre de usuario:")]
        [StringLength(200, MinimumLength = 5)]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [Display(Name = "Nombre completo:")]
        [StringLength(250, MinimumLength = 5)]
        public string NombreCompleto { get; set; }
        [Required(ErrorMessage = "El correo del usuario es obligatorio")]
        [Display(Name = "Correo:")]
        [StringLength(150, MinimumLength = 5)]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }
        [Required(ErrorMessage = "La contraseña del usuario es obligatorio")]
        [Display(Name = "Contraseña:")]
        [StringLength(255, MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        [Required(ErrorMessage = "El rol del usuario es obligatorio")]
        [Display(Name = "Rol:")]
        [MaxLength(100)]
        public string Rol { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; }
        public virtual ICollection<Tarea>? Tareas { get; set; }
        public virtual ICollection<Comentario>? Comentarios { get; set; }
        public virtual ICollection<DetalleEquipo>? DetalleEquipos { get; set; }
    }
}
