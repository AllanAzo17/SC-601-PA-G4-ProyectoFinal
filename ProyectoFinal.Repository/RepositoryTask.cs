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
                var task = _set.Find(taskId);
                if (task != null)
                {
                    task.Status = status;
                    _context.Entry(task).State = EntityState.Modified;
                    Save();
                    System.Diagnostics.Debug.WriteLine($"Tarea {taskId} actualizada a estado: {status}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Tarea {taskId} no encontrada para actualizar");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error actualizando estado de tarea {taskId}: {ex.Message}");
                throw;
            }
        }
    }
}

