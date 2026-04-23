using SistemaProyectos.Models;
using System;
using System.Collections.Generic;

namespace SistemaProyectos.ViewModels
{
    public class HomeUserIndex
    {
        // Datos básicos del empleado
        public int UsuarioID { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Lista de tareas asignadas
        public List<TareaResumen> Tareas { get; set; } = new List<TareaResumen>();

        // Lista de equipos donde pertenece el usuario
        public List<EquipoResumen> Equipos { get; set; } = new List<EquipoResumen>();
    }

    // DTO pequeño para mostrar solo lo necesario de la tarea
    public class TareaResumen
    {
        public int TareaID { get; set; }
        public string Titulo { get; set; }
        public string Estado { get; set; }
    }

    // DTO pequeño para mostrar solo lo necesario del equipo
    public class EquipoResumen
    {
        public int EquipoID { get; set; }
        public string NombreEquipo { get; set; }
    }
}
