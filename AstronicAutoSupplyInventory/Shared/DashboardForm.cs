using Infrastructure.UserControls;
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
using Infrastructure.EventMessenger;
using CommonLibrary.Dtos;

namespace AstronicAutoSupplyInventory.Shared
{
    public partial class DashboardForm : Form
    {
        private SalesInvoiceController salesInvoiceController = new SalesInvoiceController();
        private SalesReturnController salesReturnController = new SalesReturnController();
        private PurchaseOrderController purchaseOrderController = new PurchaseOrderController();
        private PurchaseOrderReturnController purchaseOrderReturnController = new PurchaseOrderReturnController();
        private ItemController itemController = new ItemController();

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public DashboardForm()
        {
            InitializeComponent();
        }

        private void AddPanelTile(UserControl control)
        {
            var panelHolder = new PanelTileHolder(control);

            pnlStockContainer.Controls.Add(panelHolder);

            panelHolder.Dock = DockStyle.Left;

            panelHolder.BringToFront();
        }

        private void AddGraphPanel(IEnumerable<TransactionFrequencyDtos> source, string title, Panel panel)
        {
            var frequencyGraphUI = new FrequencyGraphUI(source, title);
            panel.Controls.Add(frequencyGraphUI);
            frequencyGraphUI.Dock = DockStyle.Left;
            frequencyGraphUI.BringToFront();
        }

        private void MakePanelCenter(Panel parent)
        {
            var pleaseWait = new PleaseWaitUI();
            parent.Controls.Add(pleaseWait);
            pleaseWait.Left = (parent.ClientSize.Width - pleaseWait.Width) / 2;
            pleaseWait.Top = (parent.ClientSize.Height - pleaseWait.Height) / 2;
        }

        private async Task InitializeDashboard()
        {
            MakePanelCenter(pnlGraphContainer1);
            MakePanelCenter(pnlGraphContainer2);
            var minimumItems = await itemController.GetMinimumStock();
            AddPanelTile(new PanelTile1UI(new NavigateToMinimumStockEventMessenger(MinimumStockInvoked), minimumItems.Count(), Color.IndianRed));
            AddPanelTile(new DateNowPanelUI());

            var salesFrequency = await salesInvoiceController.GetTransactionFrequency();
            AddGraphPanel(salesFrequency, "Sales Invoice", pnlGraphContainer1);

            var salesReturnFrequency = await salesReturnController.GetTransactionFrequency();
            AddGraphPanel(salesReturnFrequency, "Sales Invoice Return", pnlGraphContainer1);

            var poFrequency = await purchaseOrderController.GetTransactionFrequency();
            AddGraphPanel(poFrequency, "Purchase Order", pnlGraphContainer2);

            var poReturnFrequency = await purchaseOrderReturnController.GetTransactionFrequency();
            AddGraphPanel(poReturnFrequency, "Purchase Order Return", pnlGraphContainer2);
        }

        private void MinimumStockInvoked(bool minimumStock)
        {
            var inventoryReport = new InventorySummaryReportForm(minimumStock);

            inventoryReport.ShowDialog();

            //mainForm.ChangeForm(inventoryReport);
        }

        private async void DashboardForm_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                await InitializeDashboard();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }
    }
}
