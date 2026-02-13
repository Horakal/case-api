using RestApiCase.Domain.Logging.Entities;

namespace RestApiCase.Domain.Logging.Interfaces
{
    public interface IRequestLogFactory
    {
        RequestLog Create(string method, string path, int statusCode, long elapsedMs, Guid? userId);
        RequestLog CreateError(string method, string path, int statusCode, long elapsedMs, Guid? userId, Exception exception);
    }
}
