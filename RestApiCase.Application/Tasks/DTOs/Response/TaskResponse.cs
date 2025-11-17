using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Application.Tasks.DTOS.ResponseDTO
{
    public class TaskResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string DueDate { get; set; }
        public string UpdatedAt { get; set; }
        public string Status { get; set; }

        public TaskResponse(TaskItem task)
        {
            Id = task.Id.ToString();
            Title = task.Title;
            Description = task.Description;
            Summary = task.Summary;
            DueDate = task.DueDate.GetValueOrDefault().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            UpdatedAt = task.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            Status = GetDescription(task.Status);
        }

        public static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
    }
}