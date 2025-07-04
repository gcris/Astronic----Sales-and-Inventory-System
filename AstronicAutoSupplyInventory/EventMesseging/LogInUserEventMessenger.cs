using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstronicAutoSupplyInventory.EventMesseging
{
    public delegate void LogInUserEventMessenger(UserDtos userDtos, bool onClosing = false);
}
