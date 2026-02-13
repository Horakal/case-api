using FluentAssertions;
using RestApiCase.Domain.User.Entities;
using Xunit;

namespace RestApiCase.Tests.Domain
{
    public class EntityTests
    {
        [Fact]
        public void Equals_SameId_Should_ReturnTrue()
        {
            var user1 = new User("user1", "a@b.com", "hash");
            var user2 = user1;

            user1.Equals(user2).Should().BeTrue();
        }

        [Fact]
        public void Equals_DifferentId_Should_ReturnFalse()
        {
            var user1 = new User("user1", "a@b.com", "hash");
            var user2 = new User("user2", "b@b.com", "hash");

            user1.Equals(user2).Should().BeFalse();
        }

        [Fact]
        public void Equals_Null_Should_ReturnFalse()
        {
            var user = new User("user1", "a@b.com", "hash");

            user.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void Equals_DifferentType_Should_ReturnFalse()
        {
            var user = new User("user1", "a@b.com", "hash");

            user.Equals("not a user").Should().BeFalse();
        }

        [Fact]
        public void OperatorEquals_SameEntity_Should_ReturnTrue()
        {
            var user1 = new User("user1", "a@b.com", "hash");
            var user2 = user1;

            (user1 == user2).Should().BeTrue();
        }

        [Fact]
        public void OperatorEquals_DifferentEntity_Should_ReturnFalse()
        {
            var user1 = new User("user1", "a@b.com", "hash");
            var user2 = new User("user2", "b@b.com", "hash");

            (user1 == user2).Should().BeFalse();
        }

        [Fact]
        public void OperatorNotEquals_DifferentEntity_Should_ReturnTrue()
        {
            var user1 = new User("user1", "a@b.com", "hash");
            var user2 = new User("user2", "b@b.com", "hash");

            (user1 != user2).Should().BeTrue();
        }

        [Fact]
        public void OperatorEquals_BothNull_Should_ReturnTrue()
        {
            User? user1 = null;
            User? user2 = null;

            (user1 == user2).Should().BeTrue();
        }

        [Fact]
        public void OperatorEquals_OneNull_Should_ReturnFalse()
        {
            var user1 = new User("user1", "a@b.com", "hash");
            User? user2 = null;

            (user1 == user2).Should().BeFalse();
            (user2 == user1).Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_Should_ReturnIdHashCode()
        {
            var user = new User("user1", "a@b.com", "hash");

            user.GetHashCode().Should().Be(user.Id.GetHashCode());
        }
    }
}
