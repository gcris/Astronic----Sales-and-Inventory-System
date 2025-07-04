using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Transaction.SalesInvoice;
using CommonLibrary.Dtos;
using InventoryServices.Controllers;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AstronicAutoSupplyInventory.Shared
{
    public partial class SalesSummaryReportForm : Form
    {
        private SalesInvoiceController salesInvoiceController = new SalesInvoiceController();
        private CategoryController categoryController = new CategoryController();
        private CustomerController customerController = new CustomerController();
        private UserController userController = new UserController();

        private DateTime from, to;

        private bool started;

        List<ReportDataSource> subSources;

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private IEnumerable<CategoryDtos> categoryList;
        private int categoryId;
        private SearchKeyEventMessenger searchKeyEventMessenger;
        private ProcessCmdEventMessenger listViewEventMessenger;
        private ProcessCmdEventMessenger confirmSelectionEventMessenger;
        private bool isConfirmSelection;

        public SalesSummaryReportForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Escape:
                    if (pnlSelectCategory.Visible)
                    {
                        pnlSelectCategory.Visible = false;

                        return true;
                    }

                    if (mainForm.IsLoading) break;

                    this.Close();

                    return true;
                case Keys.Down:
                    if (pnlSelectCategory.Visible)
                    {
                        return listViewEventMessenger();
                    }

                    return true;
                case Keys.Back:
                    if (pnlSelectCategory.Visible)
                    {
                        if (txtCategory.Focused) return false;

                        txtCategory.Focus();

                        if (txtCategory.Text.Length > 0)
                        {
                            txtCategory.Text = txtCategory.Text.Substring(0, txtCategory.Text.Length - 1);
                        }

                        return true;
                    }

                    break;
                case Keys.Enter:
                    if (pnlSelectCategory.Visible)
                    {
                        return confirmSelectionEventMessenger();
                    }

                    break;
                case Keys.Tab:
                    if (pnlSelectCategory.Visible)
                    {
                        var msg = txtCategory.Text;

                        if (msg.Length == 0 || msg == "SELECT CATEGORY") return true;

                        return confirmSelectionEventMessenger();
                    }

                    break;
                case Keys.Alt | Keys.T:
                    cboReport.Focus();

                    return true;
                case Keys.Alt | Keys.C:
                    txtCategory.Focus();

                    return true;
                case Keys.Alt | Keys.D:
                    lnkChangeDate_LinkClicked(lnkChangeDate, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async void SalesSummaryReportForm_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                cboReport.SelectedIndex = 0;

                await InitializeCategories();

                //await InitializeCustomer();

                started = true;

                from = DateTime.Now.Date;

                to = DateTime.Now.Date;

                lblDate.Text = from == to ?
                    string.Format("{0}", from.ToShortDateString()) :
                    string.Format("{0} to {1}", from.ToShortDateString(), to.ToShortDateString());
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        //private async Task InitializeCustomer()
        //{
        //    var queryCustomerList = await customerController.GetAll();

        //    cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

        //    cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;

        //    if (queryCustomerList.Count() > 0)
        //    {
        //        var list = new List<CustomerDtos>();

        //        list.AddRange(queryCustomerList);

        //        list.Add(new CustomerDtos { CustomerName = "ALL CUSTOMER", CustomerId = 0 });

        //        cboCustomer.DataSource = list;

        //        cboCustomer.DisplayMember = "CustomerName";

        //        cboCustomer.ValueMember = "CustomerId";

        //        cboCustomer.SelectedValue = 0;
        //    }
        //}

        private async Task InitializeCategories()
        {
            var categoryList = await categoryController.GetAll();

            this.categoryList = categoryList;
        }

        private void ConfirmDateInvoked(DateTime from, DateTime to)
        {
            this.from = from;

            this.to = to;

            lblDate.Text = from == to ?
                string.Format("{0}", from.ToShortDateString()) :
                string.Format("{0} to {1}", from.ToShortDateString(), to.ToShortDateString());

            var dateRangeForm = (DateRangeForm)Application.OpenForms["DateRangeForm"];

            if (dateRangeForm != null) dateRangeForm.Close();
        }

        private void lnkChangeDate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!started || mainForm.IsLoading) return;

            var dateRangeForm = new DateRangeForm(new EventMesseging.ConfirmDataRangeEventMessenger(ConfirmDateInvoked), from, to);

            dateRangeForm.ShowDialog();
        }

        private void ShowReport(string title, string reportFile, List<ReportDataSource> sources, List<ReportDataSource> subSources, List<ReportParameter> parameters = null)
        {
            this.subSources = subSources;

            pnlReport.Controls.Clear();

            var reportViewer = new ReportViewer();

            reportViewer.RefreshReport();

            lblTitle.Text = title;

            reportViewer.ProcessingMode = ProcessingMode.Local;

            reportViewer.LocalReport.ReportEmbeddedResource = "AstronicAutoSupplyInventory.Reports." + reportFile;

            sources.ForEach(source => reportViewer.LocalReport.DataSources.Add(source));

            if (subSources != null) // means the report contains sub report
                reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessing);

            reportViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);

            if (parameters != null) reportViewer.LocalReport.SetParameters(parameters);

            reportViewer.RefreshReport();

            pnlReport.Controls.Add(reportViewer);

            reportViewer.ReportExport += reportViewer_ReportExport;

            reportViewer.Print += reportViewer_Print;

            reportViewer.Dock = DockStyle.Fill;

            reportViewer.ZoomMode = ZoomMode.PageWidth;
        }

        private async void reportViewer_Print(object sender, ReportPrintEventArgs e)
        {
            if (!e.Cancel)
            {
                await userController.SaveActivity(
                    string.Format("Prints file '{0}'", lblTitle.Text),
                    mainForm.UserDtos.UserId);
            }
        }

        private async void reportViewer_ReportExport(object sender, ReportExportEventArgs e)
        {
            if (!e.Cancel)
            {
                await userController.SaveActivity(
                    string.Format("Exports file '{0}' as '{1}'", lblTitle.Text, e.Extension.Name),
                    mainForm.UserDtos.UserId);
            }
        }

        private void SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            subSources.ForEach(source => e.DataSources.Add(source));
        }

        private async void btnGo_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !started) return;

            var hasRecord = true;

            if (cboReport.SelectedIndex == 1)
            {
                var salesInvoicePerCustomerForm = new SalesInvoicePerCustomerForm();

                salesInvoicePerCustomerForm.ShowDialog();

                return;
            }

            try
            {
                mainForm.ShowProgressStatus();

                List<ReportDataSource> sources = new List<ReportDataSource>();//, subSources = null;

                List<ReportParameter> parameters = null;

                string title = "", reportFile = "";

                //if (cboCustomer.SelectedValue != null) int.TryParse(cboCustomer.SelectedValue.ToString(), out customerId);

                if (cboReport.SelectedIndex == 0)
                {
                    var salesInvoiceDtosList = await salesInvoiceController.GetAll(null, from, to);

                    if (salesInvoiceDtosList.Count() > 0)
                    {
                        sources.Add(new ReportDataSource
                        {
                            Name = "SalesInvoiceDtos",
                            Value = salesInvoiceDtosList
                        });

                        title = "Item Sales Report";

                        reportFile = "DailySalesInvoiceSummaryReport.rdlc";
                    }
                    else hasRecord = false;
                }
                else if (cboReport.SelectedIndex == 2)
                {
                    var salesSummaryDtos = await salesInvoiceController.ProcessItemSalesReport(from, to, "", categoryId);

                    var list = salesSummaryDtos.SalesSummaryDetailDtosList.Where(item => item.SoldAmount > 0);

                    if (list.Count() > 0)
                    {
                        sources.Add(new ReportDataSource
                        {
                            Name = "SalesSummaryDetailDtos",
                            Value = list
                        });

                        title = "Item Sales Report";

                        reportFile = "ItemSalesReport.rdlc";

                        parameters = new List<ReportParameter>
                        {
                            new ReportParameter("DateRange", from.Date == to.Date ? from.ToShortDateString() : 
                                string.Format("{0} to {1}", from.ToShortDateString(), to.ToShortDateString()))
                        };
                    }
                    else hasRecord = false;
                }
                else if (cboReport.SelectedIndex == 3)
                {
                    var salesIncomeDtos = await salesInvoiceController.ProcessSalesIncomeReport(from, to, "", categoryId);

                    if (salesIncomeDtos.SalesIncomeDetailDtosList.Count() > 0)
                    {
                        sources.Add(new ReportDataSource
                        {
                            Name = "SalesIncomeDetailDtos",
                            Value = salesIncomeDtos.SalesIncomeDetailDtosList
                        });

                        title = "Sales Income Summary";

                        reportFile = "SalesIncomeReport.rdlc";

                        parameters = new List<ReportParameter>
                        {
                            new ReportParameter("DateRange", from.Date == to.Date ? from.ToShortDateString() : 
                                string.Format("{0} to {1}", from.ToShortDateString(), to.ToShortDateString()))
                        };
                    }
                    else hasRecord = false;
                }

                if (hasRecord) ShowReport(title, reportFile, sources, null, parameters);
                else mainForm.ShowMessage("No record found");
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void cboReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlCategory.Visible = cboReport.SelectedIndex >= 2;
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }

        private void cboReport_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar)) e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        private void txtCategory_Enter(object sender, EventArgs e)
        {
            if (txtCategory.Text == "SELECT CATEGORY")
            {
                txtCategory.Text = string.Empty;
            }
        }

        private void txtCategory_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                txtCategory.Text = "SELECT CATEGORY";

                pnlSelectCategory.Controls.Clear();

                pnlSelectCategory.Visible = false;
            }
        }

        private void txtCategory_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategory.Text) || isConfirmSelection ||
                txtCategory.Text == "SELECT CATEGORY" || !started) return;

            try
            {
                if (searchKeyEventMessenger != null) searchKeyEventMessenger(txtCategory.Text);

                SearchPanel(this.categoryList);
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }

        private void SearchPanel(IEnumerable<object> myList)
        {
            if (!pnlSelectCategory.Visible) return;

            var selectCategoryUI = new SelectCategoryOrSupplierUI(new SelectCategoryOrSupplierEventMessenger(SelectInvoked), myList);

            pnlSelectCategory.Controls.Add(selectCategoryUI);

            selectCategoryUI.Dock = DockStyle.Fill;

            searchKeyEventMessenger = new SearchKeyEventMessenger(selectCategoryUI.SearchKeyInvoked);

            listViewEventMessenger = new ProcessCmdEventMessenger(selectCategoryUI.ListViewFocusInvoked);

            confirmSelectionEventMessenger = new ProcessCmdEventMessenger(selectCategoryUI.ConfirmSelectionInvoked);

            pnlSelectCategory.Visible = !pnlSelectCategory.Visible;
        }

        private async void SelectInvoked(int id, string name, bool isCategory)
        {
            pnlSelectCategory.Controls.Clear();

            pnlSelectCategory.Visible = false;

            isConfirmSelection = true;

            this.categoryId = id;

            txtCategory.Text = name;

            txtCategory.Focus();

            await InitializeCategories();

            isConfirmSelection = false;
        }

        private void btnViewCategories_Click(object sender, EventArgs e)
        {
            pnlSelectCategory.Visible = !pnlSelectCategory.Visible;

            SearchPanel(this.categoryList);
        }
    }
}
