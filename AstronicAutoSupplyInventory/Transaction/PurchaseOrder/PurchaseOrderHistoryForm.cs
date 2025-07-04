using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Shared;
using CommonLibrary.Dtos;
using InventoryServices.Controllers;
using InventoryServices.Models;
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
    public partial class PurchaseOrderHistoryForm : Form
    {
        private PurchaseOrderController poController = new PurchaseOrderController();
        private PurchaseOrderReturnController poReturnController = new PurchaseOrderReturnController();
        private UserController userController = new UserController();

        private IEnumerable<PurchaseOrderDtos> purchaseOrders = new List<PurchaseOrderDtos>();

        private DateRangeForm dateRangeForm;
        //private EnterRemarksForm enterRemarksForm;

        private DateTime from, to;
        //private IEnumerable<PurchaseOrderDtos> purchaseOrderDtosList;
        private bool started;

        //private int pageSize = 20, pageNumber = 1, totalPage = 0;

        private Timer typingTimer;  // Timer to handle debounce
        private const int TypingDelay = 300; // Delay in milliseconds (e.g., 500ms)

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public PurchaseOrderHistoryForm()
        {
            // Initialize the Timer
            typingTimer = new Timer();
            typingTimer.Interval = TypingDelay;
            typingTimer.Tick += TypingTimer_Tick;

            InitializeComponent();

            var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            Width = screen.Width - 50;

            Height = screen.Height - 50;
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.F1:
                    btnNew_Click(this, new EventArgs());

                    return true;
                case Keys.F2:
                    btnReturnItems_Click(btnReturnItems, new EventArgs());

                    return true;
                case Keys.F3:
                    btnReturnPo_Click(btnReturnPo, new EventArgs());

                    return true;
                case Keys.F5:
                    btnRefresh_Click(this, new EventArgs());

                    return true;
                case Keys.Enter:
                    if (!dgvItems.Focused) break;

                    btnViewDetail_Click(this, new EventArgs());

                    return true;
                case Keys.Escape:
                    btnClose2_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.S:
                    txtSearch.Focus();

                    return true;
                case Keys.Alt | Keys.D:
                    lnkMyInventory_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
                case Keys.Alt | Keys.X:
                    btnClose_Click(this, new EventArgs());

                    return true;
                case Keys.Down:
                    if (dgvItems.Focused) break;

                    dgvItems.Focus();

                    return true;
                //case Keys.Control | Keys.Left:
                //    btnPaging_Click(btnPrevious, new EventArgs());

                //    return true;
                //case Keys.Control | Keys.Alt | Keys.Left:
                //    btnPaging_Click(btnFirstPage, new EventArgs());

                //    return true;
                //case Keys.Control | Keys.Right:
                //    btnPaging_Click(btnNext, new EventArgs());

                //    return true;
                //case Keys.Control | Keys.Alt | Keys.Right:
                //    btnPaging_Click(btnLastPage, new EventArgs());

                //    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private void InitializePrivileges()
        {
            var privileges = mainForm.UserDtos.UserPrivilegeDtosList;

            if (privileges.Count() > 0)
            {
                btnNew.Enabled = privileges.Any(privilege => privilege.Action == "Encode Purchase Order" && privilege.IsEnable);

                btnReturnItems.Enabled = privileges.Any(privilege => privilege.Action == "Return Purchase Order" && privilege.IsEnable);

                btnReturnPo.Enabled = btnReturnItems.Enabled;

                btnViewDetail.Enabled = privileges.Any(privilege => privilege.Action == "View Purchase Order History" && privilege.IsEnable);
            }
        }

        private async Task InitializePurchaseOrder(string key = "")
        {
            //if (this.purchaseOrderDtosList == null)
            //    this.purchaseOrderDtosList = await poController.GetAll(null, DateTime.MinValue, DateTime.MinValue); ;

            purchaseOrders = await poController.GetAll(null, from, to, key);

            purchaseOrders = purchaseOrders.OrderByDescending(t => t.Date);

             var itemCount = purchaseOrders.Count();

            //if (pageSize < itemCount) purchaseOrderDtosList = purchaseOrderDtosList.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            //var pages = itemCount < 1 || itemCount < pageSize ? 1 : (decimal)itemCount / pageSize;

            //var excess = pages - (int)Math.Truncate(pages);

            //if (excess > 0) excess = 1;

            //pages += excess;

            //totalPage = (int)pages;

            //lblRecord.Invoke(new MethodInvoker(() => lblRecord.Text =
            //    string.Format("Record: {0} of {1}", pageSize * (pageNumber - 1) + purchaseOrderDtosList.Count(),
            //        itemCount)));

            dgvItems.Rows.Clear();

            foreach (var poDtos in purchaseOrders)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = poDtos.PurchaseOrderId;

                row.Cells[0].Value = poDtos.PONumber;

                row.Cells[1].Value = poDtos.SupplierName;

                row.Cells[2].Value = poDtos.Date.ToString();

                row.Cells[3].Value = poDtos.TotalQuantity.ToString("#,0.00;(#,0.00);''");

                row.Cells[4].Value = poDtos.TotalDiscount.ToString("#,0.00;(#,0.00);''");

                row.Cells[5].Value = poDtos.GrandTotalAmount.ToString("#,0.00;(#,0.00);''");

                row.Cells[6].Value = poDtos.Remarks;

                row.Cells[7].Value = poDtos.UserName;
            }

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (DataGridViewRow currow in dgvItems.Rows)
            {
                currow.DefaultCellStyle.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
        }

        private void PurchaseOrderHistoryForm_Load(object sender, EventArgs e)
        {
            try
            {
                var dateTime1 = DateTime.Now.Date;
                var dateTime2 = dateTime1.AddDays(-(dateTime1.Day - 1));

                from = dateTime1.AddDays(-(dateTime1.Day - 1));

                var lastDay = DateTime.DaysInMonth(dateTime1.Year, dateTime1.Month);
                to = dateTime1.AddDays(lastDay - dateTime1.Day);

                //mainForm.ShowProgressStatus();

                InitializePrivileges();

                ConfirmDateRangeInvoked(DateTime.MinValue, DateTime.MinValue);
                //await InitializePurchaseOrder();

                started = true;
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            //finally { mainForm.ShowProgressStatus(false); }
        }

        private void lnkMyInventory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dateRangeForm = new DateRangeForm(new ConfirmDataRangeEventMessenger(ConfirmDateRangeInvoked), from, to);

            dateRangeForm.ShowDialog();

            dateRangeForm.BringToFront();
        }

        private async void ConfirmDateRangeInvoked(DateTime from, DateTime to)
        {
            try
            {
                mainForm.ShowProgressStatus();

                pnlDateRange.Visible = from > DateTime.MinValue && to > DateTime.MinValue;

                lblDateRange.Text = from.Date == to.Date ? string.Format("P.O. for {0}", from.ToShortDateString()) :
                    string.Format("P.O. from {0} to {1}", from.ToShortDateString(), to.ToShortDateString());

                if (dateRangeForm != null) dateRangeForm.Close();

                if (from > DateTime.MinValue && to > DateTime.MinValue)
                {
                    this.from = from;

                    this.to = to;
                }

                var key = txtSearch.Text;

                await InitializePurchaseOrder(key);
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnClose2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            pnlDateRange.Visible = false;

            from = DateTime.MinValue;

            to = DateTime.MinValue;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnNew.Enabled) return;

            var purchaseOrderEntryForm = new PurchaseOrderEntryForm();

            mainForm.ChangeForm(purchaseOrderEntryForm);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!started || !txtSearch.Focused) return;

            try
            {
                typingTimer.Stop();
                typingTimer.Start();
            }
            catch { /*mainForm.HandleException(ex);*/ }
        }

        private void SearchPO(string key = "")
        {
            key = key.ToLower();
            IEnumerable<PurchaseOrderDtos> purchaseOrders = this.purchaseOrders.Where(t => t.PONumber.ToLower().Contains(key.ToLower())).ToList();

            purchaseOrders = purchaseOrders.OrderByDescending(t => t.Date).ToList();

            dgvItems.Rows.Clear();

            foreach (var poDtos in purchaseOrders)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = poDtos.PurchaseOrderId;

                row.Cells[0].Value = poDtos.PONumber;

                row.Cells[1].Value = poDtos.SupplierName;

                row.Cells[2].Value = poDtos.Date.ToString();

                row.Cells[3].Value = poDtos.TotalQuantity.ToString("#,0.00;(#,0.00);''");

                row.Cells[4].Value = poDtos.TotalDiscount.ToString("#,0.00;(#,0.00);''");

                row.Cells[5].Value = poDtos.GrandTotalAmount.ToString("#,0.00;(#,0.00);''");

                row.Cells[6].Value = poDtos.Remarks;

                row.Cells[7].Value = poDtos.UserName;
            }

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (DataGridViewRow currow in dgvItems.Rows)
            {
                currow.DefaultCellStyle.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
        }

        private void btnReturnPo_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnReturnPo.Enabled) return;

            var poNumber = "";

            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            if (row.Cells[0].Value == null) return;

            poNumber = row.Cells[0].Value.ToString();

            if (string.IsNullOrWhiteSpace(poNumber)) return;

            var result = mainForm.ShowMessage("Are you sure you want to return this purchase order?", true);

            if (result == System.Windows.Forms.DialogResult.No) return;

            var enterRemarksForm = new EnterRemarksForm(
                new ConfirmRemarksEventMessenger(ConfirmRemarksInvoked),
                poNumber,
                row);

            enterRemarksForm.ShowDialog();
        }

        private async void ConfirmRemarksInvoked(string poNumber, string remarks, DateTime date, DataGridViewRow row)
        {
            var enterRemarksForm = (EnterRemarksForm)Application.OpenForms["EnterRemarksForm"];

            if (enterRemarksForm == null) return;

            enterRemarksForm.Close();

            enterRemarksForm = null;

            try
            {
                var poDtos = await poController.Find(poNumber);

                poDtos.UserId = mainForm.UserDtos.UserId;

                var success = await poReturnController.Save(
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

                    dgvItems.Rows.Remove(row);

                    mainForm.ShowMessage("Successfully returned.", true);
                }
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }
        }

        private void btnReturnItems_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnReturnItems.Enabled) return;

            var poNumber = "";

            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            if (row.Cells[0].Value == null) return;

            poNumber = row.Cells[0].Value.ToString();

            var poReturnForm = new PoReturnItemsForm(poNumber);

            mainForm.ChangeForm(poReturnForm, true);
        }

        private void btnPaging_Click(object sender, EventArgs e)
        {
            //if (mainForm.IsLoading) return;

            //var btn = (Button)sender;

            //if (btn == null) return;

            //if (!btn.Enabled) return;

            //pnlPageArea.Enabled = false;

            //var command = btn.Tag.ToString();

            //var number = pageNumber;

            //switch (command)
            //{
            //    case "First Page":
            //        number = 1;
            //        break;
            //    case "Previous":
            //        number--;
            //        break;
            //    case "Next":
            //        number++;
            //        break;
            //    case "Last Page":
            //        number = totalPage;
            //        break;
            //}
            //if (number == pageNumber) return;

            //pageNumber = number;

            //txtCurrentPage.Text = pageNumber.ToString();

            //pnlPageArea.Enabled = true;

            //var key = txtSearch.Text;

            //await InitializePurchaseOrder(key);

            //ActiveControl = btn;
        }

        private void btnViewDetail_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnViewDetail.Enabled) return;

            var poNumber = "";

            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            if (row.Cells[0].Value == null) return;

            poNumber = row.Cells[0].Value.ToString();

            var poViewDetailForm = new PurchaseOrderViewDetailForm(poNumber);

            mainForm.ChangeForm(poViewDetailForm, true);
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (timer.Interval >= 300 && timer.Enabled)
            {
                timer.Stop();

                timer.Enabled = false;

                try
                {
                    var key = txtSearch.Text;
                    SearchPO(key);
                }
                catch { /*mainForm.HandleException(ex);*/ }
            }
            else timer.Interval += 300;
        }
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer
            typingTimer.Stop();
            var key = txtSearch.Text;
            // Perform the action, e.g., call search function
            SearchPO(key);
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                //mainForm.ShowProgressStatus();

                //InitializePrivileges();

                //this.purchaseOrderDtosList = null;

                ConfirmDateRangeInvoked(from, to);
                //await InitializeSalesInvoice();

                started = true;
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }
    }
}
