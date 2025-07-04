using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infrastructure.EventMessenger;
using AstronicAutoSupplyInventory.Properties;
using AstronicAutoSupplyInventory.Transaction.SalesInvoice;
using AstronicAutoSupplyInventory.Transaction.PurchaseOrder;
using AstronicAutoSupplyInventory.Items;
using AstronicAutoSupplyInventory.Customer;
using AstronicAutoSupplyInventory.Categories;
using AstronicAutoSupplyInventory.User;
using AstronicAutoSupplyInventory.EventMesseging;
using System.Diagnostics;

namespace AstronicAutoSupplyInventory.Shared
{
    public partial class MenuButtonUI : UserControl
    {
        private readonly string formName;
        private readonly Image backgroundImage;
        private readonly ChangeUserControlEventMessenger changeUserControlEventMessenger;
        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private bool isButtonActive;

        private List<string> formNameList;
        private string mainFormName;

        public MenuButtonUI(ChangeUserControlEventMessenger changeUserControlEventMessenger,
            string formName, string mainForm, List<string> formList, Image backgroundImage)
        {
            this.changeUserControlEventMessenger = changeUserControlEventMessenger;

            this.formName = formName;

            this.mainFormName = mainForm;

            this.backgroundImage = backgroundImage;

            this.formNameList = formList;

            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e)
        {
            pnlBackground.BackColor = Color.MistyRose;

            pnlLeftStatus.Visible = true;

            if (formNameList == null)
            {
                var form = (Form)Application.OpenForms[this.mainFormName];

                if (form == null)
                {
                    if (this.mainFormName == "DashboardForm") form = new DashboardForm();
                    else if (this.mainFormName == "SalesInvoiceForm") form = new SalesInvoiceForm();
                    else if (this.mainFormName == "PurchaseOrderEntryForm") form = new PurchaseOrderEntryForm();
                    else if (this.mainFormName == "CategoryListForm") form = new CategoryListForm();
                    else if (this.mainFormName == "CustomerListForm") form = new CustomerListForm();
                    else if (this.mainFormName == "PriceInquiryForm") form = new PriceInquiryForm();
                }

                changeUserControlEventMessenger(form);

                pnlBackground.BackColor = Color.Transparent;

                pnlLeftStatus.Visible = false;
            }
            else
            {
                isButtonActive = true;

                mnuMenu.Show(pnlBackground, 0, pnlBackground.Height);
            }
        }

        private void MenuButton_MouseHover(object sender, EventArgs e)
        {
            if (isButtonActive) return;

            pnlBackground.BackColor = Color.MistyRose;

            pnlLeftStatus.Visible = true;

            //pnlKey.Visible = true;
        }

        private void MenuButton_MouseLeave(object sender, EventArgs e)
        {
            if (isButtonActive) return;

            pnlBackground.BackColor = Color.Transparent;

            pnlLeftStatus.Visible = false;

            //pnlKey.Visible = false;
        }

        private void MenuButton_Load(object sender, EventArgs e)
        {
            lblButtonName.Text = formName;

            if (backgroundImage != null) picLogo.Image = backgroundImage;

            if (formNameList != null)
            {
                foreach (var control in formNameList)
                {
                    var form = GetForm(control);

                    var itemMenu = new ToolStripMenuItem
                    {
                        Font = new Font("Arial", 12f),
                        BackColor = Color.AntiqueWhite,
                        Image = Resources.point_right_32x32,
                        Padding = new Padding(5, 2, 5, 2),
                        Text = form.Text,
                        Name = control
                    };

                    mnuMenu.Items.Add(itemMenu);

                    itemMenu.Click += itemMenu_Click;
                }
            }
        }

        private Form GetForm(string currentForm)
        {
            var form = (Form)Application.OpenForms[currentForm];

            if (form == null)
            {
                if (currentForm == "AddEditItemForm") form = new AddEditItemForm();
                else if (currentForm == "ItemListForm") form = new ItemListForm();
                else if (currentForm == "UpdateMinimumStockForm") form = new UpdateMinimumStockForm();
                else if (currentForm == "ChangePriceForm") form = new ChangePriceForm(DateTime.MinValue);
                else if (currentForm == "SalesInvoiceHistoryForm") form = new SalesInvoiceHistoryForm();
                else if (currentForm == "SalesReturnHistoryForm") form = new SalesReturnHistoryForm();
                else if (currentForm == "PurchaseOrderHistoryForm") form = new PurchaseOrderHistoryForm();
                else if (currentForm == "PoReturnHistoryForm") form = new PoReturnHistoryForm();
                else if (currentForm == "InventorySummaryReportForm") form = new InventorySummaryReportForm();
                else if (currentForm == "SalesSummaryReportForm") form = new SalesSummaryReportForm();
                else if (currentForm == "PurchaseOrderPerSupplierForm") form = new PurchaseOrderPerSupplierForm();
                else if (currentForm == "MyAccountForm") form = new MyAccountForm();
                else if (currentForm == "ChangePasswordForm") form = new ChangePasswordForm();
                else if (currentForm == "UserActivityForm") form = new UserActivityForm();
                else if (currentForm == "AddEditUserForm") form = new AddEditUserForm();
                else if (currentForm == "UserRoleForm") form = new UserRoleForm();
                else if (currentForm == "UserListForm") form = new UserListForm();
            }

            return form;
        }

        private void itemMenu_Click(object sender, EventArgs e)
        {
            var itemMenu = (ToolStripMenuItem)sender;

            if (itemMenu == null) return;

            var currentForm = formNameList.FirstOrDefault(item => item == itemMenu.Name);

            var form = GetForm(currentForm);

            changeUserControlEventMessenger(form);
        }

        private void mnuMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            isButtonActive = false;

            MenuButton_MouseLeave(sender, e);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //timer.Interval += 100;

            //if (timer.Interval >= 800)
            //{
            //    pnlKey.Visible = false;

            //    timer.Stop();

            //    timer.Enabled = false;

            //    timer.Interval = 100;
            //}
        }
    }
}
