using Microsoft.AspNetCore.Http;
using RestApiCase.Application.Tasks.DTOS.Requests;
using RestApiCase.Application.Tasks.DTOS.ResponseDTO;
using RestApiCase.Application.Tasks.Mappings;
using RestApiCase.Domain.Tasks.Commands;
using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Enums;
using RestApiCase.Domain.Tasks.Interfaces;
using System.Net.Http;

namespace RestApiCase.Application.Tasks.Services
{
    public class TaskService(ITaskRepository taskRepository, IHttpContextAccessor contextAccessor) : ITaskService<TaskResponse>
    {
        private readonly ITaskRepository _taskRepository = taskRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = contextAccessor;

        public async Task<TaskResponse> ExecuteAsync<TRequest>(TRequest request) where TRequest : ICommand
        {
            return request switch
            {
                CreateTask createTask => await CreateTaskAsync(createTask),
                UpdateTask updateTask => await UpdateTaskAsync(updateTask),
                null => throw new ArgumentNullException(nameof(request), "Request cannot be null"),
                _ => throw new ArgumentException("Unsupported command type", nameof(request)),
            };
        }

        public async Task<TaskResponse> CreateTaskAsync<TRequest>(TRequest request) where TRequest : ICommand
        {
            if (request is not CreateTask createTask)
            {
                throw new ArgumentException("Invalid request type", nameof(request));
            }

            var newTask = createTask.ToTaskItem();

            var task = await _taskRepository.CreateTaskAsync(newTask);

            return task.ToTaskResponse();
        }

        public async Task DeleteTaskAsync(Guid id)
        {
            await _taskRepository.DeleteTaskAsync(id);
        }

        public async Task<IEnumerable<TaskResponse>> GetAllTasksAsync(Guid userId, TaskItemStatus? status = null)
        {
            var isSuperUser = _httpContextAccessor.HttpContext.User.IsInRole("SUPER_USER");
            var tasks = await _taskRepository.GetAllTasksAsync(userId, isSuperUser, status);

            return tasks.Select(x => x.ToTaskResponse());
        }

        public async Task<TaskResponse?> GetTaskByIdAsync(Guid id, Guid userId)
        {
            var isSuperUser = _httpContextAccessor.HttpContext.User.IsInRole("SUPER_USER");
            var task = await _taskRepository.GetTaskByIdAsync(id, isSuperUser, userId);

            return task?.ToTaskResponse();
        }

        public async Task<TaskResponse> UpdateTaskAsync<TRequest>(TRequest request) where TRequest : class
        {
            if (request is not UpdateTask updateTask)
            {
                throw new ArgumentException("Invalid request type", nameof(request));
            }

            if (updateTask.Id == Guid.Empty)
            {
                throw new KeyNotFoundException("Invalid Id");
            }
           
            var isSuperUser = _httpContextAccessor.HttpContext.User.IsInRole("SUPER_USER");
            var taskToUpdate = await _taskRepository.GetTaskByIdAsync(updateTask.Id, isSuperUser, updateTask.UserUpdateId) ?? throw new KeyNotFoundException("Task not found");
            taskToUpdate.ApplyUpdate(updateTask);

            return await _taskRepository.UpdateTaskAsync(taskToUpdate)
                .ContinueWith(_ => taskToUpdate.ToTaskResponse());
        }

    }
}