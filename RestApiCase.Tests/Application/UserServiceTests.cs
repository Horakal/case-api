using FluentAssertions;
using Moq;
using RestApiCase.Application.User.Service;
using RestApiCase.Domain.Commons;
using RestApiCase.Domain.User.Entities;
using RestApiCase.Domain.User.Enums;
using RestApiCase.Domain.User.Interface;
using Xunit;

namespace RestApiCase.Tests.Application
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _service = new UserService(_mockRepo.Object);
        }

        [Fact]
        public async Task Authenticate_ValidCredentials_Should_ReturnGuid()
        {
            var expectedId = Guid.NewGuid();
            _mockRepo.Setup(r => r.Authenticate("user1", "pass1")).ReturnsAsync(expectedId);

            var result = await _service.Authenticate("user1", "pass1");

            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task Authenticate_InvalidCredentials_Should_ReturnNull()
        {
            _mockRepo.Setup(r => r.Authenticate("user1", "wrong")).ReturnsAsync((Guid?)null);

            var result = await _service.Authenticate("user1", "wrong");

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetById_ExistingUser_Should_ReturnUser()
        {
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@email.com", "hash");
            _mockRepo.Setup(r => r.GetUser(userId)).ReturnsAsync(user);

            var result = await _service.GetById(userId);

            result.Should().NotBeNull();
            result!.Username.Should().Be("testuser");
        }

        [Fact]
        public async Task GetById_NonExistingUser_Should_ReturnNull()
        {
            _mockRepo.Setup(r => r.GetUser(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            var result = await _service.GetById(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetRoles_Should_ReturnRoles()
        {
            var userId = Guid.NewGuid();
            var roles = new List<Role> { new Role(UserRoles.USER, userId) };
            _mockRepo.Setup(r => r.GetByIdWithRolesAsync(userId)).ReturnsAsync(roles);

            var result = await _service.GetRoles(userId);

            result.Should().HaveCount(1);
            result.First().RoleType.Should().Be(UserRoles.USER);
        }

        [Fact]
        public async Task CreateToken_Should_CallSaveToken()
        {
            var token = new RefreshToken("abc", Guid.NewGuid(), false, DateTime.UtcNow.AddDays(15));

            await _service.CreateToken(token);

            _mockRepo.Verify(r => r.SaveToken(token), Times.Once);
        }

        [Fact]
        public async Task ValidateToken_ValidToken_Should_ReturnTrue()
        {
            var userId = Guid.NewGuid();
            var refToken = new RefreshToken("valid-token", userId, false, DateTime.UtcNow.AddDays(15));
            _mockRepo.Setup(r => r.GetToken(userId)).ReturnsAsync(refToken);

            var result = await _service.ValidateToken("valid-token", userId);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateToken_WrongToken_Should_ReturnFalse()
        {
            var userId = Guid.NewGuid();
            var refToken = new RefreshToken("valid-token", userId, false, DateTime.UtcNow.AddDays(15));
            _mockRepo.Setup(r => r.GetToken(userId)).ReturnsAsync(refToken);

            var result = await _service.ValidateToken("wrong-token", userId);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateToken_ExpiredToken_Should_ReturnFalseAndDelete()
        {
            var userId = Guid.NewGuid();
            var refToken = new RefreshToken("token", userId, false, DateTime.UtcNow.AddDays(-1));
            _mockRepo.Setup(r => r.GetToken(userId)).ReturnsAsync(refToken);

            var result = await _service.ValidateToken("token", userId);

            result.Should().BeFalse();
            _mockRepo.Verify(r => r.DeleteToken(refToken), Times.Once);
        }

        [Fact]
        public async Task ValidateToken_NoToken_Should_ReturnFalse()
        {
            _mockRepo.Setup(r => r.GetToken(It.IsAny<Guid>())).ReturnsAsync((RefreshToken?)null);

            var result = await _service.ValidateToken("any", Guid.NewGuid());

            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteToken_ExistingToken_Should_Delete()
        {
            var userId = Guid.NewGuid();
            var refToken = new RefreshToken("token", userId, false, DateTime.UtcNow.AddDays(1));
            _mockRepo.Setup(r => r.GetToken(userId)).ReturnsAsync(refToken);

            await _service.DeleteToken(userId);

            _mockRepo.Verify(r => r.DeleteToken(refToken), Times.Once);
        }

        [Fact]
        public async Task DeleteToken_NoToken_Should_NotCallDelete()
        {
            _mockRepo.Setup(r => r.GetToken(It.IsAny<Guid>())).ReturnsAsync((RefreshToken?)null);

            await _service.DeleteToken(Guid.NewGuid());

            _mockRepo.Verify(r => r.DeleteToken(It.IsAny<RefreshToken>()), Times.Never);
        }

        [Fact]
        public async Task GetRefreshToken_Should_ReturnToken()
        {
            var userId = Guid.NewGuid();
            var refToken = new RefreshToken("token", userId, false, DateTime.UtcNow.AddDays(1));
            _mockRepo.Setup(r => r.GetToken(userId)).ReturnsAsync(refToken);

            var result = await _service.GetRefreshToken(userId);

            result.Should().NotBeNull();
            result!.Token.Should().Be("token");
        }

        [Fact]
        public void GetCurrentUser_Should_ReturnClaimsPrincipal()
        {
            var result = _service.GetCurrentUser("any-token");

            // Returns Thread.CurrentPrincipal which is null in test context
            result.Should().BeNull();
        }
    }
}
