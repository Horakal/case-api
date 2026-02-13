using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using RestApiCase.Api.middleware;
using RestApiCase.Domain.Logging.Entities;
using RestApiCase.Domain.Logging.Interfaces;
using System.Security.Claims;
using Xunit;

namespace RestApiCase.Tests.Api
{
    public class LogginMiddlewareTests
    {
        private readonly Mock<IRequestLogFactory> _mockFactory;
        private readonly Mock<IRequestLogRepository> _mockRepo;

        public LogginMiddlewareTests()
        {
            _mockFactory = new Mock<IRequestLogFactory>();
            _mockRepo = new Mock<IRequestLogRepository>();
        }

        [Fact]
        public async Task InvokeAsync_SuccessfulRequest_Should_LogRequest()
        {
            var log = new RequestLog("GET", "/api/test", 200, 10, RestApiCase.Domain.Logging.Enums.LogType.Request, null);
            _mockFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<Guid?>()))
                        .Returns(log);

            var middleware = new LogginMiddleware(next: (ctx) => Task.CompletedTask);
            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/api/test";

            await middleware.InvokeAsync(context, _mockFactory.Object, _mockRepo.Object);

            _mockFactory.Verify(f => f.Create("GET", "/api/test", It.IsAny<int>(), It.IsAny<long>(), null), Times.Once);
            _mockRepo.Verify(r => r.SaveAsync(log), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WithAuthenticatedUser_Should_PassUserId()
        {
            var userId = Guid.NewGuid();
            var log = new RequestLog("GET", "/api/test", 200, 10, RestApiCase.Domain.Logging.Enums.LogType.Request, userId);
            _mockFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>(), userId))
                        .Returns(log);

            var middleware = new LogginMiddleware(next: (ctx) => Task.CompletedTask);
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("jti", userId.ToString())
            }, "TestAuth"));

            await middleware.InvokeAsync(context, _mockFactory.Object, _mockRepo.Object);

            _mockFactory.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>(), userId), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_ExceptionThrown_Should_LogError()
        {
            var exception = new InvalidOperationException("boom");
            var log = new RequestLog("POST", "/api/test", 0, 10, RestApiCase.Domain.Logging.Enums.LogType.Error, null, "boom");
            _mockFactory.Setup(f => f.CreateError(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<Guid?>(), It.IsAny<Exception>()))
                        .Returns(log);

            var middleware = new LogginMiddleware(next: (ctx) => throw exception);
            var context = new DefaultHttpContext();

            var act = () => middleware.InvokeAsync(context, _mockFactory.Object, _mockRepo.Object);

            await act.Should().ThrowAsync<InvalidOperationException>();
            _mockFactory.Verify(f => f.CreateError(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>(), null, exception), Times.Once);
            _mockRepo.Verify(r => r.SaveAsync(log), Times.Once);
        }
    }
}
