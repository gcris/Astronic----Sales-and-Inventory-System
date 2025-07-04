using AstronicAutoSupplyInventory.EventMesseging;
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
    public partial class InventorySummaryReportForm : Form
    {
        private ItemController itemController = new ItemController();
        private CategoryController categoryController = new CategoryController();
        private InventorySummaryController inventorySummaryController = new InventorySummaryController();
        private UserController userController = new UserController();

        private DateTime from, to;

        //private List<ReportDataSource> subSources;

        private bool started, minimumStock;

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private IEnumerable<CategoryDtos> categoryList;
        private int categoryId;
        private SearchKeyEventMessenger searchKeyEventMessenger;
        private ProcessCmdEventMessenger listViewEventMessenger;
        private ProcessCmdEventMessenger confirmSelectionEventMessenger;
        private bool isConfirmSelection;

        public InventorySummaryReportForm(bool minimumStock = false)
        {
            this.minimumStock = minimumStock;

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
                case Keys.Alt | Keys.I:
                    cboPrices.Focus();

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

        private async void InventorySummaryReport_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                cboReport.SelectedIndex = minimumStock ? 3 : 0;

                await InitializeCategories();

                pnlDate.Visible = cboReport.SelectedIndex == 0 || cboReport.SelectedIndex == 1;

                cboPrices.SelectedIndex = 0;

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

        private void lnkChangeDate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!started || mainForm.IsLoading) return;

            var dateRangeForm = new DateRangeForm(new EventMesseging.ConfirmDataRangeEventMessenger(ConfirmDateInvoked), from, to);

            dateRangeForm.ShowDialog();
        }

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

        private void ShowReport(string title, string reportFile, List<ReportDataSource> sources, List<ReportParameter> parameters = null)
        {
            pnlReport.Controls.Clear();

            var reportViewer = new ReportViewer();

            reportViewer.RefreshReport();

            lblTitle.Text = title;

            reportViewer.ProcessingMode = ProcessingMode.Local;

            //var path = Path.Combine(Environment.CurrentDirectory, @"Reports\" + reportFile);

            reportViewer.LocalReport.ReportEmbeddedResource = "AstronicAutoSupplyInventory.Reports." + reportFile;

            sources.ForEach(source => reportViewer.LocalReport.DataSources.Add(source));

            //if (subSources != null) // means the report contains sub report
            //    reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportProcessing);

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

        private async void btnGo_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !started) return;

            var hasRecord = true;

            try
            {
                mainForm.ShowProgressStatus();

                var sources = new List<ReportDataSource>();

                List<ReportParameter> parameters = null;

                string title = "", reportFile = "";

                var withQuantity = chkWithQuantity.Checked;

                if (cboReport.SelectedIndex == 0)
                {
                    var inventoryDtosList = await inventorySummaryController.GetDailyInventory(from, to, categoryId);

                    if (inventoryDtosList.Count() > 0)
                    {
                        sources.Add(new ReportDataSource
                        {
                            Name = "InventorySummaryDtos",
                            Value = inventoryDtosList
                        });

                        title = "Inventory Summary";

                        reportFile = "DailyInventorySummaryReport.rdlc";
                    }
                    else hasRecord = false;
                }
                else if (cboReport.SelectedIndex == 1)
                {
                    var inventoryDtos = await inventorySummaryController.GetInventory(from, to, categoryId);

                    var inventorySummaryDetailDtosList = new List<InventorySummaryDetailDtos>();
                    foreach (var item in inventoryDtos)
                    {
                        inventorySummaryDetailDtosList.AddRange(item.InventorySummaryDetailDtosList);
                    }

                    //if (withQuantity) inventorySummaryDetailDtosList = inventorySummaryDetailDtosList.Where(item => item.EndingInv != 0);

                    if (inventoryDtos != null)
                    {
                        sources.Add(new ReportDataSource
                        {
                            Name = "InventorySummaryDetailDtos",
                            Value = inventorySummaryDetailDtosList
                        });

                        title = "Inventory Summary";

                        reportFile = "InventorySummaryReport.rdlc";

                        parameters = new List<ReportParameter>
                        {
                            new ReportParameter("DateRange", from.Date == to.Date ? from.ToShortDateString() : 
                                string.Format("{0} to {1}", from.ToShortDateString(), to.ToShortDateString()))
                        };
                    }
                    else hasRecord = false;
                }
                else if (cboReport.SelectedIndex == 2 || cboReport.SelectedIndex == 3)
                {
                    var itemDtosList = await itemController.GetAllByCategoryAndSupplier(null, "", categoryId);
                    if (withQuantity) itemDtosList = itemDtosList.Where(item => item.QuantityOnHand != 0);
                    if (itemDtosList.Count() > 0)
                    {
                        var list = new List<ItemDtos>();
                        switch (cboPrices.Text)
                        {
                            case "Price 2":
                                break;
                            case "Current Cost":
                                break;
                        }
                        foreach (var item in itemDtosList)
                        {
                            item.Price1 = cboPrices.SelectedIndex == 0 ? item.Price1 : cboPrices.SelectedIndex == 1 ? item.Price2 : item.CurrentCost;
                            item.CurrentCost = item.CurrentCost;
                            item.MinimumStock =  item.Price1 * item.QuantityOnHand;
                            item.UnitOfMeasure = item.UnitOfMeasure;
                            list.Add(item);
                        }
                        if (cboReport.SelectedIndex == 2)
                        {
                            if (parameters == null) parameters = new List<ReportParameter>();
                            parameters.Add(new ReportParameter("Title", cboPrices.Text));
                        }
                        sources.Add(new ReportDataSource
                        {
                            Name = "ItemDtos",
                            Value = cboReport.SelectedIndex == 2 ||cboReport.SelectedIndex == 3 ? list : itemDtosList.ToList()
                        });
                        title = cboReport.SelectedIndex == 2 ? "Ending Inventory" : "Ending Summary Report";
                        reportFile =  cboReport.SelectedIndex == 2 && cboPrices.Text == "All" ? "EndingInventoryWithPricesReport.rdlc" :
                            cboReport.SelectedIndex == 2 ? "EndingInventoryReport.rdlc"  : "EndingInventoryByCategoryReport.rdlc";

                    }
                    else hasRecord = false;
                }
                else
                {
                    var reportType = cboReport.Text;

                    IEnumerable<ItemDtos> itemStockList = null;

                    if (reportType.Contains("Minimum")) itemStockList = await itemController.GetMinimumStock();
                    else if (reportType.Contains("Non-Moving")) itemStockList = await itemController.GetNonMovingItems(categoryId);
                    else itemStockList = await itemController.GetMovingItems(categoryId, "", reportType.Contains("Slow-Moving"));

                    if (withQuantity) itemStockList = itemStockList.Where(item => item.QuantityOnHand != 0);

                    if (itemStockList.Count() > 0)
                    {
                        sources.Add(new ReportDataSource
                        {
                            Name = "ItemDtos",
                            Value = itemStockList
                        });

                        title = reportType;

                        reportFile = "ItemStockReport.rdlc";

                        if (parameters == null) parameters = new List<ReportParameter>();

                        parameters.Add(new ReportParameter("Title", reportType));
                    }
                    else hasRecord = false;
                }

                mainForm.ShowProgressStatus(false);

                if (hasRecord) ShowReport(title, reportFile, sources, parameters);
                else mainForm.ShowMessage("No record found");
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void cboReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!started) return;
            pnlDate.Visible = cboReport.SelectedIndex >= 0 && cboReport.SelectedIndex <= 1;
            pnlPrices.Visible = cboReport.SelectedIndex == 2;
            if (cboReport.SelectedIndex == 3) categoryId = 0;
            txtCategory.Text = cboReport.SelectedIndex == 3 ? "ALL CATEGORY" : "SELECT CATEGORY";
            pnlWithQuantity.Visible = cboReport.SelectedIndex != 0;
            if (cboReport.SelectedIndex == 0) chkWithQuantity.Checked = false;
            await InitializeCategories();
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
                txtCategory.Text = cboReport.SelectedIndex == 3 ? "ALL CATEGORY" : "SELECT CATEGORY";

                pnlSelectCategory.Controls.Clear();

                pnlSelectCategory.Visible = false;
            }
        }

        private void txtCategory_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategory.Text) || isConfirmSelection ||
                txtCategory.Text == "SELECT CATEGORY" || txtCategory.Text == "ALL CATEGORY" || !started) return;

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
