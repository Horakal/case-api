using FluentAssertions;
using RestApiCase.Domain.User.Entities;
using RestApiCase.Domain.User.Enums;
using Xunit;

namespace RestApiCase.Tests.Domain
{
    public class RoleTests
    {
        [Fact]
        public void Create_ValidRole_Should_SetProperties()
        {
            var userId = Guid.NewGuid();
            var role = new Role(UserRoles.USER, userId);

            role.RoleType.Should().Be(UserRoles.USER);
            role.UserId.Should().Be(userId);
        }

        [Fact]
        public void Create_SuperUserRole_Should_SetProperties()
        {
            var userId = Guid.NewGuid();
            var role = new Role(UserRoles.SUPER_USER, userId);

            role.RoleType.Should().Be(UserRoles.SUPER_USER);
            role.UserId.Should().Be(userId);
        }
    }
}
