using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class UserRoleDtos
    {
        public int UserRoleId { get; set; }
        public string RoleName { get; set; }

        public IEnumerable<UserPrivilegeDtos> UserPrivilegeDtosList { get; set; }
    }
}
