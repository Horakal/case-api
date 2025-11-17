using RestApiCase.Domain.Tasks.Commands;
using System.ComponentModel.DataAnnotations;

namespace RestApiCase.Application.Tasks.DTOS.Requests
{
    public class CreateTask : ICommand
    {
        public Guid UserId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; }

        public string Summary { get; set; } = "";
        public DateTime? DueDate { get; set; }

        public CreateTask()
        {
        }

        public CreateTask(Guid userId, string title, string description, string summary, DateTime? dueDate)
        {
            UserId = userId;
            Title = title;
            Description = description;
            Summary = summary;
            DueDate = dueDate;
        }
    }
}