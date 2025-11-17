using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Domain.Tasks.Enums
{
    public enum TaskItemStatus
    {
        [Description("Pending")]
        Pending = 0,

        [Description("Completed")]
        Completed = 2,
    }
}