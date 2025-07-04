using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class UserDtos
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public bool IsEnable { get; set; }

        public string UserRole { get; set; }
        public int UserRoleId { get; set; }

        public IEnumerable<UserPrivilegeDtos> UserPrivilegeDtosList { get; set; }

        public IEnumerable<PasswordHistoryDtos> PasswordHistoryDtosList { get; set; }
    }
}
