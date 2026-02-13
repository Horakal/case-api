using RestApiCase.Domain.Logging.Entities;
using RestApiCase.Domain.Logging.Enums;
using RestApiCase.Domain.Logging.Interfaces;

namespace RestApiCase.Infrastructure.Logging
{
    public class RequestLogFactory : IRequestLogFactory
    {
        public RequestLog Create(string method, string path, int statusCode, long elapsedMs, Guid? userId)
        {
            return new RequestLog(
                method: method,
                path: path,
                statusCode: statusCode,
                elapsedMs: elapsedMs,
                logType: LogType.Request,
                userId: userId
            );
        }

        public RequestLog CreateError(string method, string path, int statusCode, long elapsedMs, Guid? userId, Exception exception)
        {
            return new RequestLog(
                method: method,
                path: path,
                statusCode: statusCode,
                elapsedMs: elapsedMs,
                logType: LogType.Error,
                userId: userId,
                errorMessage: exception.Message,
                stackTrace: exception.StackTrace
            );
        }
    }
}
