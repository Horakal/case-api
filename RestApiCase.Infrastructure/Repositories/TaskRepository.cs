using Microsoft.EntityFrameworkCore;
using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Enums;
using RestApiCase.Domain.Tasks.Interfaces;
using RestApiCase.Domain.User.Entities;
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
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
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

        public async Task<IReadOnlyList<TaskItem>> GetAllTasksAsync(Guid userId, bool isSuperUser, TaskItemStatus? status = null)
        {
            var query = _context.Tasks.AsQueryable();
            if (!isSuperUser)
            {
                query = query.Where(t => t.UserId == userId);
            }
            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }
            query = query.OrderBy(t => t.Status == TaskItemStatus.Pending ? 0 : 1);
            return await query.ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(Guid id, bool isSuperUser, Guid userId)
        {
            var query = _context.Tasks.AsQueryable();
            if (!isSuperUser)
            {
                query = query.Where(t => t.UserId == userId);
            }
            query = query.Where(t => t.Id == id);
            query = query.OrderBy(t => t.Status == TaskItemStatus.Pending ? 0 : 1);
            return await query.FirstOrDefaultAsync();
        }


        public async Task UpdateTaskAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }
    }
}