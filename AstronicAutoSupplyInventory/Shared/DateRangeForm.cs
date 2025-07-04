using AstronicAutoSupplyInventory.EventMesseging;
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
    public partial class DateRangeForm : Form
    {
        private readonly ConfirmDataRangeEventMessenger confirmDataRangeEventMessenger;
        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private readonly DateTime dateFrom;
        private readonly DateTime dateTo;

        public DateRangeForm(ConfirmDataRangeEventMessenger confirmDataRangeEventMessenger, DateTime dateFrom, DateTime dateTo)
        {
            this.confirmDataRangeEventMessenger = confirmDataRangeEventMessenger;
            this.dateFrom = dateFrom;
            this.dateTo = dateTo;

            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Enter:
                    btnConfirm_Click(this, new EventArgs());

                    return true;
                case Keys.Escape:
                    btnCancel_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            confirmDataRangeEventMessenger(dtpFrom.Value.Date, dtpTo.Value.Date);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            this.Close();
        }

        private void DateRangeForm_Load(object sender, EventArgs e)
        {
            dtpFrom.Value = dateFrom;
            dtpTo.Value = dateTo;
            dtpFrom.Focus();
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }
    }
}
