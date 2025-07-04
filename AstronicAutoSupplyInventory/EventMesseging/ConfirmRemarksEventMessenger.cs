using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AstronicAutoSupplyInventory.EventMesseging
{
    public delegate void ConfirmRemarksEventMessenger(string orNumber, string remarks, DateTime date, DataGridViewRow row);
}
