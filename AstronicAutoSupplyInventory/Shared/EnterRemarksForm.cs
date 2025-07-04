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
    public partial class EnterRemarksForm : Form
    {
        private readonly ConfirmRemarksEventMessenger confirmRemarksEventMessenger;
        private readonly string orNumber;
        private readonly DataGridViewRow row;
        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public EnterRemarksForm(ConfirmRemarksEventMessenger confirmRemarksEventMessenger, 
            string orNumber, DataGridViewRow row)
        {
            this.confirmRemarksEventMessenger = confirmRemarksEventMessenger;

            this.orNumber = orNumber;

            this.row = row;

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            this.Close();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            btnConfirm.Focus();

            confirmRemarksEventMessenger(this.orNumber, txtRemarks.Text, dtpDate.Value, this.row);
        }

        private void EnterRemarksForm_Load(object sender, EventArgs e)
        {
            txtRemarks.Focus();
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }
    }
}
