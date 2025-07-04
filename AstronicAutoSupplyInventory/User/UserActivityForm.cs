using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Shared;
using InventoryServices.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AstronicAutoSupplyInventory.User
{
    public partial class UserActivityForm : Form
    {
        private UserController userController = new UserController();
        private DateTime from;
        private DateTime to;

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private bool started;

        public UserActivityForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys) {
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
                case Keys.Escape:
                    this.Close();

                    return true;
                case Keys.Alt | Keys.D:
                    lnkMyInventory_LinkClicked(lnkMyInventory, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }


            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task InitializeActivities(DateTime from, DateTime to, string key = "")
        {
            var query = await userController.GetActivities(0, from, to, key);

            dgvItems.Rows.Clear();

            foreach (var item in query)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Cells[0].Value = item.Action;

                row.Cells[1].Value = item.Date.ToString();

                row.Cells[2].Value = item.UserName;
            }

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (DataGridViewRow currow in dgvItems.Rows)
            {
                currow.DefaultCellStyle.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
        }

        private void lnkMyInventory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var dateRangeForm = new DateRangeForm(new ConfirmDataRangeEventMessenger(ConfirmDateRangeInvoked), from, to);

            dateRangeForm.ShowDialog();

            dateRangeForm.BringToFront();
        }

        private async void ConfirmDateRangeInvoked(DateTime from, DateTime to)
        {
            try
            {
                pnlDateRange.Visible = from > DateTime.MinValue && to > DateTime.MinValue;

                lblDateRange.Text = string.Format("Sales Invoice from {0} to {1}",
                    from.ToShortDateString(), to.ToShortDateString());

                var dateRangeForm = (DateRangeForm)Application.OpenForms["DateRangeForm"];

                if (dateRangeForm == null) return;

                dateRangeForm.Close();

                if (from > DateTime.MinValue && to > DateTime.MinValue)
                {
                    this.from = from;

                    this.to = to;
                }

                var key = txtSearch.Text;

                await InitializeActivities(from, to, key);
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            pnlDateRange.Visible = false;

            from = DateTime.MinValue;

            to = DateTime.MinValue;
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!started) return;

            var key = txtSearch.Text;

            try
            {
                await InitializeActivities(from, to, key);
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }

        private async void UserActivityForm_Load(object sender, EventArgs e)
        {
            mainForm.ShowProgressStatus();

            try
            {
                await InitializeActivities(DateTime.MinValue, DateTime.MinValue);

                started = true;
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }
    }
}
