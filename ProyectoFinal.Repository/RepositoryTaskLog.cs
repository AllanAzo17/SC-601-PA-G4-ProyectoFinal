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

        // Obtiene todos los logs de ejecución de una tarea específica ordenados por fecha
        public IEnumerable<TaskLog> GetLogsByTaskId(int taskId)
        {
            return _context.TaskLogs
                .Include("Task")
                .Where(l => l.TaskId == taskId)
                .OrderByDescending(l => l.ExecutionStart)
                .ToList();
        }

        // Obtiene los logs más recientes de ejecución limitados por cantidad
        public IEnumerable<TaskLog> GetRecentLogs(int count = 50)
        {
            return _context.TaskLogs
                .Include("Task")
                .OrderByDescending(l => l.ExecutionStart)
                .Take(count)
                .ToList();
        }

        // Agrega un nuevo log de ejecución a la base de datos
        public void AddLog(TaskLog log)
        {
            _context.TaskLogs.Add(log);
            Save();
        }
    }
} 