using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infrastructure.EventMessenger;

namespace Infrastructure.UserControls
{
    public partial class PanelTile1UI : UserControl
    {
        private readonly int items;
        private readonly Color color;
        private readonly string title;
        private readonly NavigateToMinimumStockEventMessenger navigateToMinimumStockEventMessenger;

        public PanelTile1UI(NavigateToMinimumStockEventMessenger navigateToMinimumStockEventMessenger, int items, Color color, string title = "")
        {
            this.navigateToMinimumStockEventMessenger = navigateToMinimumStockEventMessenger;

            this.items = items;

            this.color = color;

            this.title = title;

            InitializeComponent();
        }

        private void PanelTile1UI_Load(object sender, EventArgs e)
        {
            lblNumber.Text = items.ToString("#,#");

            if (!string.IsNullOrWhiteSpace(title)) lblTitle.Text = title;

            lblTitle.BackColor = color;
        }

        private void lnkMore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            navigateToMinimumStockEventMessenger(true);
        }
    }
}
