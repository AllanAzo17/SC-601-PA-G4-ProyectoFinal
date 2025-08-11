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

        // Obtiene una tarea específica o todas las tareas según el ID proporcionado
        public IEnumerable<TaskEntity> Get(int? id)
        {
            var task = new List<TaskEntity>();
            if (id != null)
                task.Add(repositoryTask.GetById((int)id));
            else
                task.AddRange(repositoryTask.GetAll());

            return task;
        }

        // Obtiene todas las tareas del repositorio
        public IEnumerable<TaskEntity> GetAllTasks()
        {
            return repositoryTask.GetAll();
        }

        // Obtiene una tarea específica por su ID
        public TaskEntity GetTaskById(int id)
        {
            return repositoryTask.GetById(id);
        }

        // Crea una nueva tarea con estado "Pendiente" y fecha de creación actual
        public int CreateTask(TaskEntity entity)
        {
            entity.Status = "Pendiente";
            entity.CreatedAt = DateTime.Now;
            repositoryTask.Add(entity);
            return entity.TaskId;
        }

        // Guarda o actualiza una tarea según el ID, evitando modificar tareas en proceso o finalizadas
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

        // Actualiza una tarea existente solo si no está en proceso o finalizada
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

        // Elimina una tarea solo si no está en proceso o finalizada
        public void DeleteTask(int id)
        {
            var task = repositoryTask.GetById(id);
                            if (task != null && task.Status != "En Proceso" && task.Status != "Finalizada")
            {
                repositoryTask.Delete(id);
            }
        }

        // Alias para DeleteTask
        public void Delete(int id)
        {
            DeleteTask(id);
        }

        // Filtra tareas por estado específico
        public IEnumerable<TaskEntity> GetTasksByStatus(string status)
        {
            return repositoryTask.GetAll().Where(t => t.Status == status);
        }

        // Filtra tareas por prioridad específica
        public IEnumerable<TaskEntity> GetTasksByPriority(string priority)
        {
            return repositoryTask.GetAll().Where(t => t.Priority == priority);
        }

        // Agrega una tarea pendiente a la cola de procesamiento evitando duplicados
        public void EnqueueTask(int taskId)
        {
            var task = repositoryTask.GetById(taskId);
            if (task != null && task.Status == "Pendiente")
            {
                taskQueueRepository.AddToQueue(taskId);
            }
        }

        // Genera un resumen estadístico de todas las tareas por estado
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
