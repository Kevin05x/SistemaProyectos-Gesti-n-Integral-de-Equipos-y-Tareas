using SistemaProyectos.Models;
using System;
using System.Collections.Generic;

namespace SistemaProyectos.ViewModels
{
    public class EquipoDetailsViewModel
    {
        public int EquipoID { get; set; }
        public string NombreEquipo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public List<Usuario> Integrantes { get; set; } = new();
    }
}
