using ProyectoFinal.Data;
using ProyectoFinal.Business.Models;
using System;
using System.Collections.Generic;
using TaskEntity = ProyectoFinal.Data.Task;

namespace ProyectoFinal.Business.Interfaces
{
    public interface ITaskService
    {
        IEnumerable<TaskEntity> GetAllTasks();
        TaskEntity GetTaskById(int id);
        int CreateTask(TaskEntity task);
        void UpdateTask(TaskEntity task);
        void DeleteTask(int id);
        IEnumerable<TaskEntity> GetTasksByStatus(string status);
        IEnumerable<TaskEntity> GetTasksByPriority(string priority);
        void EnqueueTask(int taskId);
        void RetryFailedTask(int taskId);
        TaskDashboardSummary GetTaskSummary();
    }
} 