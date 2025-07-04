using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.EventMessenger
{
    public delegate void NotifierEventMessenger(bool success);
}
