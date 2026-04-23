using Microsoft.EntityFrameworkCore;
using SistemaProyectos.Models;

namespace SistemaProyectos.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SistemaProyectos.Models.Usuario> Usuario { get; set; } = default!;
        public DbSet<SistemaProyectos.Models.Proyecto> Proyecto { get; set; } = default!;
        public DbSet<SistemaProyectos.Models.Equipo> Equipo { get; set; } = default!;
        public DbSet<SistemaProyectos.Models.Tarea> Tarea { get; set; } = default!;
        public DbSet<SistemaProyectos.Models.Comentario> Comentario { get; set; } = default!;
        public DbSet<SistemaProyectos.Models.DetalleEquipo> DetalleEquipo { get; set; } = default!;
    }
}
