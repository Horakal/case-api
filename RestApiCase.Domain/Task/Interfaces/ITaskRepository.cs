﻿using RestApiCase.Domain.Tasks.Entities;

namespace RestApiCase.Domain.Tasks.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem> CreateTaskAsync(TaskItem task);

        Task<TaskItem?> GetTaskByIdAsync(Guid id);

        Task<bool> UpdateTaskAsync(TaskItem task);

        Task<bool> DeleteTaskAsync(Guid id);

        Task<IEnumerable<TaskItem>> GetAllTasksAsync(Guid id);
    }
}