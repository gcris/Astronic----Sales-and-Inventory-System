using CommonLibrary.Dtos;
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
    public partial class AddEditUserForm : Form
    {
        private UserController userController = new UserController();

        private readonly int id;
        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        private string currentUsername;

        public AddEditUserForm(int id = 0)
        {
            this.id = id;

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

        private async Task InitializeRoles()
        {
            var list = await userController.GetAllRoles();

            cboRoles.AutoCompleteMode = AutoCompleteMode.None;

            cboRoles.AutoCompleteSource = AutoCompleteSource.ListItems;

            cboRoles.DataSource = list.ToList(); ;

            cboRoles.DisplayMember = "RoleName";

            cboRoles.ValueMember = "UserRoleId";

            cboRoles.SelectedIndex = 0;
        }

        private async Task InitializeUser()
        {
            var queryUser = await userController.FindUser(id);

            if (queryUser == null) return;

            this.Text = "Edit User";

            txtUsername.Text = queryUser.Username;

            currentUsername = queryUser.Username;
            //txtPassword.Text = queryUser.Password;

            chkEnableUser.Checked = queryUser.IsEnable;

            txtLastName.Text = queryUser.LastName;

            txtFirstName.Text = queryUser.FirstName;

            txtContactNo.Text = queryUser.ContactNumber;

            txtAddress.Text = queryUser.Address;

            cboRoles.SelectedValue = queryUser.UserRole;
        }

        private async Task<bool> IsValid()
        {
            var msg = "";

            var validUsername = false;

            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
                validUsername = await userController.IsValidUsername(txtUsername.Text, currentUsername);

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                msg = "Username is required.";

                txtUsername.Focus();
            }
            else if (!validUsername)
            {
                msg = "Username already in used.";

                txtUsername.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                msg = "Password is required.";

                txtUsername.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                msg = "Confirm Password is required.";

                txtConfirmPassword.Focus();
            }
            else if (txtPassword.Text.Length < 6)
            {
                msg = "Password must atleast 6 characters long.";

                txtPassword.Focus();
            }
            else if (txtPassword.Text != txtConfirmPassword.Text)
            {
                msg = "Password must be match with Confirm Password";

                txtConfirmPassword.Focus();
            }
            else if (cboRoles.SelectedValue == null)
            {
                msg = "User role is required.";

                cboRoles.Focus();
            }

            if (msg.Length > 0) mainForm.ShowMessage(msg);

            return msg.Length == 0;
        }

        private async void AddEditUser_Load(object sender, EventArgs e)
        {
            mainForm.ShowProgressStatus();

            try
            {
                await InitializeRoles();

                await InitializeUser();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            if (!await IsValid()) return;

            var result = mainForm.ShowMessage("Are you sure you want to save?", true);

            if (result == DialogResult.No) return;

            mainForm.ShowProgressStatus();

            try
            {
                var userDtos = new UserDtos
                {
                    Address = txtAddress.Text,
                    ContactNumber = txtContactNo.Text,
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    Username = txtUsername.Text,
                    Password = txtPassword.Text,
                    UserRole = cboRoles.Text,
                    UserId = id,
                    IsEnable = chkEnableUser.Checked
                };

                var success = await userController.Save(userDtos);

                if (success)
                {
                    mainForm.ShowMessage("Successfully saved.");

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }
    }
}
