using ProyectoFinal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProyectoFinal.MVC.Controllers
{
    public class TaskController : Controller
    {
        private static List<TaskModel> tasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Status = "Pending", Description = "First Task", ExecutionDate = "2025-07-15", Package = "Basic", QueueId = 1 },
            new TaskModel { Id = 2, Status = "Completed", Description = "Second Task", ExecutionDate = "2025-07-12", Package = "Advanced", QueueId = 2 }
        };

        // GET: Task
        public ActionResult Index()
        {
            return View(tasks);
        }

        public ActionResult Details(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            return View(task);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(TaskModel model)
        {
            model.Id = tasks.Max(t => t.Id) + 1;
            tasks.Add(model);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            return View(task);
        }

        [HttpPost]
        public ActionResult Edit(TaskModel model)
        {
            var task = tasks.FirstOrDefault(t => t.Id == model.Id);
            if (task != null)
            {
                task.Status = model.Status;
                task.Description = model.Description;
                task.ExecutionDate = model.ExecutionDate;
                task.Package = model.Package;
                task.QueueId = model.QueueId;
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            tasks.Remove(task);
            return RedirectToAction("Index");
        }
    }
}
