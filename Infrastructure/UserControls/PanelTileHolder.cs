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
    public partial class PanelTileHolder : UserControl
    {
        private readonly UserControl control;

        public PanelTileHolder(UserControl control)
        {
            this.control = control;

            InitializeComponent();
        }

        private void PanelTileHolder_Load(object sender, EventArgs e)
        {
            pnlControl.Controls.Add(control);

            this.Size = new Size(control.Width + this.Padding.Left + this.Padding.Right + pnlControl.Padding.All,
                control.Height + this.Padding.Top + pnlControl.Padding.All);

            control.Dock = DockStyle.Fill;
        }
    }
}
