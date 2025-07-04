using AstronicAutoSupplyInventory.Shared;
using CommonLibrary.Dtos;
using InventoryServices.Controllers;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AstronicAutoSupplyInventory.Transaction.PurchaseOrder
{
    public partial class PurchaseOrderPerSupplierForm : Form
    {
        private SupplierController supplierController = new SupplierController();
        private PurchaseOrderController poController = new PurchaseOrderController();

        private DateTime from, to;

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private readonly int purchaseOrderId;

        public PurchaseOrderPerSupplierForm(int purchaseOrderId = 0)
        {
            this.purchaseOrderId = purchaseOrderId;

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
                case Keys.Alt | Keys.S:
                    cboSupplier.Focus();

                    return true;
                case Keys.Alt | Keys.D:
                    lnkSelectDateRange_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
                case Keys.Alt | Keys.C:
                    lnkCancelDateRange_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task InitializeSupplier()
        {
            var supplierDtosList = await supplierController.GetAll();

            cboSupplier.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            cboSupplier.AutoCompleteSource = AutoCompleteSource.ListItems;

            cboSupplier.DataSource = supplierDtosList.ToList();

            cboSupplier.DisplayMember = "Company";

            cboSupplier.ValueMember = "SupplierId";

            if (purchaseOrderId > 0) cboSupplier.SelectedValue = purchaseOrderId;
            else cboSupplier.SelectedIndex = 0;
        }

        private void lnkSelectDateRange_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var dateRangeForm = new DateRangeForm(new EventMesseging.ConfirmDataRangeEventMessenger(ConfirmDateRangeInvoked), from, to);

            dateRangeForm.ShowDialog();
        }

        private void ConfirmDateRangeInvoked(DateTime from, DateTime to)
        {
            this.from = from;

            this.to = to;

            var withDateRange = from > DateTime.MinValue && to > DateTime.MinValue;

            lblDateStatus.Text = withDateRange ? 
                string.Format("Transaction from {0} - {1}.",
                    from.ToShortDateString(),
                    to.ToShortDateString()) :
                "All Transaction.";

            lnkCancelDateRange.Visible = withDateRange;

            var dateRangeForm = (DateRangeForm)Application.OpenForms["DateRangeForm"];

            if (dateRangeForm != null) dateRangeForm.Close();
        }

        private async void PurchaseOrderPerSupplierForm_Load(object sender, EventArgs e)
        {
            try
            {
                await InitializeSupplier();
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnkCancelDateRange_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ConfirmDateRangeInvoked(DateTime.MinValue, DateTime.MinValue);
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            try
            {
                mainForm.ShowProgressStatus();

                var includeDetails = chkIncludeDetail.Checked;

                var supplierId = (int)cboSupplier.SelectedValue;

                var purchaseOrderList = await poController.GetAllBySupplier(this.from, this.to, supplierId);

                var supplierDtos = await supplierController.Find(supplierId);

                var purchaseOrdeDtosList = new List<PurchaseOrderDtos>();

                var purchaseOrderDetailDtosList = new List<PurchaseOrderDetailDtos>();

                foreach (var item in purchaseOrderList)
                {
                    purchaseOrderDetailDtosList.AddRange(item.PurchaseOrderDetailDtosList);

                    item.IncludeDetails = includeDetails;

                    purchaseOrdeDtosList.Add(item);
                }

                lblFoundStatus.Visible = purchaseOrderList.Count() < 1;

                if (purchaseOrderList.Count() > 0)
                {
                    var sources = new List<ReportDataSource>
                    {
                        new ReportDataSource 
                        {
                            Name = "PurchaseOrderDtos", 
                            Value = purchaseOrdeDtosList
                        },
                        new ReportDataSource 
                        {
                            Name = "SupplierDtos", 
                            Value = new List<SupplierDtos> { supplierDtos }
                        }
                    };

                    var subSources = new List<ReportDataSource>
                    {
                        new ReportDataSource 
                        {
                            Name = "PurchaseOrderDetailDtos", 
                            Value = purchaseOrderDetailDtosList.Select(item => new 
                                {
                                    BrandName = item.ItemDtos.BrandName,
                                    Made = item.ItemDtos.Made,
                                    Make = item.ItemDtos.Make,
                                    Model = item.ItemDtos.Model,
                                    PartNo = item.ItemDtos.PartNo,
                                    Size = item.ItemDtos.Size,
                                    CategoryName = item.ItemDtos.CategoryName,
                                    item.Quantity,
                                    item.TotalAmount,
                                    item.UnitPrice,
                                    item.PurchaseOrderId
                                }).ToList()
                        }
                    };

                    var parameters = new List<ReportParameter>
                    {
                        new ReportParameter("DateRange", from.Date == to.Date ? from.ToShortDateString() : 
                            string.Format("{0} to {1}", from.ToShortDateString(), to.ToShortDateString()))
                    };

                    var printPreviewForm = new PrintPreviewForm(
                        "Purchase Order History",
                        @"SupplierHistorySummaryReport.rdlc",
                        sources,
                        includeDetails ? subSources : null,
                        parameters);

                    printPreviewForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }

        private void cboSupplier_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar)) e.KeyChar = Char.ToUpper(e.KeyChar);
        }
    }
}
