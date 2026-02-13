using FluentAssertions;
using RestApiCase.Domain.Commons;
using RestApiCase.Domain.User.Entities;
using RestApiCase.Domain.User.Enums;
using Xunit;

namespace RestApiCase.Tests.Domain
{
    public class UserTests
    {
        [Fact]
        public void Create_ValidInput_Should_SetPropertiesCorrectly()
        {
            var user = new User("testuser", "test@email.com", "hashedpassword");

            user.Username.Should().Be("testuser");
            user.Email.Should().Be("test@email.com");
            user.Password.Should().Be("hashedpassword");
            user.Id.Should().NotBeEmpty();
            user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            user.Roles.Should().BeEmpty();
        }

        [Fact]
        public void Create_NullUsername_Should_Throw()
        {
            Action act = () => new User(null!, "test@email.com", "hash");
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Create_NullEmail_Should_Throw()
        {
            Action act = () => new User("user", null!, "hash");
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Create_NullPassword_Should_Throw()
        {
            Action act = () => new User("user", "test@email.com", null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddRole_ValidRole_Should_AddToCollection()
        {
            var user = new User("testuser", "test@email.com", "hash");
            var role = new Role(UserRoles.USER, user.Id);

            user.AddRole(role);

            user.Roles.Should().HaveCount(1);
            user.Roles.First().RoleType.Should().Be(UserRoles.USER);
        }

        [Fact]
        public void AddRole_NullRole_Should_Throw()
        {
            var user = new User("testuser", "test@email.com", "hash");

            Action act = () => user.AddRole(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddRole_DuplicateRole_Should_Throw()
        {
            var user = new User("testuser", "test@email.com", "hash");
            var role = new Role(UserRoles.USER, user.Id);
            user.AddRole(role);

            Action act = () => user.AddRole(role);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void RemoveRole_ExistingRole_Should_RemoveFromCollection()
        {
            var user = new User("testuser", "test@email.com", "hash");
            var role = new Role(UserRoles.USER, user.Id);
            user.AddRole(role);

            user.RemoveRole(role.Id);

            user.Roles.Should().BeEmpty();
        }

        [Fact]
        public void RemoveRole_NonExistingRole_Should_Throw()
        {
            var user = new User("testuser", "test@email.com", "hash");

            Action act = () => user.RemoveRole(Guid.NewGuid());
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void IsSuperUser_WithSuperUserRole_Should_ReturnTrue()
        {
            var user = new User("admin", "admin@email.com", "hash");
            var role = new Role(UserRoles.SUPER_USER, user.Id);
            user.AddRole(role);

            user.IsSuperUser().Should().BeTrue();
        }

        [Fact]
        public void IsSuperUser_WithUserRole_Should_ReturnFalse()
        {
            var user = new User("user", "user@email.com", "hash");
            var role = new Role(UserRoles.USER, user.Id);
            user.AddRole(role);

            user.IsSuperUser().Should().BeFalse();
        }

        [Fact]
        public void IsSuperUser_NoRoles_Should_ReturnFalse()
        {
            var user = new User("user", "user@email.com", "hash");

            user.IsSuperUser().Should().BeFalse();
        }
    }
}
