using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestApiCase.Domain.Tasks.Enums;

namespace RestApiCase.Domain.Tasks.Entities
{
    public class ValidationError
    {
        public string FieldName { get; private set; }
        public string ErrorMessage { get; private set; }

        internal ValidationError(string errorMessage, string fieldName)
        {
            FieldName = fieldName;
            ErrorMessage = errorMessage;
        }
    }

    public class DomainValidationException : Exception
    {
        public IReadOnlyList<ValidationError> Erros { get; }

        public DomainValidationException(IReadOnlyList<ValidationError> erros)
            : base("Falhas de validação de domínio detectadas.")
        {
            Erros = erros ?? throw new ArgumentNullException(nameof(erros));
        }
    }

    public class TaskItem
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Title { get; private set; }

        public string Summary { get; private set; }

        public string Description { get; private set; }

        public TaskItemStatus Status { get; private set; } = TaskItemStatus.Pending;
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DateTime? DueDate { get; private set; }

        private TaskItem()
        { }

        public TaskItem(Guid userId, string title, string description, string? summary, DateTime? dueDate)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Title = title;
            Description = description;
            Summary = summary ?? "";
            CreatedAt = DateTime.UtcNow;
            DueDate = dueDate.GetValueOrDefault().ToUniversalTime();
            UpdatedAt = DateTime.UtcNow;
            Status = TaskItemStatus.Pending;

            ValidateCreateTaskItem();
        }

        private void ValidateCreateTaskItem()
        {
            var erros = new List<ValidationError>();
            if (Title.Length > 100)
            {
                erros.Add(new ValidationError("Title cannot exceed 100 characters", "Title"));
            }
            if (string.IsNullOrEmpty(Description))
            {
                erros.Add(new ValidationError("Description is required", "Desc"));
            }
            else if (Description.Length < 20 || Description.Length > 500)
            {
                erros.Add(new ValidationError("Description must be between 20 and 500 characters", "Desc"));
            }
            if (!string.IsNullOrEmpty(Summary) && Summary.Length > 200)
            {
                erros.Add(new ValidationError("Summary cannot exceed 200 characters", "Summary"));
            }
            if (DueDate != null && DueDate != DateTime.MinValue.ToUniversalTime() && DueDate <= DateTime.Now)
            {
                erros.Add(new ValidationError("Due date must be in the future", nameof(DueDate)));
            }

            if (erros.Count != 0)
            {
                throw new DomainValidationException(erros);
            }
        }

        public void UpdateTitle(string title)
        {
            Title = string.IsNullOrEmpty(title) ? Title : title;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }

        public void UpdateSummary(string summary)
        {
            Summary = summary;
        }

        public void UpdateStatus(TaskItemStatus status)
        {
            Status = status;
        }

        public void UpdateDueDate(DateTime dueDate)
        {
            if (dueDate <= DateTime.UtcNow)
            {
                throw new ArgumentException("Due date must be in the future", nameof(dueDate));
            }
            DueDate = dueDate;
        }

        private void PartialUpdateValidation(string? title, string? description, string? summary, DateTime? dueDate)
        {
            var errors = new List<ValidationError>();
            if (title != null && title.Length > 100)
            {
                errors.Add(new ValidationError("Title cannot exceed 100 characters", "Title"));
            }
            if (errors.Count != 0)
            {
                throw new DomainValidationException(errors);
            }
        }
    }
}