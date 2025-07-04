using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infrastructure.UserControls
{
    public partial class DateNowPanelUI : UserControl
    {
        public DateNowPanelUI()
        {
            InitializeComponent();
        }

        private void DateNowPanel_Load(object sender, EventArgs e)
        {
            timer.Enabled = true;

            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;

            lblMonth.Text = now.ToString("MMMM");

            lblDay.Text = now.ToString("dd");

            lblYear.Text = now.ToString("yyyy");

            lblTime.Text = now.ToString("hh:mm:ss");

            lblAm.Text = now.ToString("tt") == "AM" ? "AM" : "";

            lblPm.Text = now.ToString("tt") == "PM" ? "PM" : "";
        }
    }
}
