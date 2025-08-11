using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ProyectoFinal.Business;
using ProyectoFinal.Data;

namespace ProyectoFinal.MVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly TaskBusiness taskBusiness;

        public DashboardController()
        {
            taskBusiness = new TaskBusiness();
        }

        // GET: Dashboard
        public ActionResult Index()
        {
            var summary = taskBusiness.GetTaskSummary();
            var recentTasks = taskBusiness.GetAllTasks().OrderByDescending(t => t.CreatedAt).Take(10);
            var pendingTasks = taskBusiness.GetTasksByStatus("Pendiente").OrderByDescending(t => t.Priority).ThenBy(t => t.ScheduledDate);
            var failedTasks = taskBusiness.GetTasksByStatus("Fallida").OrderByDescending(t => t.CreatedAt);

            ViewBag.Summary = summary;
            ViewBag.RecentTasks = recentTasks;
            ViewBag.PendingTasks = pendingTasks;
            ViewBag.FailedTasks = failedTasks;

            return View();
        }

        // GET: Dashboard/Summary
        public ActionResult Summary()
        {
            var summary = taskBusiness.GetTaskSummary();
            return Json(summary, JsonRequestBehavior.AllowGet);
        }

        // GET: Dashboard/TasksByStatus
        public ActionResult TasksByStatus()
        {
            var tasksByStatus = new
            {
                Pendiente = taskBusiness.GetTasksByStatus("Pendiente").Count(),
                EnProceso = taskBusiness.GetTasksByStatus("En Proceso").Count(),
                Finalizada = taskBusiness.GetTasksByStatus("Finalizada").Count(),
                Fallida = taskBusiness.GetTasksByStatus("Fallida").Count()
            };

            return Json(tasksByStatus, JsonRequestBehavior.AllowGet);
        }

        // GET: Dashboard/TasksByPriority
        public ActionResult TasksByPriority()
        {
            var tasksByPriority = new
            {
                Alta = taskBusiness.GetTasksByPriority("Alta").Count(),
                Media = taskBusiness.GetTasksByPriority("Media").Count(),
                Baja = taskBusiness.GetTasksByPriority("Baja").Count()
            };

            return Json(tasksByPriority, JsonRequestBehavior.AllowGet);
        }
    }
} 