using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infrastructure.EventMessenger
{
    public delegate void ChangeUserControlEventMessenger(Form form, bool started = true);
}
