using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ProyectoFinal.Business;
using ProyectoFinal.Business.Services;
using ProyectoFinal.Data;
using ProyectoFinal.Repository;

namespace ProyectoFinal.MVC.Controllers
{
    public class QueueController : Controller
    {
        private readonly TaskBusiness taskBusiness;
        private readonly QueueService queueService;

        public QueueController()
        {
            taskBusiness = new TaskBusiness();
            var taskQueueRepository = new RepositoryTaskQueue();
            var taskRepository = new RepositoryTask();
            var taskExecutionService = new TaskExecutionService(
                new NotificationService(new RepositoryTaskLog()),
                new RepositoryTaskLog());
            var notificationService = new NotificationService(new RepositoryTaskLog());
            
            queueService = new QueueService(
                taskQueueRepository,
                taskRepository,
                taskExecutionService,
                notificationService);
        }

        // GET: Queue
        public ActionResult Index()
        {
            var queueStatus = queueService.GetQueueStatus();
            var executionHistory = queueService.GetExecutionHistory();
            
            ViewBag.QueueStatus = queueStatus;
            ViewBag.ExecutionHistory = executionHistory;
            ViewBag.IsProcessorRunning = queueService.IsQueueProcessorRunning();
            
            return View();
        }

        // GET: Queue/Status
        public ActionResult Status()
        {
            try
            {
                var queueStatus = queueService.GetQueueStatus();
                var isRunning = queueService.IsQueueProcessorRunning();
                var queueCount = queueStatus.Count();
                
                System.Diagnostics.Debug.WriteLine($"Estado del procesador: {(isRunning ? "Ejecutándose" : "Detenido")}");
                System.Diagnostics.Debug.WriteLine($"Tareas en cola: {queueCount}");
                
                return Json(new { 
                    queueStatus = queueStatus,
                    isProcessorRunning = isRunning,
                    queueCount = queueCount,
                    lastUpdate = DateTime.Now.ToString("HH:mm:ss")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo estado: {ex.Message}");
                return Json(new { 
                    queueStatus = new List<TaskQueue>(),
                    isProcessorRunning = false,
                    queueCount = 0,
                    error = ex.Message,
                    lastUpdate = DateTime.Now.ToString("HH:mm:ss")
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Queue/StartProcessor
        [HttpPost]
        public ActionResult StartProcessor()
        {
            try
            {
                queueService.StartQueueProcessor();
                return Json(new { success = true, message = "Procesador de cola iniciado" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Queue/StopProcessor
        [HttpPost]
        public ActionResult StopProcessor()
        {
            try
            {
                queueService.StopQueueProcessor();
                return Json(new { success = true, message = "Procesador de cola detenido" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Queue/ProcessNext
        [HttpPost]
        public ActionResult ProcessNext()
        {
            try
            {
                queueService.ProcessNextTask();
                return Json(new { success = true, message = "Siguiente tarea procesada" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Queue/History
        public ActionResult History()
        {
            var executionHistory = queueService.GetExecutionHistory();
            return View(executionHistory);
        }

        // GET: Queue/History/5
        public ActionResult TaskHistory(int id)
        {
            var taskLogRepository = new RepositoryTaskLog();
            var logs = taskLogRepository.GetLogsByTaskId(id);
            var task = taskBusiness.GetTaskById(id);
            
            ViewBag.Task = task;
            return View(logs);
        }

        // POST: Queue/Retry/5
        [HttpPost]
        public ActionResult Retry(int id)
        {
            try
            {
                taskBusiness.RetryFailedTask(id);
                return Json(new { success = true, message = "Tarea agregada a la cola para reintento" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Queue/TableData
        public ActionResult TableData()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("TableData: Iniciando obtención de datos...");
                
                var queueStatus = queueService.GetQueueStatus();
                var executionHistory = queueService.GetExecutionHistory();
                
                System.Diagnostics.Debug.WriteLine($"TableData: Tareas en cola: {queueStatus.Count()}");
                System.Diagnostics.Debug.WriteLine($"TableData: Historial de ejecución: {executionHistory.Count()}");
                
                var result = new { 
                    queueStatus = queueStatus.Select(q => new {
                        position = 0,
                        taskId = q.TaskId,
                        title = q.Task.Title,
                        priority = q.Task.Priority,
                        enqueuedAt = q.EnqueuedAt?.ToString("dd/MM/yyyy HH:mm:ss") ?? "N/A"
                    }).ToList(),
                    executionHistory = executionHistory.Take(10).Select(h => new {
                        taskId = h.TaskId,
                        title = h.Task.Title,
                        success = h.Success ?? false,
                        executionStart = h.ExecutionStart?.ToString("dd/MM/yyyy HH:mm:ss") ?? "N/A",
                        executionEnd = h.ExecutionEnd?.ToString("dd/MM/yyyy HH:mm:ss") ?? "N/A"
                    }).ToList(),
                    queueCount = queueStatus.Count(),
                    historyCount = executionHistory.Count()
                };
                
                System.Diagnostics.Debug.WriteLine("TableData: Datos preparados correctamente");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo datos de tablas: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return Json(new { 
                    queueStatus = new List<object>(),
                    executionHistory = new List<object>(),
                    queueCount = 0,
                    historyCount = 0,
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
} 