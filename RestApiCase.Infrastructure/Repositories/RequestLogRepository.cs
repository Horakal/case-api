using Microsoft.EntityFrameworkCore;
using RestApiCase.Domain.Logging.Entities;
using RestApiCase.Domain.Logging.Enums;
using RestApiCase.Domain.Logging.Interfaces;
using RestApiCase.Infrastructure.Data;

namespace RestApiCase.Infrastructure.Repositories
{
    public class RequestLogRepository(ApplicationDbContext context) : IRequestLogRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task SaveAsync(RequestLog log)
        {
            await _context.RequestLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<long> GetTotalRequestsAsync()
        {
            return await _context.RequestLogs.LongCountAsync();
        }

        public async Task<double> GetAverageResponseTimeMsAsync()
        {
            if (!await _context.RequestLogs.AnyAsync())
                return 0;

            return await _context.RequestLogs.AverageAsync(l => l.ElapsedMs);
        }

        public async Task<long> GetTotalErrorsAsync()
        {
            return await _context.RequestLogs
                .Where(l => l.LogType == LogType.Error)
                .LongCountAsync();
        }

        public async Task<Dictionary<string, long>> GetRequestsByMethodAsync()
        {
            return await _context.RequestLogs
                .GroupBy(l => l.Method)
                .ToDictionaryAsync(g => g.Key, g => g.LongCount());
        }

        public async Task<Dictionary<int, long>> GetRequestsByStatusCodeAsync()
        {
            return await _context.RequestLogs
                .GroupBy(l => l.StatusCode)
                .ToDictionaryAsync(g => g.Key, g => g.LongCount());
        }
    }
}
