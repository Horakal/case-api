using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Domain.Task.Enums
{
    public class TaskItemStatus
    {
        public enum Status
        {
            Pending = 0,
            Completed = 2,
        }

        public static string GetStatusDescription(Status status)
        {
            return status switch
            {
                Status.Pending => "Task is pending.",
                Status.Completed => "Task has been completed.",
                _ => "Unknown status."
            };
        }
    }
}