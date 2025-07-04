using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Dtos
{
    public class PasswordHistoryDtos
    {
        public int PasswordHistoryId { get; set; }
        public string Password { get; set; }
        public DateTime Date { get; set; }

        public int UserId { get; set; }
    }
}
