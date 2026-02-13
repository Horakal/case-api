using RestApiCase.Domain.Commons;
using RestApiCase.Domain.User.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Domain.User.Entities
{
    public class User : Entity<Guid>
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

        private readonly List<Role> _roles = [];

        private User()
        { }

        public User(string username, string email, string hashedPassword)
        {
            Id = Guid.NewGuid();
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Password = hashedPassword ?? throw new ArgumentNullException(nameof(hashedPassword));
            CreatedAt = DateTime.UtcNow;
        }

        public void AddRole(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (_roles.Any(r => r.Id == role.Id)) throw new DomainException("Role já associada ao user.");

            _roles.Add(role);
        }

        public void RemoveRole(Guid roleId)
        {
            var roleToRemove = _roles.FirstOrDefault(r => r.Id == roleId);
            if (roleToRemove == null) throw new DomainException("Role não encontrada.");

            _roles.Remove(roleToRemove);
        }

        public bool IsSuperUser() => _roles.Any(r => r.RoleType == UserRoles.SUPER_USER);
    }
}