using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ProyectoFinal.Business.Services;
using ProyectoFinal.Repository;

namespace ProyectoFinal.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static QueueService _queueService;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Inicializar el procesador de tareas
            InitializeTaskProcessor();
        }

        private void InitializeTaskProcessor()
        {
            try
            {
                // Crear instancias de los servicios necesarios
                var taskQueueRepository = new RepositoryTaskQueue();
                var taskRepository = new RepositoryTask();
                var taskExecutionService = new TaskExecutionService(
                    new NotificationService(new RepositoryTaskLog()),
                    new RepositoryTaskLog());
                var notificationService = new NotificationService(new RepositoryTaskLog());

                // Crear el servicio de cola
                _queueService = new QueueService(
                    taskQueueRepository,
                    taskRepository,
                    taskExecutionService,
                    notificationService);

                // Iniciar el procesador automáticamente
                _queueService.StartQueueProcessor();
                
                System.Diagnostics.Debug.WriteLine("Procesador de tareas inicializado automáticamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inicializando procesador de tareas: {ex.Message}");
            }
        }

        protected void Application_End()
        {
            // Detener el procesador cuando la aplicación se cierre
            try
            {
                _queueService?.StopQueueProcessor();
                System.Diagnostics.Debug.WriteLine("Procesador de tareas detenido");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deteniendo procesador de tareas: {ex.Message}");
            }
        }
    }
}
