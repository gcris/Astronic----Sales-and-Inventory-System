using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infrastructure
{
    public partial class FloatForm : Form
    {
        private readonly UserControl userControl;
        private readonly bool isLoading;

        public FloatForm(UserControl userControl, bool isLoading = false)
        {
            this.userControl = userControl;
            
            this.isLoading = isLoading;

            InitializeComponent();
        }

        //private void PlaceLowerRight()
        //{
        //    //Determine "rightmost" screen
        //    var rightmost = Screen.PrimaryScreen;

        //    foreach (Screen screen in Screen.AllScreens)
        //    {
        //        if (screen.WorkingArea.Right > rightmost.WorkingArea.Right)
        //            rightmost = screen;
        //    }

        //    this.Left = rightmost.WorkingArea.Right - this.Width - 2;

        //    this.Top = rightmost.WorkingArea.Bottom - this.Height - 2;
        //}

        private void FloatForm_Load(object sender, EventArgs e)
        {
            this.Name = userControl.Name;

            this.FormBorderStyle = isLoading ? FormBorderStyle.None : FormBorderStyle.FixedSingle;

            this.Size = new Size(userControl.Size.Width + 25, userControl.Size.Height + (isLoading ? 0 : 39));

            this.Text = userControl.AccessibleName;

            if (isLoading) userControl.BorderStyle = BorderStyle.FixedSingle;

            this.CenterToScreen();

            Controls.Add(userControl);

            userControl.Dock = DockStyle.Fill;

            this.BringToFront();
        }
    }
}
