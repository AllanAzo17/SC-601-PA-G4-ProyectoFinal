using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoFinal.Business;
using ProyectoFinal.Data;

namespace ProyectoFinal.MVC.Controllers
{
    public class TasksController : Controller
    {
        private readonly TaskBusiness taskBusiness;

        public TasksController()
        {
            taskBusiness = new TaskBusiness();
        }

        // GET: Tasks
        public ActionResult Index()
        {
            var task = taskBusiness.Get(id: null);
            // Ordenar tareas: primero las Pendientes, luego por fecha de creación (más recientes primero)
            var orderedTasks = task.OrderByDescending(t => t.Status == "Pendiente")
                                   .ThenByDescending(t => t.CreatedAt)
                                   .ToList();
            return View(orderedTasks);
        }

        // GET: Tasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = taskBusiness.Get(id).First();
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // GET: Tasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TaskId,Title,Description,Priority,ScheduledDate,Status,CreatedAt,CreatedBy")] Task task)
        {
            if (ModelState.IsValid)
            {
                task.Status = "Pendiente";
                task.CreatedBy = 1;
                task.CreatedAt = DateTime.Now;
                
                taskBusiness.Save(0, task);
                
                return RedirectToAction("Index");
            }

            return View(task);
        }

        // GET: Tasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = taskBusiness.Get(id).FirstOrDefault();
            if (task == null)
            {
                return HttpNotFound();
            }

            if (task.User != null)
            {
                ViewBag.CreatedByUsername = task.User.Username;
            }
            else
            {
                ViewBag.CreatedByUsername = "Usuario no disponible";
            }

            return View(task);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaskId,Title,Description,Priority,ScheduledDate,Status,CreatedAt,CreatedBy")] Task task)
        {
            if (ModelState.IsValid)
            {
                var current = taskBusiness.Get(task.TaskId).FirstOrDefault();
                if (current == null) return HttpNotFound();

                task.CreatedAt = current.CreatedAt;
                task.CreatedBy = current.CreatedBy;
                task.Status = current.Status;

                taskBusiness.Save(task.TaskId, task);
                return RedirectToAction("Index");
            }
            return View(task);
        }

        // GET: Tasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = taskBusiness.Get(id).FirstOrDefault();
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var task = taskBusiness.Get(id).FirstOrDefault();
            if (task == null)
                return HttpNotFound();

                            if (task.Status == "En Proceso")
            {
                TempData["ErrorMessage"] = "No se puede eliminar una tarea que está en proceso.";
                return RedirectToAction("Index");
            }

            taskBusiness.Delete(id);

            return RedirectToAction("Index");
        }

        // POST: Tasks/DeleteAjax/5
        [HttpPost]
        public ActionResult DeleteAjax(int id)
        {
            try
            {
                var task = taskBusiness.Get(id).FirstOrDefault();
                if (task == null)
                {
                    return Json(new { success = false, message = "Tarea no encontrada" });
                }

                if (task.Status == "En Proceso")
                {
                    return Json(new { success = false, message = "No se puede eliminar una tarea que está en proceso." });
                }

                taskBusiness.Delete(id);
                return Json(new { success = true, message = "Tarea eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Tasks/Enqueue/5
        [HttpPost]
        public ActionResult Enqueue(int id)
        {
            try
            {
                taskBusiness.EnqueueTask(id);
                return Json(new { success = true, message = "Tarea agregada a la cola" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Tasks/Retry/5
        [HttpPost]
        public ActionResult Retry(int id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"TasksController.Retry: Iniciando reintento para tarea {id}");
                
                var task = taskBusiness.Get(id).FirstOrDefault();
                if (task == null)
                {
                    System.Diagnostics.Debug.WriteLine($"TasksController.Retry: Tarea {id} no encontrada");
                    return Json(new { success = false, message = "Tarea no encontrada" }, JsonRequestBehavior.AllowGet);
                }

                System.Diagnostics.Debug.WriteLine($"TasksController.Retry: Tarea encontrada - ID: {task.TaskId}, Estado actual: {task.Status}");

                // Cambiar estado a "Pendiente" para permitir reintento
                task.Status = "Pendiente";
                taskBusiness.Save(task.TaskId, task);

                System.Diagnostics.Debug.WriteLine($"TasksController.Retry: Estado cambiado a 'Pendiente'");

                // Agregar a la cola para procesamiento
                taskBusiness.EnqueueTask(id);

                System.Diagnostics.Debug.WriteLine($"TasksController.Retry: Tarea agregada a la cola exitosamente");

                return Json(new { success = true, message = "Tarea agregada a la cola para reintento" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TasksController.Retry: Error - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"TasksController.Retry: StackTrace - {ex.StackTrace}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
