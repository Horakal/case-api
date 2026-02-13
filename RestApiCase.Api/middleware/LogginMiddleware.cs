using RestApiCase.Domain.Logging.Interfaces;
using System.Diagnostics;

namespace RestApiCase.Api.middleware
{
    public class LogginMiddleware
    {
        private readonly RequestDelegate _next;

        public LogginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRequestLogFactory logFactory, IRequestLogRepository logRepository)
        {
            var stopwatch = Stopwatch.StartNew();
            var method = context.Request.Method;
            var path = context.Request.Path;

            try
            {
                await _next(context);
                stopwatch.Stop();

                var userId = GetUserId(context);
                if (string.Compare(context.Request.Path, "metrics") != 0)
                {
                    var log = logFactory.Create(method, path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds, userId);
                    await logRepository.SaveAsync(log);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                var userId = GetUserId(context);
                var log = logFactory.CreateError(method, path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds, userId, ex);
                await logRepository.SaveAsync(log);

                throw;
            }
        }

        private static Guid? GetUserId(HttpContext context)
        {
            var userIdClaim = context.User?.Claims
                .FirstOrDefault(c => c.Type == "jti")?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;

            return null;
        }
    }
}