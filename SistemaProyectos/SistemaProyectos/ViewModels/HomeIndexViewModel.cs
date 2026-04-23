using SistemaProyectos.Models;

namespace SistemaProyectos.ViewModels
{
    public class HomeIndexViewModel
    {
        // Datos básicos del empleado/usuario logueado
        public int UsuarioID { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Lista de tareas asignadas
        public List<TareaResumen> Tareas { get; set; } = new List<TareaResumen>();

        // Lista de equipos donde pertenece el usuario
        public List<EquipoResumen> Equipos { get; set; } = new List<EquipoResumen>();

        // Dashboards de administrador
        public List<ProyectoPorcentaje> Proyectos { get; set; } = new();
        public List<EquipoCnTareas> EquiposCnTareas { get; set; } = new();
        public List<ComentarioReciente> ComentariosRecientes { get; set; } = new();
        public List<TareasFueraDePlazo> TareasFueraDePlazo { get; set; } = new();
    }

    public class EquipoCnTareas
    {
        public string NombreEquipo { get; set; }
        public int CantidadTareas { get; set; }
    }

    public class ComentarioReciente
    {
        public string NombreUsuario { get; set; }
        public string Contenido { get; set; }
        public string NombreTarea { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class TareasFueraDePlazo
    {
        public string NombreTareaRetrasada { get; set; }
        public int DiasRetrasada { get; set; }
        public string NombreUsuarioEncargado { get; set; }
    }

    public class ProyectoPorcentaje
    {
        public string NombreProyecto { get; set; }
        public double PorcentajeCompletado { get; set; }
    }
}
