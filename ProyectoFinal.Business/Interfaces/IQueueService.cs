using ProyectoFinal.Data;
using System.Collections.Generic;

namespace ProyectoFinal.Business.Interfaces
{
    public interface IQueueService
    {
        IEnumerable<TaskQueue> GetQueueStatus();
        IEnumerable<TaskLog> GetExecutionHistory();
        void ProcessNextTask();
        void StartQueueProcessor();
        void StopQueueProcessor();
        bool IsQueueProcessorRunning();
    }
} 