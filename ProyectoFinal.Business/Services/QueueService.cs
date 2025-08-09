using ProyectoFinal.Data;
using ProyectoFinal.Business.Interfaces;
using ProyectoFinal.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskEntity = ProyectoFinal.Data.Task;

namespace ProyectoFinal.Business.Services
{
    public class QueueService : IQueueService
    {
        private readonly IRepositoryTaskQueue _taskQueueRepository;
        private readonly IRepositoryTask _taskRepository;
        private readonly TaskExecutionService _taskExecutionService;
        private readonly INotificationService _notificationService;
        private CancellationTokenSource _cancellationTokenSource;
        private System.Threading.Tasks.Task _queueProcessorTask;
        private bool _isRunning = false;
        private static readonly object _lock = new object();

        // Constructor que inicializa las dependencias necesarias para la gestión de la cola
        public QueueService(
            IRepositoryTaskQueue taskQueueRepository,
            IRepositoryTask taskRepository,
            TaskExecutionService taskExecutionService,
            INotificationService notificationService)
        {
            _taskQueueRepository = taskQueueRepository;
            _taskRepository = taskRepository;
            _taskExecutionService = taskExecutionService;
            _notificationService = notificationService;
        }

        // Obtiene el estado actual de todas las tareas pendientes en la cola
        public IEnumerable<TaskQueue> GetQueueStatus()
        {
            try
            {
                return _taskQueueRepository.GetPendingTasks();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo estado de cola: {ex.Message}");
                return new List<TaskQueue>();
            }
        }

        // Obtiene el historial de las últimas 100 ejecuciones de tareas
        public IEnumerable<TaskLog> GetExecutionHistory()
        {
            try
            {
                var logRepository = new RepositoryTaskLog();
                return logRepository.GetRecentLogs(100);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo historial: {ex.Message}");
                return new List<TaskLog>();
            }
        }

        // Procesa la siguiente tarea de la cola: obtiene, ejecuta, actualiza estado y notifica
        public void ProcessNextTask()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Procesando siguiente tarea...");
                
                var nextTask = _taskQueueRepository.GetNextTaskToProcess();
                if (nextTask != null)
                {
                    var task = _taskRepository.GetById(nextTask.TaskId);
                    if (task != null && task.Status == "Pendiente")
                    {
                        System.Diagnostics.Debug.WriteLine($"Procesando tarea: {task.Title} (ID: {task.TaskId})");
                        
                        try
                        {
                            _taskRepository.UpdateTaskStatus(task.TaskId, "EnProceso");
                            System.Diagnostics.Debug.WriteLine($"Tarea {task.TaskId} marcada como EnProceso");

                            _notificationService.SendTaskStartedNotification(task);

                            bool success = _taskExecutionService.ExecuteTask(task);
                            System.Diagnostics.Debug.WriteLine($"Ejecución completada. Resultado: {(success ? "Exitoso" : "Fallido")}");

                            string finalStatus = success ? "Finalizada" : "Fallida";
                            _taskRepository.UpdateTaskStatus(task.TaskId, finalStatus);
                            System.Diagnostics.Debug.WriteLine($"Tarea {task.TaskId} marcada como {finalStatus}");

                            _taskQueueRepository.RemoveFromQueue(task.TaskId);
                            System.Diagnostics.Debug.WriteLine($"Tarea {task.TaskId} removida de la cola");
                            
                            System.Diagnostics.Debug.WriteLine($"Tarea {task.Title} procesada completamente. Resultado: {(success ? "Exitoso" : "Fallido")}");
                        }
                        catch (Exception updateEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error actualizando tarea {task.TaskId}: {updateEx.Message}");
                            System.Diagnostics.Debug.WriteLine($"Inner Exception: {updateEx.InnerException?.Message}");
                            
                            try
                            {
                                _taskRepository.UpdateTaskStatus(task.TaskId, "Fallida");
                                _taskQueueRepository.RemoveFromQueue(task.TaskId);
                            }
                            catch (Exception fallbackEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error en fallback para tarea {task.TaskId}: {fallbackEx.Message}");
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Tarea no encontrada o no está pendiente");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No hay tareas pendientes en la cola");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error procesando tarea: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        // Inicia el procesador automático de tareas en segundo plano
        public void StartQueueProcessor()
        {
            lock (_lock)
            {
                if (!_isRunning)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Iniciando procesador de cola...");
                        _cancellationTokenSource = new CancellationTokenSource();
                        _queueProcessorTask = System.Threading.Tasks.Task.Run(() => ProcessQueueLoop(_cancellationTokenSource.Token));
                        _isRunning = true;
                        System.Diagnostics.Debug.WriteLine("Procesador de cola iniciado correctamente");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error iniciando procesador: {ex.Message}");
                        _isRunning = false;
                        throw;
                    }
                }
            }
        }

        // Detiene el procesador automático de tareas con un timeout de 5 segundos
        public void StopQueueProcessor()
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Deteniendo procesador de cola...");
                        _cancellationTokenSource?.Cancel();
                        _queueProcessorTask?.Wait(5000);
                        _isRunning = false;
                        System.Diagnostics.Debug.WriteLine("Procesador de cola detenido");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error deteniendo procesador: {ex.Message}");
                        _isRunning = false;
                    }
                }
            }
        }

        // Retorna el estado actual del procesador automático
        public bool IsQueueProcessorRunning()
        {
            return _isRunning;
        }

        // Bucle infinito que procesa tareas cada 30 segundos hasta ser cancelado
        private async void ProcessQueueLoop(CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine("Bucle de procesamiento iniciado");
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ProcessNextTask();
                    
                    await System.Threading.Tasks.Task.Delay(30000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("Procesador cancelado");
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error en el procesador de cola: {ex.Message}");
                    try
                    {
                        await System.Threading.Tasks.Task.Delay(5000, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
            
            System.Diagnostics.Debug.WriteLine("Bucle de procesamiento terminado");
        }
    }
} 