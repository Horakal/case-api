using RestApiCase.Domain.Commons;
using RestApiCase.Domain.User.Enums;

namespace RestApiCase.Domain.User.Entities
{
    public class Role : Entity<Guid>
    {
        public UserRoles RoleType { get; private set; }

        public Guid UserId { get; private set; }

        private Role()
        { }

        public Role(UserRoles role, Guid userId)
        {
            RoleType = role;
            UserId = userId;
        }
    }
}