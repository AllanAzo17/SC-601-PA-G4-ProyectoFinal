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
            return View(task.ToList());
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
            ViewBag.CreatedBy = new SelectList(new List<string> { "UserId", "Username" });
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TaskId,Title,Description,Priority,ScheduledDate,Status,CreatedAt,CreatedBy")] Task task)
        {
            if (ModelState.IsValid)
            {
                taskBusiness.Save(0, task);
                
                taskBusiness.EnqueueTask(task.TaskId);
                
                return RedirectToAction("Index");
            }

            ViewBag.CreatedBy = new SelectList(new List<string> { "UserId", "Username" }, task.CreatedBy);
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
            ViewBag.CreatedBy = new SelectList(new List<string> { "UserId", "Username" }, task.CreatedBy);
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaskId,Title,Description,Priority,ScheduledDate,Status,CreatedAt,CreatedBy")] Task task)
        {
            if (ModelState.IsValid)
            {
                var current = taskBusiness.Get(task.TaskId).FirstOrDefault();
                if (current == null) return HttpNotFound();

                task.CreatedAt = current.CreatedAt;

                taskBusiness.Save(task.TaskId, task);
                return RedirectToAction("Index");
            }
            ViewBag.CreatedBy = new SelectList(new List<string> { "UserId", "Username" }, task.CreatedBy);
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

            if (task.Status == "EnProceso")
            {
                TempData["ErrorMessage"] = "No se puede eliminar una tarea que está en proceso.";
                return RedirectToAction("Index");
            }

            taskBusiness.Delete(id);

            return RedirectToAction("Index");
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
                taskBusiness.RetryFailedTask(id);
                return Json(new { success = true, message = "Tarea agregada a la cola para reintento" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
