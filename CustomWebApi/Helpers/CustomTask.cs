using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Scheduler;
using CMS.EventLog;

namespace CustomWebApi.Helpers
{
    public class CustomTask : ITask
    {

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <param name="ti">Info object representing the scheduled task</param>
        public string Execute(TaskInfo ti)
        {
            string details = "Custom scheduled task executed. Task data: " + ti.TaskData;

            // Logs the execution of the task in the event log
            EventLogProvider.LogInformation("CustomTask", "Execute", details);

            // Returns a null value to indicate that the task executed successfully
            // Return an error message string with details in cases where the execution fails
            return null;
        }
    }
}
