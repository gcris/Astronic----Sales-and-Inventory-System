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

namespace AstronicAutoSupplyInventory.Shared
{
    public partial class ReturnTransactionForm : Form
    {
        private SalesInvoiceController salesInvoiceController = new SalesInvoiceController();
        private SalesReturnController salesReturnController = new SalesReturnController();
        private PurchaseOrderController purchaseOrderController = new PurchaseOrderController();
        private PurchaseOrderReturnController purchaseOrderReturnController = new PurchaseOrderReturnController();
        private UserController userController = new UserController();

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        private bool salesInvoice;

        public ReturnTransactionForm(bool salesInvoice = true)
        {
            this.salesInvoice = salesInvoice;

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

        private void EnterOrNumberForm_Load(object sender, EventArgs e)
        {
            lblRequirement.Text = salesInvoice ?
                "O.R. Number is required" :
                "P.O. Number is required";

            lblORNumber.Text = salesInvoice ?
                "O.R. Number:" :
                "P.O. Number:";

            this.Text = salesInvoice ? "Return Sales Invoice" : "Return Purchase Order";

            txtOrNumber.Focus();
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            try
            {
                mainForm.ShowProgressStatus();

                var orExists = false;

                var date = DateTime.MinValue;

                if (!string.IsNullOrWhiteSpace(txtOrNumber.Text) && salesInvoice)
                {
                    var salesInvoiceDtos = await salesInvoiceController.Find(txtOrNumber.Text.Trim());

                    if (salesInvoiceDtos != null)
                    {
                        txtOrNumber.Text = salesInvoiceDtos.ORNumber;

                        date = salesInvoiceDtos.Date;
                    }

                    orExists = salesInvoiceDtos != null;
                }
                else
                {
                    var poDtos = await purchaseOrderController.Find(txtOrNumber.Text.Trim());

                    if (poDtos != null)
                    {
                        txtOrNumber.Text = poDtos.PONumber;

                        date = poDtos.Date;
                    }

                    orExists = poDtos != null;
                }

                lblStatus.Text = orExists ? string.Format("Invoice Date: {0}", date.ToShortDateString()) :
                    string.Format("{0} Number not exists.", salesInvoice ? "O.R." : "P.O.");

                lblStatus.ForeColor = orExists ? Color.Green : Color.Red;

                if (orExists)
                {
                    mainForm.ShowProgressStatus(false);

                    var enterRemarksForm = new EnterRemarksForm(new ConfirmRemarksEventMessenger(ConfirmRemarksInvoked),
                        txtOrNumber.Text.Trim(), null);

                    enterRemarksForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void ConfirmRemarksInvoked(string orNumber, string remarks, DateTime date, DataGridViewRow row)
        {
            if (mainForm.IsLoading) return;

            try 
            {
                mainForm.ShowProgressStatus();

                var result = mainForm.ShowMessage("Are you sure yo want to return?", true);

                if (result == DialogResult.No) return;

                var enterRemarksForm = (EnterRemarksForm)Application.OpenForms["EnterRemarksForm"];

                enterRemarksForm.Close();

                enterRemarksForm = null;

                var success = false;

                var userId = mainForm.UserDtos.UserId;

                if (salesInvoice)
                {
                    var salesDtos = await salesInvoiceController.Find(orNumber);

                    salesDtos.UserId = userId;

                    success = await salesReturnController.Save(
                        null,
                        date,
                        salesDtos,
                        remarks);

                    if (success)
                    {
                        foreach (var item in salesDtos.SalesInvoiceDetailDtosList)
                        {
                            await userController.SaveItemActivity(
                                new UserActivityDtos
                                {
                                    Action = "Sales Return (IN-RETURNED)",
                                    CurrentStock = item.CurrentStock,
                                    Date = DateTime.Now,
                                    ItemId = item.ItemDtos.ItemId,
                                    Quantity = item.Quantity,
                                    Amount = item.TotalAmount,
                                    ReferenceNumber = salesDtos.ORNumber,
                                    UserId = mainForm.UserDtos.UserId
                                });
                        }
                    }
                }
                else
                {
                    var poDtos = await purchaseOrderController.Find(orNumber);

                    poDtos.UserId = userId;

                    success = await purchaseOrderReturnController.Save(
                        null,
                        date,
                        poDtos,
                        remarks);

                    if (success)
                    {
                        foreach (var item in poDtos.PurchaseOrderDetailDtosList)
                        {
                            await userController.SaveItemActivity(
                                new UserActivityDtos
                                {
                                    Action = "Purchase Order Return (OUT-RETURNED)",
                                    CurrentStock = item.CurrentStock,
                                    Date = DateTime.Now,
                                    ItemId = item.ItemDtos.ItemId,
                                    Quantity = item.Quantity,
                                    Amount = item.TotalAmount,
                                    ReferenceNumber = poDtos.PONumber,
                                    UserId = mainForm.UserDtos.UserId
                                });
                        }
                    }
                }

                this.Close();
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
