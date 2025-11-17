using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Interfaces;
using RestApiCase.Application.Tasks.DTOS.ResponseDTO;
using RestApiCase.Domain.Tasks.Commands;
using RestApiCase.Application.Tasks.DTOS.Requests;
using RestApiCase.Application.Tasks.Mappings;

namespace RestApiCase.Application.Tasks.Services
{
    public class TaskService(ITaskRepository taskRepository) : ITaskService<TaskResponse>
    {
        private readonly ITaskRepository _taskRepository = taskRepository;

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

        public async Task DeleteTaskAsync(string id)
        {
            _ = Guid.TryParse(id, out Guid guid);
            if (guid == Guid.Empty)
            {
                throw new KeyNotFoundException("Task not found");
            }
            await _taskRepository.DeleteTaskAsync(guid);
        }

        public async Task<IEnumerable<TaskResponse>> GetAllTasksAsync(string id)
        {
            _ = Guid.TryParse(id, out Guid guid);
            if (guid == Guid.Empty)
            {
                throw new KeyNotFoundException("User not found");
            }

            var tasks = await _taskRepository.GetAllTasksAsync(guid);

            return [.. tasks.Select(x => x.ToTaskResponse())];
        }

        public async Task<TaskResponse?> GetTaskByIdAsync(string id)
        {
            _ = Guid.TryParse(id, out Guid guid);

            var task = await _taskRepository.GetTaskByIdAsync(guid);

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
            var taskToUpdate = await _taskRepository.GetTaskByIdAsync(updateTask.Id) ?? throw new KeyNotFoundException("Task not found");
            taskToUpdate.ApplyUpdate(updateTask);

            return await _taskRepository.UpdateTaskAsync(taskToUpdate)
                .ContinueWith(_ => taskToUpdate.ToTaskResponse());
        }

        public Task<IEnumerable<TaskResponse>> GetTasksByStatusAsync(string userId, int status)
        {
            throw new NotImplementedException();
        }
    }
}