using Microsoft.EntityFrameworkCore;
using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Interfaces;
using RestApiCase.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskDbContext _context;

        public TaskRepository(TaskDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task DeleteTaskAsync(Guid id)
        {
            await _context.Tasks.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync(Guid userId)
        {
            List<TaskItem> tasks = await _context.Tasks.Where(x => x.UserId == userId).OrderByDescending(x => x.Status).ToListAsync();
            return tasks;
        }

        public async Task<TaskItem?> GetTaskByIdAsync(Guid id)
        {
            return await _context.Tasks.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }
    }
}