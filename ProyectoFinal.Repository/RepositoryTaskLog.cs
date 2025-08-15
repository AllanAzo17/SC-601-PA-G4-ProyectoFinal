using ProyectoFinal.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.Repository
{
    public interface IRepositoryTaskLog : IRepositoryBase<TaskLog>
    {
        IEnumerable<TaskLog> GetLogsByTaskId(int taskId);
        IEnumerable<TaskLog> GetRecentLogs(int count = 50);
        void AddLog(TaskLog log);
    }

    public class RepositoryTaskLog : RepositoryBase<TaskLog>, IRepositoryTaskLog
    {
        public RepositoryTaskLog() : base()
        {
        }

        public IEnumerable<TaskLog> GetLogsByTaskId(int taskId)
        {
            return _context.TaskLogs
                .Include("Task")
                .Where(l => l.TaskId == taskId)
                .OrderByDescending(l => l.ExecutionStart)
                .ToList();
        }

        public IEnumerable<TaskLog> GetRecentLogs(int count = 50)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"RepositoryTaskLog.GetRecentLogs: Iniciando consulta para {count} logs...");
                
                var result = _context.TaskLogs
                    .Include("Task")
                    .OrderByDescending(l => l.ExecutionStart)
                    .Take(count)
                    .ToList();
                
                System.Diagnostics.Debug.WriteLine($"RepositoryTaskLog.GetRecentLogs: Encontrados {result.Count()} logs");
                
                foreach (var log in result.Take(5))
                {
                    System.Diagnostics.Debug.WriteLine($"  - TaskId: {log.TaskId}, Title: {log.Task?.Title}, Success: {log.Success}, Start: {log.ExecutionStart}");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GetRecentLogs: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public void AddLog(TaskLog log)
        {
            _context.TaskLogs.Add(log);
            Save();
        }
    }
} 