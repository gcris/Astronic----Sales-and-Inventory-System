using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Shared;
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
    public partial class UserLogInForm : Form
    {
        private UserController userController = new UserController();

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private readonly LogInUserEventMessenger logInUserEventMessenger;

        private UserDtos userDtos;
        //private bool priceInquiry;

        public UserLogInForm(LogInUserEventMessenger logInUserEventMessenger)
        {
            this.logInUserEventMessenger = logInUserEventMessenger;

            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Escape:
                    var result = mainForm.ShowMessage("Are you sure you want to exit?", true);

                    if (result == System.Windows.Forms.DialogResult.No) break;

                    Application.Exit();

                    return true;
                case Keys.Enter:
                    btnLogIn_Click(this, new EventArgs());

                    return true;
            }
            return base.ProcessCmdKey(ref message, keys);
        }

        private bool IsValid()
        {
            var msg = "";

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                msg = "Username is required.";

                txtUsername.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                msg = "Password is required.";

                txtPassword.Focus();
            }

            if (msg.Length > 0) mainForm.ShowMessage(msg);

            return msg.Length == 0;
        }

        private async void btnLogIn_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            if (!IsValid()) return;

            mainForm.ShowProgressStatus();
            btnLogIn.Text = "Logging in...";
            await Task.Delay(100);

            try
            {
                //CryptographyDtos.
                this.userDtos = await userController.LogIn(txtUsername.Text, txtPassword.Text);

                if (userDtos == null) mainForm.ShowMessage("Username and/or Password is not valid");
                else logInUserEventMessenger(this.userDtos);
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally
            {
                btnLogIn.Text = "Log In";

                mainForm.ShowProgressStatus(false);
            }
        }

        private void UserLogIn_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.userDtos != null) return;

            e.Cancel = mainForm.IsLoading;

            if (!mainForm.IsLoading) logInUserEventMessenger(this.userDtos);
        }

        private void btnPriceInquiry_Click(object sender, EventArgs e)
        {
            //priceInquiry = true;
            var priceInquiryForm = new PriceInquiryForm();
            priceInquiryForm.ShowDialog();
        }

        private void UserLogInForm_Load(object sender, EventArgs e)
        {
            //priceInquiry = false;
        }
    }
}
