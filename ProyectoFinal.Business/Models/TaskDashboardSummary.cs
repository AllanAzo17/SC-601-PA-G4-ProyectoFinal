using System;

namespace ProyectoFinal.Business.Models
{
    public class TaskDashboardSummary
    {
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int FailedTasks { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }
}
