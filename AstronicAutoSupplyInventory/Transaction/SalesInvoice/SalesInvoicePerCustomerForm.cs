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

namespace AstronicAutoSupplyInventory.Transaction.SalesInvoice
{
    public partial class SalesInvoicePerCustomerForm : Form
    {
        private SalesInvoiceController salesInvoiceController = new SalesInvoiceController();
        private CustomerController customerController = new CustomerController();

        private DateTime from, to;

        private readonly int customerId;

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public SalesInvoicePerCustomerForm(int customerId = 0)
        {
            this.customerId = customerId;

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
                case Keys.Alt | Keys.C:
                    cboCustomer.Focus();

                    return true;
                case Keys.Alt | Keys.D:
                    lnkSelectDateRange_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
                case Keys.Alt | Keys.X:
                    lnkCancelDateRange_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task InitializeCustomer()
        {
            var supplierDtosList = await customerController.GetAll();

            cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;

            cboCustomer.DataSource = supplierDtosList.ToList();

            cboCustomer.DisplayMember = "CustomerName";

            cboCustomer.ValueMember = "CustomerId";

            if (customerId > 0) cboCustomer.SelectedValue = customerId;
            else cboCustomer.SelectedIndex = 0;
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

        private async void SalesInvoicePerCustomerForm_Load(object sender, EventArgs e)
        {
            try
            {
                await InitializeCustomer();
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

                var customerId = (int)cboCustomer.SelectedValue;

                var salesInvoiceList = await salesInvoiceController.GetAllByCustomer(this.from, this.to, customerId);

                var customerDtos = await customerController.Find(customerId);

                var salesInvoiceDtosList = new List<SalesInvoiceDtos>();

                var salesInvoiceDetailDtosList = new List<SalesInvoiceDetailDtos>();

                foreach (var item in salesInvoiceList)
                {
                    salesInvoiceDetailDtosList.AddRange(item.SalesInvoiceDetailDtosList);

                    item.IncludeDetails = includeDetails;

                    salesInvoiceDtosList.Add(item);
                }

                lblFoundStatus.Visible = salesInvoiceList.Count() < 1;

                if (salesInvoiceList.Count() > 0)
                {
                    var sources = new List<ReportDataSource>
                    {
                        new ReportDataSource 
                        {
                            Name = "SalesInvoiceDtos", 
                            Value = salesInvoiceDtosList
                        },
                        new ReportDataSource 
                        {
                            Name = "CustomerDtos", 
                            Value = new List<CustomerDtos> { customerDtos }
                        }
                    };

                    var subSources = new List<ReportDataSource>
                    {
                        new ReportDataSource 
                        {
                            Name = "SalesInvoiceDetailDtos", 
                            Value = salesInvoiceDetailDtosList.Select(item => new 
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
                                    item.SalesInvoiceId
                                }).ToList()
                        }
                    };

                    var parameters = new List<ReportParameter>
                    {
                        new ReportParameter("DateRange", from.Date == to.Date ? from.ToShortDateString() : 
                            string.Format("{0} to {1}", from.ToShortDateString(), to.ToShortDateString()))
                    };

                    var printPreviewForm = new PrintPreviewForm(
                        "Sales Invoice History",
                        @"SalesInvoicePerCustomerSummaryReport.rdlc",
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
            lnkHelp.Visible = !pnlHelp.Visible;
        }

        private void cboCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar)) e.KeyChar = Char.ToUpper(e.KeyChar);
        }
    }
}
