using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Domain.User.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
    }
}