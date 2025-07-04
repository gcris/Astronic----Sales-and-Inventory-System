using AstronicAutoSupplyInventory.Categories;
using AstronicAutoSupplyInventory.Customer;
using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Items;
using AstronicAutoSupplyInventory.Properties;
using AstronicAutoSupplyInventory.Shared;
using AstronicAutoSupplyInventory.Supplier;
using AstronicAutoSupplyInventory.Transaction.PurchaseOrder;
using AstronicAutoSupplyInventory.Transaction.SalesInvoice;
using AstronicAutoSupplyInventory.User;
using Infrastructure;
using Infrastructure.UserControls;
using InventoryServices.Controllers;
using InventoryServices.EventMessenger;
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

namespace AstronicAutoSupplyInventory
{
    public partial class MainForm : Form
    {
        private Form currentForm;
        private bool isLoading;
        private CommonLibrary.Dtos.UserDtos userDtos;

        private UserController userController = new UserController();
        private bool closeForm;
        private bool login;
        private bool logout;

        public NotifierEventMessenger NotifierEventMessenger;

        public CommonLibrary.Dtos.UserDtos UserDtos { get { return userDtos; } set { value = userDtos; } }

        public bool IsLoading { get { return isLoading; } set { value = isLoading; } }

        public MainForm()
        {
            InitializeComponent();
        }

        public void ChangeForm(Form currentForm, bool makeAsNew = false)
        {
            if (isLoading && this.currentForm != null && login) return;

            this.currentForm = currentForm;

            var existingForm = (Form)Application.OpenForms[currentForm.Name];

            if (existingForm != null)
            {
                if (makeAsNew) existingForm.Close();
                else
                {
                    existingForm.BringToFront();

                    lblFormTitle.Text = existingForm.Text;

                    return;
                }
            }

            if (currentForm.Text != "Purchase Order Per Supplier" &&
                currentForm.Text != "Price Inquiry")
            {
                currentForm.MdiParent = this;

                currentForm.Show();

                currentForm.BringToFront();
            }
            else currentForm.ShowDialog();

            currentForm.FormClosing += currentForm_FormClosing;

            lblFormTitle.Text = currentForm.Text;
        }

