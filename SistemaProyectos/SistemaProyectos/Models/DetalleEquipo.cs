using System.ComponentModel.DataAnnotations;

namespace SistemaProyectos.Models;

public class DetalleEquipo
{
    [Key]
    public int DetalleEquipoID { get; set; }
    public int EquipoID { get; set; }
    public int UsuarioID { get; set; }
    public virtual Equipo? Equipo { get; set; }
    public virtual Usuario? Usuario { get; set; }

}
