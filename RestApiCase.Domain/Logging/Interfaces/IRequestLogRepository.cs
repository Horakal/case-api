using RestApiCase.Domain.Logging.Entities;

namespace RestApiCase.Domain.Logging.Interfaces
{
    public interface IRequestLogRepository
    {
        Task SaveAsync(RequestLog log);
        Task<long> GetTotalRequestsAsync();
        Task<double> GetAverageResponseTimeMsAsync();
        Task<long> GetTotalErrorsAsync();
        Task<Dictionary<string, long>> GetRequestsByMethodAsync();
        Task<Dictionary<int, long>> GetRequestsByStatusCodeAsync();
    }
}
