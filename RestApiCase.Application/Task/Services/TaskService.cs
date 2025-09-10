using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Interfaces;

namespace RestApiCase.Application.Tasks.Services
{
    public class TaskService(ITaskRepository taskRepository) : ITaskService
    {
        private readonly ITaskRepository _taskRepository = taskRepository;

        public async Task<TaskItem> CreateTaskAsync(Guid userId, string title, string description, string? summary, DateTime? update)
        {
            var newTask = new TaskItem(userId, title, description, summary, update);

            await _taskRepository.CreateTaskAsync(newTask);

            return newTask;
        }

        public async Task<bool> DeleteTaskAsync(string id)
        {
            _ = Guid.TryParse(id, out Guid guid);
            if (guid == Guid.Empty)
            {
                throw new KeyNotFoundException("Task not found");
            }
            await _taskRepository.DeleteTaskAsync(guid);

            return true;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync(string id)
        {
            _ = Guid.TryParse(id, out Guid guid);
            if (guid == Guid.Empty)
            {
                throw new KeyNotFoundException("User not found");
            }

            var tasks = await _taskRepository.GetAllTasksAsync(guid);

            return tasks;
        }

        public Task<TaskItem?> GetTaskByIdAsync(string id)
        {
            _ = Guid.TryParse(id, out Guid guid);
            if (guid == Guid.Empty)
            {
                throw new KeyNotFoundException("Task not found");
            }
            var task = _taskRepository.GetTaskByIdAsync(guid);

            return task;
        }

        public Task<bool> UpdateTaskAsync(string id, string? title, string? description, TaskStatus? status, DateTime? dueDate)
        {
            throw new NotImplementedException();
        }
    }
}