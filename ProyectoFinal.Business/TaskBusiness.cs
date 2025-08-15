using ProyectoFinal.Data;
using ProyectoFinal.Repository;
using ProyectoFinal.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskEntity = ProyectoFinal.Data.Task;

namespace ProyectoFinal.Business
{
    public class TaskBusiness
    {
        private readonly IRepositoryTask repositoryTask;
        private readonly IRepositoryTaskQueue taskQueueRepository;

        public TaskBusiness()
        {
            repositoryTask = new RepositoryTask();
            taskQueueRepository = new RepositoryTaskQueue();
        }

        public IEnumerable<TaskEntity> Get(int? id)
        {
            var task = new List<TaskEntity>();
            if (id != null)
                task.Add(repositoryTask.GetById((int)id));
            else
                task.AddRange(repositoryTask.GetAll());

            return task;
        }

        public IEnumerable<TaskEntity> GetAllTasks()
        {
            return repositoryTask.GetAll();
        }

        public TaskEntity GetTaskById(int id)
        {
            return repositoryTask.GetById(id);
        }

        public int CreateTask(TaskEntity entity)
        {
            entity.Status = "Pendiente";
            entity.CreatedAt = DateTime.Now;
            repositoryTask.Add(entity);
            return entity.TaskId;
        }

        public int Save(int id, TaskEntity entity)
        {
            if (id <= 0)
            {
                return CreateTask(entity);
            }
            else
            {
                var exist = repositoryTask.GetById(id);
                if (exist != null)
                {
                    if (exist.Status != "En Proceso" && exist.Status != "Finalizada")
                    {
                        exist.Title = entity.Title;
                        exist.Description = entity.Description;
                        exist.Priority = entity.Priority;
                        exist.ScheduledDate = entity.ScheduledDate;
                        exist.CreatedBy = entity.CreatedBy;
                        repositoryTask.Update(exist);
                    }
                }
                return id;
            }
        }

        public void UpdateTask(TaskEntity entity)
        {
            var exist = repositoryTask.GetById(entity.TaskId);
                            if (exist != null && exist.Status != "En Proceso" && exist.Status != "Finalizada")
            {
                exist.Title = entity.Title;
                exist.Description = entity.Description;
                exist.Priority = entity.Priority;
                exist.ScheduledDate = entity.ScheduledDate;
                exist.CreatedBy = entity.CreatedBy;
                repositoryTask.Update(exist);
            }
        }

        public void DeleteTask(int id)
        {
            var task = repositoryTask.GetById(id);
                            if (task != null && task.Status != "En Proceso" && task.Status != "Finalizada")
            {
                repositoryTask.Delete(id);
            }
        }

        public void Delete(int id)
        {
            DeleteTask(id);
        }

        public IEnumerable<TaskEntity> GetTasksByStatus(string status)
        {
            return repositoryTask.GetAll().Where(t => t.Status == status);
        }

        public IEnumerable<TaskEntity> GetTasksByPriority(string priority)
        {
            return repositoryTask.GetAll().Where(t => t.Priority == priority);
        }

        public void EnqueueTask(int taskId)
        {
            var task = repositoryTask.GetById(taskId);
            if (task != null && task.Status == "Pendiente")
            {
                taskQueueRepository.AddToQueue(taskId);
            }
        }

        public TaskDashboardSummary GetTaskSummary()
        {
            var allTasks = repositoryTask.GetAll();
            return new TaskDashboardSummary
            {
                TotalTasks = allTasks.Count(),
                PendingTasks = allTasks.Count(t => t.Status == "Pendiente"),
                InProgressTasks = allTasks.Count(t => t.Status == "En Proceso"),
                CompletedTasks = allTasks.Count(t => t.Status == "Finalizada"),
                FailedTasks = allTasks.Count(t => t.Status == "Fallida")
            };
        }
    }
}
