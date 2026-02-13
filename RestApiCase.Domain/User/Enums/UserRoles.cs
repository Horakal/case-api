using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Domain.User.Enums
{
    public enum UserRoles
    {
        [Description("User")]
        USER = 0,

        [Description("SuperUser")]
        SUPER_USER = 1,
    }
}