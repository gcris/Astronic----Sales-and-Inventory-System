using AstronicAutoSupplyInventory.EventMesseging;
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

namespace AstronicAutoSupplyInventory.Customer
{
    public partial class AddEditCustomerForm : Form
    {
        private int id;

        private readonly AddNewEventMessenger addNewCustomerEventMessenger;
        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        private CustomerController customerController = new CustomerController();
        private UserController userController = new UserController();

        public AddEditCustomerForm(AddNewEventMessenger addNewCustomerEventMessenger = null, int id = 0)
        {
            this.id = id;

            this.addNewCustomerEventMessenger = addNewCustomerEventMessenger;

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

        private async Task InitializeCustomer()
        {
            if (id < 1) return;

            var query = await customerController.Find(id);

            if (query == null) return;

            txtAddress.Text = query.Address;

            txtCustomerName.Text = query.CustomerName;

            txtContactNo.Text = query.ContactNo;
        }

        private bool IsValid()
        {
            var msg = "";

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text.Trim()))
            {
                msg = "Customer's Name is required.";

                txtCustomerName.Focus();
            }

            if (msg.Length > 0) mainForm.ShowMessage(msg);

            return msg.Length == 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void AddEditCustomer_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                await InitializeCustomer();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!IsValid()) return;

            var result = mainForm.ShowMessage("Are you sure you want to save changes?", true);

            if (result == System.Windows.Forms.DialogResult.No) return;

            try
            {
                var customerDtos = new CustomerDtos
                {
                    CustomerId = this.id,
                    CustomerName = txtCustomerName.Text.Trim(),
                    Address = txtAddress.Text,
                    ContactNo = txtContactNo.Text
                };

                var customerId = await customerController.Save(customerDtos);

                if (customerId > 0)
                {
                    await userController.SaveActivity(
                        string.Format(id < 1 ? "Creates new Customer '{0}'" : "Updates Customer '{0}'", txtCustomerName.Text),
                        mainForm.UserDtos.UserId);

                    mainForm.ShowMessage("Successfully saved.");
                    //MessageBox.Show(this, "Successfully saved.", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (addNewCustomerEventMessenger != null) addNewCustomerEventMessenger(customerId);

                    this.Close();
                }
            }
            catch (Exception ex) 
            {
                mainForm.HandleException(ex);
                //MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}
