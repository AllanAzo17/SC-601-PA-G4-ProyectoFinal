using ProyectoFinal.Data;
using ProyectoFinal.Business.Interfaces;
using System;
using System.Net.Mail;
using System.Text;
using TaskEntity = ProyectoFinal.Data.Task;
using ProyectoFinal.Repository;

namespace ProyectoFinal.Business.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepositoryTaskLog _taskLogRepository;

        // Constructor que inicializa el repositorio de logs para el registro de eventos
        public NotificationService(IRepositoryTaskLog taskLogRepository)
        {
            _taskLogRepository = taskLogRepository;
        }

        // Crea notificación de éxito y registra en logs la finalización exitosa de una tarea
        public void SendTaskCompletedNotification(TaskEntity task, TaskLog log)
        {
            var notification = new Notification
            {
                TaskId = task.TaskId,
                SentAt = DateTime.Now,
                MessageType = "Email",
                MessageContent = $"Tarea completada exitosamente: {task.Title}",
                Recipient = "admin@example.com"
            };

            LogTaskExecution(task, true, "Tarea completada", $"Tarea {task.Title} completada exitosamente");
        }

        // Crea notificación de error y registra en logs el fallo de una tarea
        public void SendTaskFailedNotification(TaskEntity task, TaskLog log)
        {
            var notification = new Notification
            {
                TaskId = task.TaskId,
                SentAt = DateTime.Now,
                MessageType = "Email",
                MessageContent = $"Tarea falló: {task.Title}. Error: {log.ErrorMessage}",
                Recipient = "admin@example.com"
            };

            LogTaskExecution(task, false, "Tarea falló", $"Tarea {task.Title} falló: {log.ErrorMessage}");
        }

        // Crea notificación informativa y registra en logs el inicio de una tarea
        public void SendTaskStartedNotification(TaskEntity task)
        {
            var notification = new Notification
            {
                TaskId = task.TaskId,
                SentAt = DateTime.Now,
                MessageType = "Info",
                MessageContent = $"Tarea iniciada: {task.Title}",
                Recipient = "admin@example.com"
            };

            LogTaskExecution(task, true, "Tarea iniciada", $"Tarea {task.Title} iniciada");
        }

        // Registra un evento de ejecución en la base de datos con estado y detalles
        public void LogTaskExecution(TaskEntity task, bool success, string message, string details)
        {
            var log = new TaskLog
            {
                TaskId = task.TaskId,
                ExecutionStart = DateTime.Now,
                ExecutionEnd = DateTime.Now,
                Success = success,
                LogDetails = details
            };

            _taskLogRepository.AddLog(log);
        }

        // Método privado para envío real de emails (actualmente comentado para desarrollo)
        private void SendEmail(int userId, string message)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("noreply@empresa.com"),
                        Subject = "Notificación de Tarea",
                        Body = message,
                        IsBodyHtml = false
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error enviando email: {ex.Message}");
            }
        }
    }
} 