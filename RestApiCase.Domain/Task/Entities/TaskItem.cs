using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestApiCase.Domain.Task.Enums;

namespace RestApiCase.Domain.Tasks.Entities
{
    public class TaskItem
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string Title { get; init; }

        public string Summary { get; init; }

        public string Description { get; init; }

        public TaskItemStatus.Status Status { get; init; } = TaskItemStatus.Status.Pending;
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }

        public TaskItem()
        { }

        public TaskItem(Guid userId, string title, string description, string? summary, DateTime? update)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Title = title;
            Description = description;
            Summary = summary ?? "";
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = update != null ? ((DateTime)update).ToUniversalTime() : DateTime.UtcNow;
        }
    }
}