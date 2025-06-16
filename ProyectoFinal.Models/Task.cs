using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal.Models
{
    public class Task
    {
        public int Id { get; set; }
        public String Status { get; set; }
        public string Description { get; set; }
        public String ExecutionDate { get; set; }
        public String Package { get; set; }
        public int QueueId { get; set; }
    }
}
