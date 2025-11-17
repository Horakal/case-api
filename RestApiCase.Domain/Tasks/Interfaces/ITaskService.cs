using RestApiCase.Domain.Tasks.Commands;
using RestApiCase.Domain.Tasks.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestApiCase.Domain.Tasks.Interfaces
{
    public interface ITaskService<TResponse> where TResponse : class
    {
        Task<TResponse> ExecuteAsync<TRequest>(TRequest request) where TRequest : ICommand;

        Task<TResponse?> GetTaskByIdAsync(string id);

        Task<TResponse> UpdateTaskAsync<TRequest>(TRequest request) where TRequest : class;

        Task DeleteTaskAsync(string id);

        Task<IEnumerable<TResponse>> GetAllTasksAsync(string userId);

        Task<IEnumerable<TResponse>> GetTasksByStatusAsync(string userId, int status);
    }
}