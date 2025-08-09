using ProyectoFinal.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.Repository
{
    public interface IRepositoryTaskQueue : IRepositoryBase<TaskQueue>
    {
        IEnumerable<TaskQueue> GetPendingTasks();
        TaskQueue GetNextTaskToProcess();
        void RemoveFromQueue(int taskId);
    }

    public class RepositoryTaskQueue : RepositoryBase<TaskQueue>, IRepositoryTaskQueue
    {
        public RepositoryTaskQueue() : base()
        {
        }

        // Obtiene todas las tareas pendientes ordenadas por prioridad y fecha de encolado
        public IEnumerable<TaskQueue> GetPendingTasks()
        {
            return _context.TaskQueues
                .Include("Task")
                .Where(q => q.Task.Status == "Pendiente")
                .OrderByDescending(q => q.Task.Priority)
                .ThenBy(q => q.EnqueuedAt)
                .ToList();
        }

        // Obtiene la siguiente tarea a procesar según prioridad y orden FIFO
        public TaskQueue GetNextTaskToProcess()
        {
            return _context.TaskQueues
                .Include("Task")
                .Where(q => q.Task.Status == "Pendiente")
                .OrderByDescending(q => q.Task.Priority)
                .ThenBy(q => q.EnqueuedAt)
                .FirstOrDefault();
        }

        // Remueve una tarea específica de la cola de procesamiento
        public void RemoveFromQueue(int taskId)
        {
            var queueItem = _context.TaskQueues.FirstOrDefault(q => q.TaskId == taskId);
            if (queueItem != null)
            {
                _context.TaskQueues.Remove(queueItem);
                Save();
            }
        }
    }
} 