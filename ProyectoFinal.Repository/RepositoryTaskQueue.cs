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
        void AddToQueue(int taskId);
    }

    public class RepositoryTaskQueue : RepositoryBase<TaskQueue>, IRepositoryTaskQueue
    {
        public RepositoryTaskQueue() : base()
        {
        }

        public IEnumerable<TaskQueue> GetPendingTasks()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("RepositoryTaskQueue.GetPendingTasks: Iniciando consulta...");
                
                var result = _context.TaskQueues
                    .Include("Task")
                    .ToList()
                    .OrderBy(q => GetPriorityOrder(q.Task.Priority))
                    .ThenBy(q => q.Task.CreatedAt)
                    .ToList();
                
                System.Diagnostics.Debug.WriteLine($"RepositoryTaskQueue.GetPendingTasks: Encontradas {result.Count()} tareas en cola");
                
                foreach (var item in result)
                {
                    System.Diagnostics.Debug.WriteLine($"  - TaskId: {item.TaskId}, Title: {item.Task?.Title}, Priority: {item.Task?.Priority}, CreatedAt: {item.Task?.CreatedAt}");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GetPendingTasks: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        private int GetPriorityOrder(string priority)
        {
            switch (priority?.ToLower())
            {
                case "alta":
                    return 1;
                case "media":
                    return 2;
                case "baja":
                    return 3;
                default:
                    return 4;
            }
        }

        public TaskQueue GetNextTaskToProcess()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("RepositoryTaskQueue.GetNextTaskToProcess: Iniciando consulta...");
                
                var result = _context.TaskQueues
                    .Include("Task")
                    .Where(q => q.Task.Status == "Pendiente")
                    .ToList()
                    .OrderBy(q => GetPriorityOrder(q.Task.Priority))
                    .ThenBy(q => q.Task.CreatedAt)
                    .FirstOrDefault();
                
                if (result != null)
                {
                    System.Diagnostics.Debug.WriteLine($"RepositoryTaskQueue.GetNextTaskToProcess: Tarea encontrada - TaskId: {result.TaskId}, Title: {result.Task?.Title}, Priority: {result.Task?.Priority}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("RepositoryTaskQueue.GetNextTaskToProcess: No se encontraron tareas pendientes");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GetNextTaskToProcess: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public void RemoveFromQueue(int taskId)
        {
            var queueItem = _context.TaskQueues.FirstOrDefault(q => q.TaskId == taskId);
            if (queueItem != null)
            {
                _context.TaskQueues.Remove(queueItem);
                Save();
            }
        }

        public void AddToQueue(int taskId)
        {
            try
            {
                var existingItem = _context.TaskQueues.FirstOrDefault(q => q.TaskId == taskId);
                if (existingItem != null)
                {
                    System.Diagnostics.Debug.WriteLine($"RepositoryTaskQueue.AddToQueue: La tarea {taskId} ya está en la cola");
                    return;
                }

                var queueItem = new TaskQueue
                {
                    TaskId = taskId,
                    EnqueuedAt = DateTime.Now
                };

                _context.TaskQueues.Add(queueItem);
                Save();

                System.Diagnostics.Debug.WriteLine($"RepositoryTaskQueue.AddToQueue: Tarea {taskId} agregada a la cola exitosamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en AddToQueue: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
} 