using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProyectoFinal.Data;
using ProyectoFinal.Repository;

namespace ProyectoFinal.MVC.Controllers
{
    public class QueueController : Controller
    {
        private readonly IRepositoryTaskQueue taskQueueRepository;
        private readonly RepositoryTaskLog taskLogRepository;
        private readonly IRepositoryTask taskRepository;
        private const string PROCESSOR_STATUS_KEY = "QueueProcessorRunning";

        public QueueController()
        {
            taskQueueRepository = new RepositoryTaskQueue();
            taskLogRepository = new RepositoryTaskLog();
            taskRepository = new RepositoryTask();
        }

        private bool IsProcessorRunning
        {
            get
            {
                var status = HttpContext.Application[PROCESSOR_STATUS_KEY];
                return status != null && (bool)status;
            }
            set
            {
                HttpContext.Application[PROCESSOR_STATUS_KEY] = value;
                System.Diagnostics.Debug.WriteLine($"IsProcessorRunning set to: {value}");
            }
        }

        // GET: Queue
        public ActionResult Index()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Index: Iniciando carga de datos...");
                var queueStatus = taskQueueRepository.GetPendingTasks();
                var executionHistory = taskLogRepository.GetRecentLogs(10);
                
                System.Diagnostics.Debug.WriteLine($"Index: Tareas en cola: {queueStatus.Count()}");
                System.Diagnostics.Debug.WriteLine($"Index: Logs de ejecución: {executionHistory.Count()}");
                System.Diagnostics.Debug.WriteLine($"Index: Estado del procesador: {IsProcessorRunning}");
                
                ViewBag.QueueStatus = queueStatus;
                ViewBag.ExecutionHistory = executionHistory;
                ViewBag.IsProcessorRunning = IsProcessorRunning;
                
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Index: Error: {ex.Message}");
                ViewBag.QueueStatus = new List<TaskQueue>();
                ViewBag.ExecutionHistory = new List<TaskLog>();
                ViewBag.IsProcessorRunning = false;
                
