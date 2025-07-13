using ProyectoFinal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProyectoFinal.MVC.Controllers
{
    public class QueueController : Controller
    {
        private static List<Queue> queues = new List<Queue>
        {
            new Queue { Id = 1, Priority = 1 },
            new Queue { Id = 2, Priority = 2 }
        };

        public ActionResult Index()
        {
            return View(queues);
        }

        public ActionResult Details(int id)
        {
            var queue = queues.FirstOrDefault(q => q.Id == id);
            return View(queue);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Queue model)
        {
            model.Id = queues.Max(q => q.Id) + 1;
            queues.Add(model);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var queue = queues.FirstOrDefault(q => q.Id == id);
            return View(queue);
        }

        [HttpPost]
        public ActionResult Edit(Queue model)
        {
            var queue = queues.FirstOrDefault(q => q.Id == model.Id);
            if (queue != null)
            {
                queue.Priority = model.Priority;
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var queue = queues.FirstOrDefault(q => q.Id == id);
            return View(queue);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var queue = queues.FirstOrDefault(q => q.Id == id);
            queues.Remove(queue);
            return RedirectToAction("Index");
        }
    }
}
