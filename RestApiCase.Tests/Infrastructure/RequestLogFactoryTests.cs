using FluentAssertions;
using RestApiCase.Domain.Logging.Enums;
using RestApiCase.Infrastructure.Logging;
using Xunit;

namespace RestApiCase.Tests.Infrastructure
{
    public class RequestLogFactoryTests
    {
        private readonly RequestLogFactory _factory = new();

        [Fact]
        public void Create_Should_ReturnRequestLog()
        {
            var userId = Guid.NewGuid();

            var log = _factory.Create("GET", "/api/v1/tasks", 200, 45, userId);

            log.Method.Should().Be("GET");
            log.Path.Should().Be("/api/v1/tasks");
            log.StatusCode.Should().Be(200);
            log.ElapsedMs.Should().Be(45);
            log.LogType.Should().Be(LogType.Request);
            log.UserId.Should().Be(userId);
            log.ErrorMessage.Should().BeNull();
            log.StackTrace.Should().BeNull();
            log.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Create_NullUserId_Should_SetNull()
        {
            var log = _factory.Create("GET", "/healthcheck", 200, 10, null);

            log.UserId.Should().BeNull();
            log.LogType.Should().Be(LogType.Request);
        }

        [Fact]
        public void CreateError_Should_ReturnErrorLog()
        {
            var userId = Guid.NewGuid();
            var exception = new InvalidOperationException("Something went wrong");

            var log = _factory.CreateError("POST", "/api/v1/tasks", 500, 120, userId, exception);

            log.Method.Should().Be("POST");
            log.Path.Should().Be("/api/v1/tasks");
            log.StatusCode.Should().Be(500);
            log.ElapsedMs.Should().Be(120);
            log.LogType.Should().Be(LogType.Error);
            log.UserId.Should().Be(userId);
            log.ErrorMessage.Should().Be("Something went wrong");
            log.StackTrace.Should().BeNull(); // No stack trace when exception wasn't thrown
        }

        [Fact]
        public void CreateError_WithStackTrace_Should_IncludeIt()
        {
            try
            {
                throw new Exception("Test exception");
            }
            catch (Exception ex)
            {
                var log = _factory.CreateError("GET", "/api/error", 500, 200, null, ex);

                log.ErrorMessage.Should().Be("Test exception");
                log.StackTrace.Should().NotBeNullOrEmpty();
                log.LogType.Should().Be(LogType.Error);
                log.UserId.Should().BeNull();
            }
        }
    }
}
