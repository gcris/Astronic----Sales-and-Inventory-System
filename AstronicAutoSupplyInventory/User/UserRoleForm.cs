using CommonLibrary.Dtos;
using InventoryServices.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AstronicAutoSupplyInventory.User
{
    public partial class UserRoleForm : Form
    {
        private UserController controller = new UserController();
        private bool started;

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public UserRoleForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Alt | Keys.R:
                    cboRoles.Focus();

                    return true;
                case Keys.F1:
                    btnConfirm_Click(this, new EventArgs());

                    return true;
                case Keys.Escape:
                    btnCancel_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.A:
                    btnAddUserRole_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task InitializeRoles(string roleName = null)
        {
            var list = await controller.GetAllRoles();

            int roleId = 0;

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                UserRoleDtos data = list.FirstOrDefault(d => d.RoleName == roleName);

                if (data != null)
                {
                    roleId = data.UserRoleId;
                }
            }

            cboRoles.AutoCompleteMode = AutoCompleteMode.None;

            cboRoles.AutoCompleteSource = AutoCompleteSource.ListItems;

            cboRoles.DataSource = list.ToList(); ;

            cboRoles.DisplayMember = "RoleName";

            cboRoles.ValueMember = "UserRoleId";

            if (roleId == 0)
            {
                cboRoles.SelectedIndex = 0;
            }
            else
            {
                cboRoles.SelectedItem = roleId;
            }
        }

        private async Task InitializeActions(string userRole)
        {
            var actions = new List<Tuple<string, bool>>
            {
                new Tuple<string, bool>("Log In", true),
                new Tuple<string, bool>("Encode Sales Invoice", true),
                new Tuple<string, bool>("Encode Sales Invoice Return", true),
                new Tuple<string, bool>("Encode Purchase Order", true),
                new Tuple<string, bool>("Encode Purchase Order Return", true),
                new Tuple<string, bool>("View Sales Invoice History", true),
                new Tuple<string, bool>("View Sales Invoice History Per Customer", true),
                new Tuple<string, bool>("View Sales Invoice Return History", true),
                new Tuple<string, bool>("View Purchase Order History", true),
                new Tuple<string, bool>("View Purchase Order Return History", true),
                new Tuple<string, bool>("View Purchase Order History Per Supplier", true),
                new Tuple<string, bool>("Return Sales Invoice", true),
                new Tuple<string, bool>("Return Purchase Order", true),
                new Tuple<string, bool>("Delete Sales Invoice Return", true),
                new Tuple<string, bool>("Delete Purchase Order Return", true),
                new Tuple<string, bool>("Add Item", false),
                new Tuple<string, bool>("Edit Item", false),
                new Tuple<string, bool>("Delete Item", false),
                new Tuple<string, bool>("View Items", false),
                new Tuple<string, bool>("Change Item Price", false),
                new Tuple<string, bool>("Update Minimum Stock of Items", false),
                new Tuple<string, bool>("Add Category", false),
                new Tuple<string, bool>("Edit Category", false),
                new Tuple<string, bool>("Delete Category", false),
                new Tuple<string, bool>("View Categories", false),
                new Tuple<string, bool>("Add Customer", false),
                new Tuple<string, bool>("Edit Customer", false),
                new Tuple<string, bool>("Delete Customer", false),
                new Tuple<string, bool>("View Customers", false),
                new Tuple<string, bool>("Add Supplier", false),
                new Tuple<string, bool>("Edit Supplier", false),
                new Tuple<string, bool>("Delete Supplier", false),
                new Tuple<string, bool>("View Suppliers", false)
            };

            var privileges = await controller.GetPrivileges(userRole);

            dgvItems.Rows.Clear();

            foreach (var action in actions)
            {
                UserPrivilegeDtos privilege = null;

                var exists = privileges.Any(item => item.Action == action.Item1);

                if (exists) privilege = privileges.FirstOrDefault(item => item.Action == action.Item1);
                
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                if (exists)
                {
                    row.Tag = privilege.UserPrivilegeId;

                    row.Cells[0].Value = privilege.IsEnable;
                }
                else row.Cells[0].Value = userRole == "Encoder" ? action.Item2 : true;

                row.Cells[1].Value = action.Item1;

                row.Cells[1].ReadOnly = true;
            }
        }

        private async void UserRoleForm_Load(object sender, EventArgs e)
        {
            mainForm.ShowProgressStatus();

            try
            {
                await InitializeRoles();

                await InitializeActions(cboRoles.Text);

                started = true;
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void rbtnRoles_CheckedChanged(object sender, EventArgs e)
        {
            if (!started) return;

            mainForm.ShowProgressStatus();

            try
            {
                var rbtn = (RadioButton)sender;

                if (!rbtn.Checked) return;

                await InitializeActions(rbtn.Text);
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            btnConfirm.Focus();

            var result = mainForm.ShowMessage("Are you sure you want to save changes?", true);

            if (result == System.Windows.Forms.DialogResult.No) return;

            try
            {
                var userPrivileges = new List<UserPrivilegeDtos>();

                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    var privilegeId = 0;

                    var isEnable = false;

                    if (row.Tag != null) int.TryParse(row.Tag.ToString(), out privilegeId);

                    bool.TryParse(row.Cells[0].Value.ToString(), out isEnable);

                    userPrivileges.Add(new UserPrivilegeDtos
                    {
                        UserPrivilegeId = privilegeId,
                        IsEnable = isEnable,
                        Action = row.Cells[1].Value.ToString()
                    });
                }

                var success = await controller.SaveUserRole(new UserRoleDtos
                {
                    RoleName = pnlUserRole.Visible ? txtRoleName.Text : cboRoles.Text,
                    UserPrivilegeDtosList = userPrivileges
                });

                if (success)
                {
                    mainForm.ShowMessage("Successfully saved.");

                    string roleName = txtRoleName.Text;

                    if (!string.IsNullOrWhiteSpace(roleName))
                    {
                        await InitializeRoles(roleName);

                        pnlUserRole.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnAddUserRole_Click(object sender, EventArgs e)
        {
            pnlUserRole.Visible = true;

            txtRoleName.Focus();

            await InitializeActions("");
        }

        private void btnCancelUserRole_Click(object sender, EventArgs e)
        {
            pnlUserRole.Visible = false;
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }

        private void cboRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private async void cboRoles_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!started) return;

            mainForm.ShowProgressStatus();

            try
            {
                await InitializeActions(cboRoles.Text);
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            finally { mainForm.ShowProgressStatus(false); }
        }
    }
}
