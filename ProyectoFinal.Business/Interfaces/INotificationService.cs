using ProyectoFinal.Data;
using System;

namespace ProyectoFinal.Business.Interfaces
{
    public interface INotificationService
    {
        void SendTaskCompletedNotification(Task task, TaskLog log);
        void SendTaskFailedNotification(Task task, TaskLog log);
        void SendTaskStartedNotification(Task task);
        void LogTaskExecution(Task task, bool success, string message, string details);
    }
} 