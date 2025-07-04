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
    public partial class IndexForm : Form
    {
        private int xPos = 0;

        public IndexForm()
        {
            InitializeComponent();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.Width <= xPos)
            {
                //repeat marquee
                lblTitle.Location = new System.Drawing.Point(0, lblTitle.Location.Y);

                xPos = 0;
            }
            else
            {
                lblTitle.Location = new System.Drawing.Point(xPos, lblTitle.Location.Y);

                xPos += 10;
            }
        }

        private void IndexForm_Load(object sender, EventArgs e)
        {
            timer.Enabled = true;
            timer.Start();
        }

        private void pnlContent_Resize(object sender, EventArgs e)
        {
            picBatch.Size = new Size((pnlContent.Width / 2) - 5, picBatch.Height);
            picLaoag.Size = new Size((pnlContent.Width / 2) - 5, picLaoag.Height);
        }
    }
}
