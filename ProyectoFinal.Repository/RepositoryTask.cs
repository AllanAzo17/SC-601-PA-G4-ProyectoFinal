using ProyectoFinal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace ProyectoFinal.Repository
{
    public interface IRepositoryTask : IRepositoryBase<Task>
    {
        void UpdateTaskStatus(int taskId, string status);
    }
    
    public class RepositoryTask : RepositoryBase<Task>, IRepositoryTask
    {
        public RepositoryTask() : base()
        {
        }

        // Actualiza el estado de una tarea específica de forma atómica
        public void UpdateTaskStatus(int taskId, string status)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"UpdateTaskStatus: Iniciando actualización de tarea {taskId} a estado '{status}'");
                
                var task = _set.Find(taskId);
                if (task != null)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateTaskStatus: Tarea encontrada - ID: {task.TaskId}, Estado actual: {task.Status}");
                    
                    // Validar que el estado sea válido según las restricciones de la BD
                    if (status != "Pendiente" && status != "En Proceso" && status != "Finalizada" && status != "Fallida")
                    {
                        throw new ArgumentException($"Estado '{status}' no es válido. Estados permitidos: Pendiente, En Proceso, Finalizada, Fallida");
                    }
                    
                    task.Status = status;
                    _context.Entry(task).State = EntityState.Modified;
                    
                    System.Diagnostics.Debug.WriteLine($"UpdateTaskStatus: Guardando cambios en la base de datos...");
                    Save();
                    
                    System.Diagnostics.Debug.WriteLine($"UpdateTaskStatus: Tarea {taskId} actualizada exitosamente a estado: {status}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateTaskStatus: Tarea {taskId} no encontrada para actualizar");
                    throw new InvalidOperationException($"Tarea con ID {taskId} no encontrada");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateTaskStatus: Error actualizando estado de tarea {taskId}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"UpdateTaskStatus: StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}

