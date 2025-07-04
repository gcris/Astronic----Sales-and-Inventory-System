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
    public partial class UserListForm : Form
    {
        private UserController controller = new UserController();

        private bool started;

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public UserListForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.F1:
                    btnAdd_Click(this, new EventArgs());

                    return true;
                case Keys.F2:
                    btnEdit_Click(this, new EventArgs());

                    return true;
                case Keys.F3:
                    btnEnableDisable_Click(this, new EventArgs());

                    return true;
                case Keys.Escape:
                    btnClose_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task InitializeUser(string key = "")
        {
            var users = await controller.GetAll(key);

            dgvItems.Rows.Clear();

            foreach (var user in users.Where(user => user.UserId != mainForm.UserDtos.UserId))
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = user.UserId;

                row.Cells[0].Value = user.Username;

                row.Cells[1].Value = !string.IsNullOrWhiteSpace(user.FirstName) && 
                    !string.IsNullOrWhiteSpace(user.LastName) ? string.Format("{0}, {1}", user.LastName, user.FirstName) :
                    !string.IsNullOrWhiteSpace(user.FirstName) ? user.FirstName : user.LastName;

                row.Cells[2].Value = user.Address;

                row.Cells[3].Value = user.ContactNumber;

                row.Cells[4].Value = user.UserRole;

                row.Cells[5].Value = user.IsEnable ? "Yes" : "No";
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

        private async void UserListForm_Load(object sender, EventArgs e)
        {
            mainForm.ShowProgressStatus();

            try
            {
                await InitializeUser();

                started = true;
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!started) return;

            try
            {
                await InitializeUser(txtSearch.Text);
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addEditUserForm = new AddEditUserForm();

            addEditUserForm.ShowDialog();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            var id = 0;

            if (row.Tag == null) return;

            int.TryParse(row.Tag.ToString(), out id);

            if (id < 1) return;

            var addEditUserForm = new AddEditUserForm(id);

            addEditUserForm.ShowDialog();
        }

        private async void btnEnableDisable_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            var id = 0;

            if (row.Tag == null) return;

            int.TryParse(row.Tag.ToString(), out id);

            if (id < 1) return;

            var enable = row.Cells[5].Value.ToString() == "Yes";

            mainForm.ShowProgressStatus();

            try
            {
                var result = mainForm.ShowMessage(string.Format("Are you sure you want to {0}?", 
                    enable ? "disable" : "enable"), true);

                if (result == System.Windows.Forms.DialogResult.No) return;

                var success = await controller.EnableUser(id, !enable);

                if (success)
                {
                    mainForm.ShowMessage(string.Format("Successfully {0}", enable ? "disabled" : "enabled"));

                    row.DefaultCellStyle.BackColor = enable ? Color.OrangeRed : Color.White;

                    row.Cells[5].Value = enable ? "No" : "Yes";
                }
                else mainForm.ShowMessage(string.Format("We cannot {0} this user. Try it later.", enable ? "disable" : "enable"));
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }
    }
}
