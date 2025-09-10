using RestApiCase.Domain.Tasks.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestApiCase.Domain.Tasks.Interfaces
{
    public interface ITaskService
    {
        Task<TaskItem> CreateTaskAsync(Guid userId, string title, string description, string? summary, DateTime? update);

        Task<TaskItem?> GetTaskByIdAsync(string id);

        Task<bool> UpdateTaskAsync(string id, string? title, string? description, TaskStatus? status, DateTime? dueDate);

        Task<bool> DeleteTaskAsync(string id);

        Task<IEnumerable<TaskItem>> GetAllTasksAsync(string userId);
    }
}