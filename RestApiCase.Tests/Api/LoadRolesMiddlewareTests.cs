using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using RestApiCase.Api.middleware;
using RestApiCase.Domain.User.Entities;
using RestApiCase.Domain.User.Enums;
using RestApiCase.Domain.User.Interface;
using System.Security.Claims;
using Xunit;

namespace RestApiCase.Tests.Api
{
    public class LoadRolesMiddlewareTests
    {
        private readonly Mock<IUserService> _mockUserService;

        public LoadRolesMiddlewareTests()
        {
            _mockUserService = new Mock<IUserService>();
        }

        [Fact]
        public async Task InvokeAsync_AuthenticatedUser_Should_AddRoleClaims()
        {
            var userId = Guid.NewGuid();
            var roles = new List<Role> { new Role(UserRoles.USER, userId) };
            _mockUserService.Setup(s => s.GetRoles(userId)).ReturnsAsync(roles);

            var nextCalled = false;
            var middleware = new LoadRolesMiddleware(next: (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("jti", userId.ToString())
            }, "TestAuth"));

            await middleware.InvokeAsync(context, _mockUserService.Object);

            nextCalled.Should().BeTrue();
            context.User.IsInRole("USER").Should().BeTrue();
        }

        [Fact]
        public async Task InvokeAsync_SuperUser_Should_AddSuperUserClaim()
        {
            var userId = Guid.NewGuid();
            var roles = new List<Role> { new Role(UserRoles.SUPER_USER, userId) };
            _mockUserService.Setup(s => s.GetRoles(userId)).ReturnsAsync(roles);

            var middleware = new LoadRolesMiddleware(next: (ctx) => Task.CompletedTask);
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("jti", userId.ToString())
            }, "TestAuth"));

            await middleware.InvokeAsync(context, _mockUserService.Object);

            context.User.IsInRole("SUPER_USER").Should().BeTrue();
        }

        [Fact]
        public async Task InvokeAsync_NoUser_Should_CallNextWithoutRoles()
        {
            var nextCalled = false;
            var middleware = new LoadRolesMiddleware(next: (ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

            var context = new DefaultHttpContext();

            await middleware.InvokeAsync(context, _mockUserService.Object);

            nextCalled.Should().BeTrue();
            _mockUserService.Verify(s => s.GetRoles(It.IsAny<Guid>()), Times.Never);
        }
    }
}
