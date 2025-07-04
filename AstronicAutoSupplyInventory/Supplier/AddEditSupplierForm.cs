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

namespace AstronicAutoSupplyInventory.Supplier
{
    public partial class AddEditSupplierForm : Form
    {
        private int id;

        private readonly AddNewEventMessenger addNewCustomerEventMessenger;

        private SupplierController supplierController = new SupplierController();
        private UserController userController = new UserController();

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public AddEditSupplierForm(AddNewEventMessenger addNewCustomerEventMessenger = null, int id = 0)
        {
            this.id = id;

            this.addNewCustomerEventMessenger = addNewCustomerEventMessenger;

            InitializeComponent();
        }
        
        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.F1:
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

        private bool IsValid()
        {
            var msg = "";

            if (string.IsNullOrWhiteSpace(txtContactPerson.Text.Trim()))
            {
                msg = "Contact Person Name is required.";

                txtContactPerson.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtCompanyName.Text.Trim()))
            {
                msg = "Company Name is required.";

                txtCompanyName.Focus();
            }

            if (msg.Length > 0) mainForm.ShowMessage(msg);

            return msg.Length == 0;
        }

        private async Task InitializeSupplier()
        {
            if (id < 1) return;

            var query = await supplierController.Find(id);

            if (query == null) return;

            txtAddress.Text = query.Address;

            txtContactPerson.Text = query.ContactPerson;

            txtContactNo.Text = query.ContactNo;

            txtCompanyName.Text = query.ContactNo;
        }

        private async void AddEditSupplierForm_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                await InitializeSupplier();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!IsValid() || mainForm.IsLoading) return;

            var result = mainForm.ShowMessage("Are you sure you want to save changes?", true);

            if (result == System.Windows.Forms.DialogResult.No) return;

            try
            {
                mainForm.ShowProgressStatus();

                var supplierDtos = new SupplierDtos
                {
                    SupplierId = this.id,
                    ContactPerson = txtContactPerson.Text.Trim(),
                    Address = txtAddress.Text,
                    ContactNo = txtContactNo.Text,
                    Company = txtCompanyName.Text
                };

                var customerId = await supplierController.Save(supplierDtos);

                if (customerId > 0)
                {
                    await userController.SaveActivity(
                        string.Format(id < 1 ? "Creates new Supplier '{0}'" : "Updates Category '{0}'", txtCompanyName.Text),
                        mainForm.UserDtos.UserId);

                    mainForm.ShowMessage("Successfully saved.");

                    if (addNewCustomerEventMessenger != null) addNewCustomerEventMessenger(customerId);

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
            if (mainForm.IsLoading) return;

            this.Close();
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }
    }
}
