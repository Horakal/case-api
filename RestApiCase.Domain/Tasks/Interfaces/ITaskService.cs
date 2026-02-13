using RestApiCase.Domain.Tasks.Commands;
using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestApiCase.Domain.Tasks.Interfaces
{
    public interface ITaskService<TResponse> where TResponse : class
    {
        Task<TResponse> ExecuteAsync<TRequest>(TRequest request) where TRequest : ICommand;

        Task<TResponse?> GetTaskByIdAsync(Guid id, Guid userId);

        Task<TResponse> UpdateTaskAsync<TRequest>(TRequest request) where TRequest : class;

        Task DeleteTaskAsync(Guid id);

        Task<IEnumerable<TResponse>> GetAllTasksAsync(Guid userId, TaskItemStatus? status = null);
    }
}