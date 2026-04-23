using System.ComponentModel.DataAnnotations;

namespace SistemaProyectos.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Campo usuario es necesario")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "Campo contraseña es necesario")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }
    }
}
