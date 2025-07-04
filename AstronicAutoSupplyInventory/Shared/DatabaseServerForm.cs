using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AstronicAutoSupplyInventory.Shared
{
    public partial class DatabaseServerForm : Form
    {
        public DatabaseServerForm()
        {
            InitializeComponent();
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            //var dbContext = new InventoryServices.InventoryDbContext(
            //    txtServerName.Text.Trim(),
            //    txtUsername.Text,
            //    txtPassword.Text
            //);

            //if (dbContext.IsConnected()) MessageBox.Show("Connected");
        }
    }
}
