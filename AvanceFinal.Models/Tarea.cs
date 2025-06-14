using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvanceFinal.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public String Package { get; set; }
        public String DateDeliver { get; set; }
        public int Priority { get; set; }
        public String Status { get; set; }
    }
}
