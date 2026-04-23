using SistemaProyectos.Models;

namespace SistemaProyectos.Data;

public class AcumuladorTareas
{
    private readonly List<Tarea> _tareas = new();

    // Agrega tarea al acumulador, sin afectar la identidad de la base de datos
    public void AgregarTarea(Tarea tarea)
    {
        // Solo asignamos un ID temporal local para manejar edición y eliminación
        tarea.TareaID = _tareas.Any() ? _tareas.Max(t => t.TareaID) + 1 : 1;
        _tareas.Add(tarea);
    }

    // Obtiene todas las tareas acumuladas
    public List<Tarea> ObtenerTareas() => _tareas;

    // Elimina una tarea por su ID temporal
    public void EliminarTarea(int id)
    {
        var tarea = _tareas.FirstOrDefault(t => t.TareaID == id);
        if (tarea != null) _tareas.Remove(tarea);
    }

    // Obtiene una tarea específica por su ID temporal
    public Tarea ObtenerTarea(int id)
    {
        return _tareas.FirstOrDefault(t => t.TareaID == id);
    }

    // Actualiza los datos de una tarea existente en memoria
    public void ActualizarTarea(Tarea tarea)
    {
        var existente = _tareas.FirstOrDefault(t => t.TareaID == tarea.TareaID);
        if (existente != null)
        {
            existente.Titulo = tarea.Titulo;
            existente.Descripcion = tarea.Descripcion;
            existente.Estado = tarea.Estado;
            existente.FechaInicio = tarea.FechaInicio;
            existente.FechaFin = tarea.FechaFin;
            existente.ProyectoID = tarea.ProyectoID;
            existente.UsuarioID = tarea.UsuarioID;
            existente.Prioridad = tarea.Prioridad;
        }
    }

    // Limpia el acumulador
    public void Limpiar() => _tareas.Clear();

    // Prepara las tareas para guardar en la base de datos, eliminando el ID temporal
    public List<Tarea> ObtenerTareasParaBD()
    {
        return _tareas.Select(t => new Tarea
        {
            // No asignamos TareaID, EF lo generará
            Titulo = t.Titulo,
            Descripcion = t.Descripcion,
            Estado = t.Estado,
            FechaInicio = t.FechaInicio,
            FechaFin = t.FechaFin,
            ProyectoID = t.ProyectoID,
            UsuarioID = t.UsuarioID,
            Prioridad = t.Prioridad,
            FechaRegistro = t.FechaRegistro
        }).ToList();
    }
}


