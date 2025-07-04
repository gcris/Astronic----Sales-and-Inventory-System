using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Dtos
{
    public class UserPrivilegeDtos
    {
        public int UserPrivilegeId { get; set; }
        public string Action { get; set; }
        public bool IsEnable { get; set; }

        public int UserRoleId { get; set; }
    }
}