        private void currentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!e.Cancel && Application.OpenForms.Count > 2)
            {
                var form = Application.OpenForms[Application.OpenForms.Count - 2];

                lblFormTitle.Text = form.Text;
            }
        }

        private void InitializeMainForm()
        {
            if (userDtos.UserRole != "SystemDeveloper")
            {
                var userRoles = userDtos.UserPrivilegeDtosList;

                foreach (ToolStripMenuItem mainItem in menuStrip.Items)
                {
                    foreach (ToolStripMenuItem subMainItem in mainItem.DropDownItems.OfType<ToolStripMenuItem>())
                    {
                        if (string.IsNullOrWhiteSpace(subMainItem.AccessibleDescription)) continue;

                        var enable = subMainItem.AccessibleDescription == "Private" && userDtos.UserRole == "Admin";

                        if (!enable) enable = userRoles.Any(role => role.Action == subMainItem.AccessibleDescription && role.IsEnable);

                        subMainItem.Enabled = enable;

                        if (enable)
                        {
                            foreach (ToolStripMenuItem dropDownItem in subMainItem.DropDownItems.OfType<ToolStripMenuItem>())
                            {
                                if (string.IsNullOrWhiteSpace(dropDownItem.AccessibleDescription)) continue;

                                enable = dropDownItem.AccessibleDescription == "Private" && userDtos.UserRole == "Admin";

                                if (!enable) enable = userRoles.Any(role => role.Action == dropDownItem.AccessibleDescription && role.IsEnable);

                                dropDownItem.Enabled = enable;
                            }
                        }
                    }
                }
            }
            dashboardToolStripMenuItem.Visible = userDtos.UserRole == "SystemDeveloper" || userDtos.UserRole == "Admin";
            login = false;
            ChangeForm(new IndexForm());
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeLoginForm();

            if (NotifierEventMessenger == null) NotifierEventMessenger = new NotifierEventMessenger(NotifierInvoked);
        }

        private void NotifierInvoked(bool success)
        {
            var poForm = (PurchaseOrderEntryForm)Application.OpenForms["PurchaseOrderEntryForm"];

            if (poForm != null)
            {
                poForm.NotifierInvoked(success);
            }

            var salesInvoiceForm = (SalesInvoiceForm)Application.OpenForms["SalesInvoiceForm"];

            if (salesInvoiceForm != null)
            {
                salesInvoiceForm.NotifierInvoked(success);
            }

            var itemListForm = (ItemListForm)Application.OpenForms["ItemListForm"];

            if (itemListForm != null)
            {
                itemListForm.NotifierInvoked(success);
            }
        }

        public DialogResult ShowMessage(string message, bool question = false, bool error = false)
        {
            var floaterFormUI = (FloatForm)Application.OpenForms["FloatForm"];
            if (floaterFormUI != null) floaterFormUI.Close();
            var infoMsg = new Infragistics.Win.UltraMessageBox.UltraMessageBoxInfo(
                Infragistics.Win.UltraMessageBox.MessageBoxStyle.Standard, this, message, "AASIS App",
                question ? MessageBoxButtons.YesNo : MessageBoxButtons.OK,
                error ? MessageBoxIcon.Error : MessageBoxIcon.Information, 
                MessageBoxDefaultButton.Button1, Infragistics.Win.DefaultableBoolean.False);
            return ultraMessageBoxManager.ShowMessageBox(infoMsg);
        }

        public void HandleException(Exception ex)
        {
            ShowProgressStatus(false);

            var msg = ex.Message;

            if (ex.HResult == -2146233087) msg = "Operation failed. Selected items cannot be deleted because they are referenced to the other process.";
            else if (ex.HResult == -2146232060) msg = "Operation failed. Your server is not running. " +
                "Please contact the administrator for correction. Thank you.";
            else if (ex.HResult == -2146233079) msg = string.Empty;
            else
            {
                msg += "\n\n" + ex.StackTrace;
            }
            
            if (msg.Length > 0) ShowMessage(msg);
        }

        public void ShowProgressStatus(bool visible = true)
        {
            isLoading = visible;
            lblStatus.Text = visible ? "Loading, please wait..." : "Ready.";
            progressBar.Visible = visible;
            pnlStatus.BackColor = visible ? Color.Orange : Color.DodgerBlue;
        }

        private void InitializeLoginForm()
        {
            var logInUI = new UserLogInForm(new LogInUserEventMessenger(LogInInvoked));

            logInUI.ShowDialog();
        }

        private async void LogInInvoked(CommonLibrary.Dtos.UserDtos userDtos, bool onClosing = false)
        {
            login = true;

            if (userDtos != null)
            {
                this.userDtos = userDtos;
                var logInForm = (UserLogInForm)Application.OpenForms["UserLogInForm"];
                if (logInForm == null) return;

                InitializeMainForm();
                pnlLogInStatus.Visible = true;
                lblWelcomeNote.Text = string.Format("Welcome back, {0}", 
                    string.IsNullOrWhiteSpace(userDtos.FirstName) ? userDtos.Username : userDtos.FirstName);

                logInForm.Close();
                logInForm.Dispose();
                await userController.SaveActivity(
                    "User log in",
                    userDtos.UserId);
            }
            else Application.Exit();
        }

        private async void btnLogOut_Click(object sender, EventArgs e)
        {
            var result = ShowMessage("Are you sure you want to log out?", true);

            if (result == System.Windows.Forms.DialogResult.No) return;
            var forms = new List<Form>();
            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == "MainForm") continue;

                forms.Add(form);
            }

            foreach (Form form in forms) form.Close();
            // Hide Log in Status
            pnlLogInStatus.Visible = false;
            await userController.SaveActivity(
                "User log out",
                userDtos.UserId);
            logout = true;
            InitializeLoginForm();
        }

        private void newSalesInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new SalesInvoiceForm());
        }

        private void newPurchaseOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new PurchaseOrderEntryForm());
        }

        private void categoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new CategoryListForm());
        }

        private void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new ItemListForm());
        }

        private void salesInvoiceHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new SalesInvoiceHistoryForm());
        }

        private void salesReturnHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new SalesReturnHistoryForm());
        }

        private void wholeTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender; 

            var returnTransactionForm = new ReturnTransactionForm(menuItem.Text.Contains("Whole Sales Invoice"));

            returnTransactionForm.ShowDialog();
        }

        private void itemsOnSalesInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new SalesReturnItemsForm());
        }

        private void purchaseOrderHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new PurchaseOrderHistoryForm());
        }

        private void purchaseOrderReturnHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new PoReturnHistoryForm());
        }

        private void newItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new AddEditItemForm());
        }

        private void newCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newForm = new AddEditCategoryForm();

            newForm.ShowDialog();
        }

        private void newCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newForm = new AddEditCustomerForm(null);

            newForm.ShowDialog();
        }

        private void newSupplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newForm = new AddEditSupplierForm();

            newForm.ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.closeForm)
            {
                var result = ShowMessage("Are you sure you want to close the application?", true);

                this.closeForm = result == System.Windows.Forms.DialogResult.Yes;

                e.Cancel = !this.closeForm;

                if (!e.Cancel && !logout) btnLogOut_Click(sender, e);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.closeForm = true;
        }

        private void changeItemPriceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new ChangePriceForm(DateTime.MinValue));
        }

        private void updateItemMinimumStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new UpdateMinimumStockForm());
        }

        private void priceInquiryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var priceInquiry = (PriceInquiryForm)Application.OpenForms["PriceInquiryForm"];

            if (priceInquiry == null)
            {
                priceInquiry = new PriceInquiryForm();
            }

            priceInquiry.Show();

            priceInquiry.BringToFront();
        }

        private void customersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new CustomerListForm());
        }

        private void suppliersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new SupplierListForm());
        }

        private void inventorySummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new InventorySummaryReportForm());
        }

        private void salesSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new SalesSummaryReportForm());
        }

        private void puchaseOrderPerSupplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new PurchaseOrderPerSupplierForm());
        }

        private void myAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new MyAccountForm());
        }

        private void userRoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new UserRoleForm());
        }

        private void usersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new UserListForm());
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new ChangePasswordForm());
        }

        private void itemsOnPurchaseOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new PoReturnItemsForm());
        }

        private void userActivitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new UserActivityForm());
        }

        private void salesInvoicePerCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new SalesInvoicePerCustomerForm());
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeForm(new DashboardForm());
        }
    }
}
