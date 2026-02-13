using RestApiCase.Domain.Commons;
using RestApiCase.Domain.Logging.Enums;

namespace RestApiCase.Domain.Logging.Entities
{
    public class RequestLog : Entity<Guid>
    {
        public string Method { get; private set; }
        public string Path { get; private set; }
        public int StatusCode { get; private set; }
        public long ElapsedMs { get; private set; }
        public LogType LogType { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? StackTrace { get; private set; }
        public Guid? UserId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private RequestLog() { }

        public RequestLog(
            string method,
            string path,
            int statusCode,
            long elapsedMs,
            LogType logType,
            Guid? userId,
            string? errorMessage = null,
            string? stackTrace = null)
        {
            Id = Guid.NewGuid();
            Method = method;
            Path = path;
            StatusCode = statusCode;
            ElapsedMs = elapsedMs;
            LogType = logType;
            UserId = userId;
            ErrorMessage = errorMessage;
            StackTrace = stackTrace;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
