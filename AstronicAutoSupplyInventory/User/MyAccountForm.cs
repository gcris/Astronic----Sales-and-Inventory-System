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
    public partial class MyAccountForm : Form
    {
        private UserController userController = new UserController();

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private string currentUsername;

        public MyAccountForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Enter:
                    btnConfirm_Click(this, new EventArgs());

                    return true;
                case Keys.F1:
                    btnChangePassword_Click(this, new EventArgs());

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

        private void InitializeUser()
        {
            var userDtos = mainForm.UserDtos;

            currentUsername = userDtos.Username;

            txtUsername.Text = userDtos.Username;

            txtLastName.Text = userDtos.LastName;

            txtFirstName.Text = userDtos.FirstName;

            txtContactNo.Text = userDtos.ContactNumber;

            txtAddress.Text = userDtos.Address;
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

            if (msg.Length > 0) mainForm.ShowMessage(msg);

            return msg.Length == 0;
        }

        private void MyAccountForm_Load(object sender, EventArgs e)
        {
            InitializeUser();
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
                    UserId = mainForm.UserDtos.UserId
                };

                var success = await userController.UpdateAccount(userDtos);

                if (success) mainForm.ShowMessage("Successfully saved.");
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

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            var changePassword = new ChangePasswordForm();

            changePassword.ShowDialog();
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }
    }
}
