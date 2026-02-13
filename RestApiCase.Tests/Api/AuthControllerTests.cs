using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using RestApiCase.Api.Controllers;
using RestApiCase.Application.User.DTOs.Requests;
using RestApiCase.Application.User.DTOs.Response;
using RestApiCase.Domain.Commons;
using RestApiCase.Domain.User.Entities;
using RestApiCase.Domain.User.Interface;
using System.Security.Claims;
using Xunit;

namespace RestApiCase.Tests.Api
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyForTestingPurposes1234567890!");
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            _controller = new AuthController(_mockConfig.Object, _mockUserService.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_Should_ReturnOkWithToken()
        {
            var userId = Guid.NewGuid();
            _mockUserService.Setup(s => s.Authenticate("user1", "pass1")).ReturnsAsync(userId);

            var result = await _controller.Login(new UserLogin { Username = "user1", Password = "pass1" });

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            var response = okResult.Value as UserResponse;
            response.Should().NotBeNull();
            response!.Token.Should().NotBeNullOrEmpty();
            response.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_InvalidCredentials_Should_ReturnUnauthorized()
        {
            _mockUserService.Setup(s => s.Authenticate("user1", "wrong")).ReturnsAsync((Guid?)null);

            var result = await _controller.Login(new UserLogin { Username = "user1", Password = "wrong" });

            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task Logout_ValidUser_Should_ReturnOk()
        {
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim("jti", userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var result = await _controller.Logout();

            result.Should().BeOfType<OkResult>();
            _mockUserService.Verify(s => s.DeleteToken(userId), Times.Once);
        }

        [Fact]
        public async Task Logout_NoUser_Should_ReturnOkWithoutDelete()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await _controller.Logout();

            result.Should().BeOfType<OkResult>();
            _mockUserService.Verify(s => s.DeleteToken(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Refresh_ValidToken_Should_ReturnNewToken()
        {
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@email.com", "hash");
            _mockUserService.Setup(s => s.GetById(userId)).ReturnsAsync(user);
            _mockUserService.Setup(s => s.ValidateToken("refresh123", userId)).ReturnsAsync(true);

            var request = new RefreshAccess { RefreshToken = "refresh123", UserId = userId.ToString() };
            var result = await _controller.Refresh(request);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var response = okResult!.Value as UserResponse;
            response.Should().NotBeNull();
            response!.Token.Should().NotBeNullOrEmpty();
            response.RefreshToken.Should().Be("refresh123");
        }

        [Fact]
        public async Task Refresh_EmptyRefreshToken_Should_ReturnBadRequest()
        {
            var request = new RefreshAccess { RefreshToken = "", UserId = Guid.NewGuid().ToString() };
            var result = await _controller.Refresh(request);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Refresh_InvalidUserId_Should_ReturnUnauthorized()
        {
            var request = new RefreshAccess { RefreshToken = "token", UserId = "not-a-guid" };
            var result = await _controller.Refresh(request);

            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task Refresh_UserNotFound_Should_ReturnUnauthorized()
        {
            var userId = Guid.NewGuid();
            _mockUserService.Setup(s => s.GetById(userId)).ReturnsAsync((User?)null);

            var request = new RefreshAccess { RefreshToken = "token", UserId = userId.ToString() };
            var result = await _controller.Refresh(request);

            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task Refresh_InvalidRefreshToken_Should_ReturnUnauthorized()
        {
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@email.com", "hash");
            _mockUserService.Setup(s => s.GetById(userId)).ReturnsAsync(user);
            _mockUserService.Setup(s => s.ValidateToken("bad-token", userId)).ReturnsAsync(false);

            var request = new RefreshAccess { RefreshToken = "bad-token", UserId = userId.ToString() };
            var result = await _controller.Refresh(request);

            result.Result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}
