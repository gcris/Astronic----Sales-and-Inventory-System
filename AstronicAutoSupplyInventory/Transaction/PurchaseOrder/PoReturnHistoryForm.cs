using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Shared;
using CommonLibrary.Dtos;
using InventoryServices.Controllers;
using InventoryServices.Interfaces;
using InventoryServices.Repositories;
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
    public partial class PoReturnHistoryForm : Form
    {
        private PurchaseOrderReturnController poReturnController = new PurchaseOrderReturnController();
        private PurchaseOrderController purchaseOrderController = new PurchaseOrderController();
        private UserController userController = new UserController();

        private DateRangeForm dateRangeForm;

        private IEnumerable<PurchaseOrderReturnDtos> purchaseReturns = new List<PurchaseOrderReturnDtos>();
        private DateTime from, to;
        //private IEnumerable<PurchaseOrderReturnDtos> purchaseOrderReturnDtosList;


        //private int pageSize = 20, pageNumber = 1, totalPage = 0;
        private Timer typingTimer;  // Timer to handle debounce
        private const int TypingDelay = 300; // Delay in milliseconds (e.g., 500ms)

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public PoReturnHistoryForm()
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
                    btnAdd_Click(this, new EventArgs());

                    return true;
                case Keys.F2:
                    btnDelete_Click(this, new EventArgs());

                    return true;
                case Keys.F5:
                    btnRefresh_Click(this, new EventArgs());

                    return true;
                case Keys.Enter:
                    if (!dgvItems.Focused) break;

                    btnDetails_Click(this, new EventArgs());

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
                btnAdd.Enabled = privileges.Any(privilege => privilege.Action == "Encode Sales Invoice Return" && privilege.IsEnable);

                btnDelete.Enabled = privileges.Any(privilege => privilege.Action == "Delete Sales Invoice Return" && privilege.IsEnable);

                btnDetails.Enabled = privileges.Any(privilege => privilege.Action == "View Sales Invoice Return History" && privilege.IsEnable);
            }
        }

        private async Task InitialiPurchaseOrderReturn(string key = "")
        {
            //if (this.purchaseOrderReturnDtosList == null)
            //    this.purchaseOrderReturnDtosList = await poReturnController.GetAll(null, DateTime.MinValue, DateTime.MinValue);

            purchaseReturns = await poReturnController.GetAll(null, from, to, key);

            purchaseReturns = purchaseReturns.OrderByDescending(t => t.Date).ToList();

            var itemCount = purchaseReturns.Count();

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

            foreach (var returnDtos in purchaseReturns)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = returnDtos.PurchaseOrderReturnId;

                row.Cells[0].Value = returnDtos.ReferenceNumber;

                row.Cells[1].Value = returnDtos.Date.ToString();

                row.Cells[2].Value = returnDtos.TotalQuantity.ToString();

                row.Cells[3].Value = returnDtos.TotalAmount.ToString();

                row.Cells[4].Value = returnDtos.UserName;
            }

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                row.DefaultCellStyle.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
            //lblTotaPage.Invoke(new MethodInvoker(() => lblTotaPage.Text = "of " + totalPage));

            //txtCurrentPage.Invoke(new MethodInvoker(() => txtCurrentPage.Text = pageNumber.ToString()));

            //// Page Settings
            //btnFirstPage.Invoke(new MethodInvoker(() => btnFirstPage.Enabled = pageNumber > 1));

            //btnPrevious.Invoke(new MethodInvoker(() => btnPrevious.Enabled = pageNumber > 1));

            //btnLastPage.Invoke(new MethodInvoker(() => btnLastPage.Enabled = pageNumber < totalPage));

            //btnNext.Invoke(new MethodInvoker(() => btnNext.Enabled = pageNumber < totalPage));
        }

        private async void SalesReturnedListForm_Load(object sender, EventArgs e)
        {
            try
            {
                var dateTime1 = DateTime.Now.Date;
                var dateTime2 = dateTime1.AddDays(-(dateTime1.Day - 1));

                from = dateTime1.AddDays(-(dateTime1.Day - 1));

                var lastDay = DateTime.DaysInMonth(dateTime1.Year, dateTime1.Month);
                to = dateTime1.AddDays(lastDay - dateTime1.Day);

                InitializePrivileges();
                await InitialiPurchaseOrderReturn();
                ConfirmDateRangeInvoked(DateTime.MinValue, DateTime.MinValue);
               

            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            //finally { mainForm.ShowProgressStatus(false); }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //mainForm.ShowProgressStatus();

                InitializePrivileges();

                ConfirmDateRangeInvoked(DateTime.MinValue, DateTime.MinValue);
                //await InitializeSalesReturn();

            }
            catch (Exception ex) { mainForm.HandleException(ex); }

        }
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer
            typingTimer.Stop();
            var key = txtSearch.Text;
            // Perform the action, e.g., call search function
            SearchReturnPurchase(key);
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
                    SearchReturnPurchase(key);
                }
                catch { /*mainForm.HandleException(ex);*/ }
            }
            else timer.Interval += 300;
        }
        private void SearchReturnPurchase(string key = "")
        {
            key = key.ToLower();
            IEnumerable<PurchaseOrderReturnDtos> returns = this.purchaseReturns.Where(t => t.ReferenceNumber.Contains(key.ToLower())).ToList();

            dgvItems.Rows.Clear();

            foreach (var returnDtos in returns)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = returnDtos.PurchaseOrderReturnId;

                row.Cells[0].Value = returnDtos.ReferenceNumber;

                row.Cells[1].Value = returnDtos.Date.ToString();

                row.Cells[2].Value = returnDtos.TotalQuantity.ToString();

                row.Cells[3].Value = returnDtos.TotalAmount.ToString();

                row.Cells[4].Value = returnDtos.UserName;
            }

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                row.DefaultCellStyle.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
        }
        private async void ConfirmDateRangeInvoked(DateTime from, DateTime to)
        {
            try
            {
                mainForm.ShowProgressStatus();

                pnlDateRange.Visible = from > DateTime.MinValue && to > DateTime.MinValue;

                lblDateRange.Text = from.Date == to.Date ? string.Format("P.O. Return for {0}", from.ToShortDateString()) :
                    string.Format("P.O. Return from {0} to {1}", from.ToShortDateString(), to.ToShortDateString());

                if (dateRangeForm != null) dateRangeForm.Close();

                if (from > DateTime.MinValue && to > DateTime.MinValue)
                {
                    this.from = from;

                    this.to = to;
                }

                var key = txtSearch.Text;

                await InitialiPurchaseOrderReturn(key);
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            pnlDateRange.Visible = false;

            from = DateTime.MinValue;

            to = DateTime.MinValue;
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count < 1 || mainForm.IsLoading || !btnDelete.Enabled) return;

            var result = mainForm.ShowMessage("Are you sure yo want to delete?", true);

            if (result == DialogResult.No) return;

            try
            {
                mainForm.ShowProgressStatus();

                var success = false;

                foreach (DataGridViewRow row in dgvItems.SelectedRows)
                {
                    var id = 0;

                    if (row.Tag == null) continue;

                    int.TryParse(row.Tag.ToString(), out id);

                    var po = await poReturnController.Find(id);

                    var detailList = po.PurchaseOrderReturnDetailDtosList;

                    success = await poReturnController.Delete(id);

                    if (!success) break;
                    else
                    {
                        dgvItems.Rows.Remove(row);

                        foreach (var item in detailList)
                        {
                            await userController.SaveItemActivity(
                                new UserActivityDtos
                                {
                                    Action = "Purchase Order Return (IN-DELETED)",
                                    CurrentStock = item.CurrentStock,
                                    Date = DateTime.Now,
                                    ItemId = item.ItemDtos.ItemId,
                                    Quantity = item.Quantity,
                                    Amount = item.Amount,
                                    ReferenceNumber = po.ReferenceNumber,
                                    Remarks = item.Remarks,
                                    UserId = mainForm.UserDtos.UserId
                                });
                        }
                    }
                }

                if (success) mainForm.ShowMessage("Successfully deleted.");
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnAdd.Enabled) return;

            var poReturnItemsForm = new PoReturnItemsForm();

            mainForm.ChangeForm(poReturnItemsForm, true);
        }

        private void lnkMyInventory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (mainForm.IsLoading) return;

            dateRangeForm = new DateRangeForm(new ConfirmDataRangeEventMessenger(ConfirmDateRangeInvoked), from, to);

            dateRangeForm.ShowDialog();

            dateRangeForm.BringToFront();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count < 1 || mainForm.IsLoading || !btnDetails.Enabled) return;

            var row = dgvItems.SelectedRows[0];

            var id = 0;

            if (row.Tag == null) return;

            int.TryParse(row.Tag.ToString(), out id);

            if (id < 1) return;

            var viewDetailForm = new PoReturnViewDetailForm(id);

            mainForm.ChangeForm(viewDetailForm);
        }

        private void btnClose2_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            this.Close();
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

            //await InitialiPurchaseOrderReturn(key);

            //ActiveControl = btn;
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                //mainForm.ShowProgressStatus();

                //InitializePrivileges();

                //this.purchaseOrderReturnDtosList = null;

                ConfirmDateRangeInvoked(from, to);
                //await InitializeSalesInvoice();

            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }
    }
}
