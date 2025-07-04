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
    public partial class ChangePasswordForm : Form
    {
        private UserController controller = new UserController();

        private MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        private UserDtos userDtos;

        public ChangePasswordForm()
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
                case Keys.Escape:
                    btnCancel_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task<bool> IsValid()
        {
            var existingUser = await controller.LogIn(userDtos.Username, txtPassword.Text);

            var msg = "";

            var passwordMatched = true;

            if (existingUser == null)
            {
                msg = "Old Password does not match.";
                txtPassword.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                msg = "Password is required.";
                txtPassword.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                msg = "Confirm Password is required.";
                txtNewPassword.Focus();
            }
            else if (txtNewPassword.Text.Length < 6)
            {
                msg = "New Password must atleast 6 characters long.";

                txtNewPassword.Focus();
            }
            else if (txtNewPassword.Text != txtConfirmNewPassword.Text)
            {
                msg = "Confirm Password must be match with Password.";

                txtConfirmNewPassword.Focus();

                passwordMatched = false;
            }

            if (passwordMatched && msg.Length < 1)
            {
                var validPassword = await controller.IsValidPassword(userDtos.UserId, txtNewPassword.Text);

                if (validPassword)
                {
                    msg = "You can't reuse your old password.";

                    txtNewPassword.Focus();
                }
            }

            if (msg.Length > 0)
            {
                mainForm.ShowMessage(msg);
            }

            return msg.Length == 0;
        }

        private void ChangePasswordForm_Load(object sender, EventArgs e)
        {
            userDtos = mainForm.UserDtos;
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!await IsValid()) return;

            var result = mainForm.ShowMessage("Are you sure you want to save changes?", true);

            if (result == DialogResult.No) return;

            var success = false;

            try
            {
                mainForm.ShowProgressStatus();

                success = await controller.ChangePassword(userDtos.UserId, txtNewPassword.Text);
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            finally
            {
                mainForm.ShowMessage(success ? "Successfully updated" : "Failed to save. Please contact your developer.");

                mainForm.ShowProgressStatus(false);

                if (success) this.Close();
            }
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
