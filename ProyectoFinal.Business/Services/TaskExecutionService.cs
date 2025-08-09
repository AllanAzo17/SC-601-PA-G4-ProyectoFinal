using ProyectoFinal.Data;
using ProyectoFinal.Business.Interfaces;
using System;
using System.Threading;
using TaskEntity = ProyectoFinal.Data.Task;
using ProyectoFinal.Repository;

namespace ProyectoFinal.Business.Services
{
    public class TaskExecutionService
    {
        private readonly INotificationService _notificationService;
        private readonly IRepositoryTaskLog _taskLogRepository;

        // Constructor que inicializa servicios para notificaciones y logging de tareas
        public TaskExecutionService(INotificationService notificationService, IRepositoryTaskLog taskLogRepository)
        {
            _notificationService = notificationService;
            _taskLogRepository = taskLogRepository;
        }

        // Simula la ejecución de una tarea con tiempos variables según prioridad y 95% de éxito
        public bool ExecuteTask(TaskEntity task)
        {
            var log = new TaskLog
            {
                TaskId = task.TaskId,
                ExecutionStart = DateTime.Now,
                Success = false,
                LogDetails = $"Simulando ejecución de tarea: {task.Title}"
            };

            try
            {
                int executionTime = GetExecutionTime(task.Priority);
                Thread.Sleep(executionTime);
                
                bool success = (task.TaskId % 20) != 0;
                
                log.ExecutionEnd = DateTime.Now;
                log.Success = success;
                log.LogDetails = success ? 
                    $"Tarea simulada completada: {task.Title}" : 
                    $"Tarea simulada falló: {task.Title}";
                
                if (!success)
                {
                    log.ErrorMessage = "Error simulado";
                }

                _taskLogRepository.AddLog(log);

                if (success)
                {
                    _notificationService.SendTaskCompletedNotification(task, log);
                }
                else
                {
                    _notificationService.SendTaskFailedNotification(task, log);
                }

                return success;
            }
            catch (Exception ex)
            {
                log.ExecutionEnd = DateTime.Now;
                log.Success = false;
                log.ErrorMessage = ex.Message;
                log.LogDetails = $"Error en simulación: {ex.Message}";

                _taskLogRepository.AddLog(log);
                _notificationService.SendTaskFailedNotification(task, log);

                return false;
            }
        }

        // Determina el tiempo de ejecución simulado basado en la prioridad de la tarea
        private int GetExecutionTime(string priority)
        {
            switch (priority?.ToLower())
            {
                case "alta":
                    return 2000;
                case "media":
                    return 3000;
                case "baja":
                    return 5000;
                default:
                    return 3000;
            }
        }
    }
} 