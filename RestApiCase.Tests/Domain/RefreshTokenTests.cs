using FluentAssertions;
using RestApiCase.Domain.Commons;
using Xunit;

namespace RestApiCase.Tests.Domain
{
    public class RefreshTokenTests
    {
        [Fact]
        public void Create_ValidInput_Should_SetProperties()
        {
            var userId = Guid.NewGuid();
            var token = new RefreshToken("abc123", userId, false, DateTime.UtcNow.AddDays(15));

            token.Token.Should().Be("abc123");
            token.UserId.Should().Be(userId);
            token.IsRevoked.Should().BeFalse();
            token.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public void IsExpired_FutureDate_Should_ReturnFalse()
        {
            var token = new RefreshToken("abc", Guid.NewGuid(), false, DateTime.UtcNow.AddDays(1));

            token.IsExpired().Should().BeFalse();
        }

        [Fact]
        public void IsExpired_PastDate_Should_ReturnTrue()
        {
            var token = new RefreshToken("abc", Guid.NewGuid(), false, DateTime.UtcNow.AddDays(-1));

            token.IsExpired().Should().BeTrue();
        }
    }
}
