using System;

namespace ProyectoFinal.Data
{
    public class TaskConfiguration
    {
        public int ConfigurationId { get; set; }
        public string TaskType { get; set; }
        public string AssemblyPath { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string Parameters { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 