                return View();
            }
        }

        // GET: Queue/Status
        public ActionResult Status()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Status: Estado actual del procesador: {IsProcessorRunning}");
                var queueStatus = taskQueueRepository.GetPendingTasks();
                var queueCount = queueStatus.Count();
                
                System.Diagnostics.Debug.WriteLine($"Status: Tareas en cola: {queueCount}");
                System.Diagnostics.Debug.WriteLine($"Status: Hash del controlador: {this.GetHashCode()}");
                
                return Json(new { 
                    isProcessorRunning = IsProcessorRunning,
                    queueCount = queueCount,
                    lastUpdate = DateTime.Now.ToString("HH:mm:ss"),
                    controllerHash = this.GetHashCode()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Status: Error: {ex.Message}");
                return Json(new { 
                    isProcessorRunning = false,
                    queueCount = 0,
                    error = ex.Message,
                    lastUpdate = DateTime.Now.ToString("HH:mm:ss")
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Queue/Start
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Start()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Start: Iniciando procesador automático...");
                IsProcessorRunning = true;
                return Json(new { success = true, message = "Procesador iniciado - procesando tareas automáticamente" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Start: Error al iniciar procesador: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Queue/Stop
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Stop()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Stop: Deteniendo procesador automático...");
                IsProcessorRunning = false;
                return Json(new { success = true, message = "Procesador detenido - procesamiento automático interrumpido" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Stop: Error al detener procesador: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Queue/ProcessNext
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessNext()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ProcessNext: Iniciando procesamiento...");
                
                // Obtener la siguiente tarea a procesar
                var nextTask = taskQueueRepository.GetNextTaskToProcess();
                if (nextTask != null)
                {
                    System.Diagnostics.Debug.WriteLine($"ProcessNext: Tarea encontrada - ID: {nextTask.TaskId}");
                    
                    var task = taskRepository.GetById(nextTask.TaskId);
                    if (task != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"ProcessNext: Tarea cargada - ID: {task.TaskId}, Title: {task.Title}, Status: {task.Status}");
                        
                        try
                        {
                            // Cambiar estado a "En Proceso" al iniciar el procesamiento
                            System.Diagnostics.Debug.WriteLine("ProcessNext: Cambiando estado a 'En Proceso'...");
                            taskRepository.UpdateTaskStatus(task.TaskId, "En Proceso");
                            
                            // Simular ejecución de tarea
                            System.Diagnostics.Debug.WriteLine("ProcessNext: Simulando ejecución...");
                            System.Threading.Thread.Sleep(2000);
                            
                            // Generar aleatoriedad: 70% éxito, 30% fallo
                            var random = new Random();
                            var randomValue = random.Next(1, 101); // 1-100
                            var isSuccess = randomValue <= 70; // 70% de probabilidad de éxito
                            
                            System.Diagnostics.Debug.WriteLine($"ProcessNext: Valor aleatorio: {randomValue}, Éxito: {isSuccess}");
                            
                            if (isSuccess)
                            {
                                // Éxito (70% de probabilidad)
                                System.Diagnostics.Debug.WriteLine("ProcessNext: Procesamiento exitoso");
                                
                                // Crear log de ejecución exitosa
                                var log = new TaskLog
                                {
                                    TaskId = task.TaskId,
                                    ExecutionStart = DateTime.Now.AddSeconds(-2),
                                    ExecutionEnd = DateTime.Now,
                                    Success = true,
                                    ErrorMessage = $"Tarea '{task.Title}' procesada exitosamente"
                                };
                                
                                taskLogRepository.Add(log);
                                
                                // Cambiar estado a "Finalizada"
                                System.Diagnostics.Debug.WriteLine("ProcessNext: Cambiando estado a 'Finalizada'...");
                                taskRepository.UpdateTaskStatus(task.TaskId, "Finalizada");
                            }
                            else
                            {
                                // Fallo (30% de probabilidad)
                                System.Diagnostics.Debug.WriteLine("ProcessNext: Procesamiento fallido");
                                
                                // Crear log de ejecución fallida
                                var log = new TaskLog
                                {
                                    TaskId = task.TaskId,
                                    ExecutionStart = DateTime.Now.AddSeconds(-2),
                                    ExecutionEnd = DateTime.Now,
                                    Success = false,
                                    ErrorMessage = $"Error simulado en el procesamiento de la tarea '{task.Title}'"
                                };
                                
                                taskLogRepository.Add(log);
                                
                                // Cambiar estado a "Fallida"
                                System.Diagnostics.Debug.WriteLine("ProcessNext: Cambiando estado a 'Fallida'...");
                                taskRepository.UpdateTaskStatus(task.TaskId, "Fallida");
                            }
                            
                            // Remover de la cola
                            System.Diagnostics.Debug.WriteLine("ProcessNext: Removiendo de la cola...");
                            taskQueueRepository.RemoveFromQueue(task.TaskId);
                            
                            // Retornar mensaje según el resultado
                            if (isSuccess)
                            {
                                System.Diagnostics.Debug.WriteLine("ProcessNext: Procesamiento completado exitosamente");
                                return Json(new { success = true, message = $"Tarea '{task.Title}' procesada exitosamente" });
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("ProcessNext: Procesamiento completado con fallo");
                                return Json(new { success = true, message = $"Tarea '{task.Title}' falló durante el procesamiento" });
                            }
                        }
                        catch (Exception updateEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"ProcessNext: Error durante el procesamiento: {updateEx.Message}");
                            System.Diagnostics.Debug.WriteLine($"ProcessNext: StackTrace: {updateEx.StackTrace}");
                            
                            // Si hay error al actualizar, intentar revertir el estado
                            try
                            {
                                System.Diagnostics.Debug.WriteLine("ProcessNext: Intentando revertir estado a 'Pendiente'...");
                                taskRepository.UpdateTaskStatus(task.TaskId, "Pendiente");
                            }
                            catch (Exception revertEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"ProcessNext: Error al revertir estado: {revertEx.Message}");
                            }
                            
                            return Json(new { success = false, message = $"Error al procesar la tarea: {updateEx.Message}" });
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ProcessNext: No se pudo cargar la tarea desde el repositorio");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ProcessNext: No hay tareas en la cola para procesar");
                }
                
                return Json(new { success = false, message = "No hay tareas en la cola" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ProcessNext: Error general: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"ProcessNext: StackTrace: {ex.StackTrace}");
                return Json(new { success = false, message = $"Error general: {ex.Message}" });
            }
        }

        // GET: Queue/History
        public ActionResult History()
        {
            var executionHistory = taskLogRepository.GetRecentLogs(50);
            return View(executionHistory);
        }

        // GET: Queue/TaskHistory/5
        public ActionResult TaskHistory(int id)
        {
            var logs = taskLogRepository.GetLogsByTaskId(id);
            var task = taskRepository.GetById(id);
            
            ViewBag.Task = task;
            return View(logs);
        }

        // GET: Queue/TableData
        public ActionResult TableData()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("TableData: Iniciando consulta de datos...");
                
                var queueStatus = taskQueueRepository.GetPendingTasks();
                System.Diagnostics.Debug.WriteLine($"TableData: Tareas en cola encontradas: {queueStatus.Count()}");
                
                foreach (var item in queueStatus)
                {
                    System.Diagnostics.Debug.WriteLine($"  - TaskId: {item.TaskId}, Title: {item.Task?.Title}, Priority: {item.Task?.Priority}");
                }
                
                var executionHistory = taskLogRepository.GetRecentLogs(10);
                System.Diagnostics.Debug.WriteLine($"TableData: Logs de ejecución encontrados: {executionHistory.Count()}");
                
                var result = new { 
                    queueStatus = queueStatus.Select(q => new {
                        taskId = q.TaskId,
                        title = q.Task?.Title ?? "Sin título",
                        priority = q.Task?.Priority ?? "Sin prioridad",
                        priorityOrder = GetPriorityOrder(q.Task?.Priority),
                        createdAt = q.Task?.CreatedAt?.ToString("dd/MM/yyyy HH:mm:ss") ?? "N/A",
                        enqueuedAt = q.EnqueuedAt?.ToString("dd/MM/yyyy HH:mm:ss") ?? "N/A"
                    }).ToList(),
                    executionHistory = executionHistory.Select(h => new {
                        taskId = h.TaskId,
                        title = h.Task?.Title ?? "Sin título",
                        success = h.Success ?? false,
                        executionStart = h.ExecutionStart?.ToString("dd/MM/yyyy HH:mm:ss") ?? "N/A",
                        executionEnd = h.ExecutionEnd?.ToString("dd/MM/yyyy HH:mm:ss") ?? "N/A"
                    }).ToList()
                };
                
                System.Diagnostics.Debug.WriteLine("TableData: Datos preparados exitosamente");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TableData: Error obteniendo datos: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"TableData: StackTrace: {ex.StackTrace}");
                return Json(new { 
                    queueStatus = new List<object>(),
                    executionHistory = new List<object>(),
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }



        // Método auxiliar para ordenar prioridades: Alta=1, Media=2, Baja=3
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

        // POST: Queue/Retry/5
        [HttpPost]
        public ActionResult Retry(int id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"QueueController.Retry: Iniciando reintento para tarea {id}");
                
                var task = taskRepository.GetById(id);
                if (task == null)
                {
                    System.Diagnostics.Debug.WriteLine($"QueueController.Retry: Tarea {id} no encontrada");
                    return Json(new { success = false, message = "Tarea no encontrada" }, JsonRequestBehavior.AllowGet);
                }

                System.Diagnostics.Debug.WriteLine($"QueueController.Retry: Tarea encontrada - ID: {task.TaskId}, Estado actual: {task.Status}");

                // Cambiar estado a "Pendiente" para permitir reintento
                taskRepository.UpdateTaskStatus(id, "Pendiente");

                System.Diagnostics.Debug.WriteLine($"QueueController.Retry: Estado cambiado a 'Pendiente'");

                // Agregar a la cola para procesamiento
                taskQueueRepository.AddToQueue(id);

                System.Diagnostics.Debug.WriteLine($"QueueController.Retry: Tarea agregada a la cola exitosamente");

                return Json(new { success = true, message = "Tarea agregada a la cola para reintento" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"QueueController.Retry: Error - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"QueueController.Retry: StackTrace - {ex.StackTrace}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
} 