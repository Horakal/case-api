using RestApiCase.Domain.Tasks.Commands;
using RestApiCase.Domain.Tasks.Enums;
using System.ComponentModel.DataAnnotations;

namespace RestApiCase.Application.Tasks.DTOS.Requests
{
    public class UpdateTask : ICommand
    {
        public Guid Id { get; set; }
        public string? Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? Summary { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskItemStatus Status { get; set; }

        public UpdateTask()
        {
        }

        public UpdateTask(Guid id, string? title, string? description, string? summary, DateTime? dueDate, TaskItemStatus status)
        {
            Id = id;
            Title = title;
            Description = description;
            Summary = summary;
            DueDate = dueDate;
            Status = status;
        }
    }
}