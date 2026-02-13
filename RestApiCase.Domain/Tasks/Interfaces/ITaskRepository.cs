using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Enums;

namespace RestApiCase.Domain.Tasks.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem> CreateTaskAsync(TaskItem task);

        Task<TaskItem?> GetTaskByIdAsync(Guid id, bool isSuperUser, Guid userId);

        Task UpdateTaskAsync(TaskItem task);

        Task DeleteTaskAsync(Guid id);

        Task<IReadOnlyList<TaskItem>> GetAllTasksAsync(Guid id, bool isSuperUser, TaskItemStatus? status = null);
    }
}