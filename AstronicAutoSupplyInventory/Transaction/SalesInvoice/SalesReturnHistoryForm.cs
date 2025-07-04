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

namespace AstronicAutoSupplyInventory.Transaction.SalesInvoice
{
    public partial class SalesReturnHistoryForm : Form
    {
        private SalesReturnController salesReturnController = new SalesReturnController();
        private SalesInvoiceController salesInvoiceController = new SalesInvoiceController();
        private UserController userController = new UserController();
        private DateRangeForm dateRangeForm;

        private IEnumerable<SalesReturnDtos> salesReturns = new List<SalesReturnDtos>();
        private DateTime from, to;
        //private IEnumerable<SalesReturnDtos> salesReturnDtosList;
        private bool started;

        //private int pageSize = 20, pageNumber = 1, totalPage = 0;

        private Timer typingTimer;  // Timer to handle debounce
        private const int TypingDelay = 300; // Delay in milliseconds (e.g., 500ms)

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public SalesReturnHistoryForm()
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
                btnAdd.Enabled = privileges.Any(privilege => privilege.Action == "Return Sales Invoice" && privilege.IsEnable);

                btnDelete.Enabled = privileges.Any(privilege => privilege.Action == "Delete Sales Invoice Return" && privilege.IsEnable);

                btnDetails.Enabled = privileges.Any(privilege => privilege.Action == "View Sales Invoice Return History" && privilege.IsEnable);
            }
        }

        private async Task InitializeSalesReturn(string key = "")
        {
            //if (this.salesReturnDtosList == null)
            //    this.salesReturnDtosList = await salesReturnController.GetAll(null, DateTime.MinValue, DateTime.MinValue);

            salesReturns = await salesReturnController.GetAll(null, from, to, key);

            salesReturns = salesReturns.OrderByDescending(t => t.Date).ToList();

            var itemCount = salesReturns.Count();

            //if (pageSize < itemCount) salesReturnDtosList = salesReturnDtosList.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            //var pages = itemCount < 1 || itemCount < pageSize ? 1 : (decimal)itemCount / pageSize;

            //var excess = pages - (int)Math.Truncate(pages);

            //if (excess > 0) excess = 1;

            //pages += excess;

            //totalPage = (int)pages;

            //lblRecord.Invoke(new MethodInvoker(() => lblRecord.Text =
            //    string.Format("Record: {0} of {1}", pageSize * (pageNumber - 1) + salesReturnDtosList.Count(),
            //        itemCount)));

            dgvItems.Rows.Clear();

            foreach (var returnDtos in salesReturns)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = returnDtos.SalesReturnId;

                row.Cells[0].Value = returnDtos.ReferenceNumber;

                row.Cells[1].Value = returnDtos.Date.ToString();

                row.Cells[2].Value = returnDtos.TotalQuantity.ToString("#,0.00;(#,0.00);''");

                row.Cells[3].Value = returnDtos.TotalAmount.ToString("#,0.00;(#,0.00);''");

                row.Cells[4].Value = returnDtos.UserName;
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

        private void SearchReturnInvoice(string key = "")
        {
            key = key.ToLower();
            IEnumerable<SalesReturnDtos> returns = this.salesReturns.Where(t => t.ReferenceNumber.Contains(key.ToLower())).ToList();

            dgvItems.Rows.Clear();

            foreach (var returnDtos in returns)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = returnDtos.SalesReturnId;

                row.Cells[0].Value = returnDtos.ReferenceNumber;

                row.Cells[1].Value = returnDtos.Date.ToString();

                row.Cells[2].Value = returnDtos.TotalQuantity.ToString("#,0.00;(#,0.00);''");

                row.Cells[3].Value = returnDtos.TotalAmount.ToString("#,0.00;(#,0.00);''");

                row.Cells[4].Value = returnDtos.UserName;
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

        private void SalesReturnedListForm_Load(object sender, EventArgs e)
        {
            try
            {
                //mainForm.ShowProgressStatus();
                var dateTime1 = DateTime.Now.Date;
                var dateTime2 = dateTime1.AddDays(-(dateTime1.Day - 1));

                from = dateTime1.AddDays(-(dateTime1.Day - 1));

                var lastDay = DateTime.DaysInMonth(dateTime1.Year, dateTime1.Month);
                to = dateTime1.AddDays(lastDay - dateTime1.Day);

                InitializePrivileges();

                ConfirmDateRangeInvoked(DateTime.MinValue, DateTime.MinValue);
                //await InitializeSalesReturn();

                started = true;
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            //finally { mainForm.ShowProgressStatus(false); }
        }
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer
            typingTimer.Stop();
            var key = txtSearch.Text;
            // Perform the action, e.g., call search function
            SearchReturnInvoice(key);
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
                    SearchReturnInvoice(key);
                }
                catch { /*mainForm.HandleException(ex);*/ }
            }
            else timer.Interval += 300;
        }
        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!started) return;

            var key = txtSearch.Text;

            try
            {
                //pageNumber = 1;

                await InitializeSalesReturn(key);
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }

        private async void ConfirmDateRangeInvoked(DateTime from, DateTime to)
        {
            try
            {
                mainForm.ShowProgressStatus();

                pnlDateRange.Visible = from > DateTime.MinValue && to > DateTime.MinValue;

                lblDateRange.Text = from.Date == to.Date ? string.Format("Sales Return for {0}", from.ToShortDateString()) :
                    string.Format("Sales Return from {0} to {1}", from.ToShortDateString(), to.ToShortDateString());

                if (dateRangeForm != null) dateRangeForm.Close();

                var key = txtSearch.Text;

                if (from > DateTime.MinValue && to > DateTime.MinValue)
                {
                    this.from = from;

                    this.to = to;
                }

                await InitializeSalesReturn(key);
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            pnlDateRange.Visible = false;

            from = DateTime.MinValue;

            to = DateTime.MinValue;
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || dgvItems.SelectedRows.Count < 1 || !btnDelete.Enabled) return;

            try
            {
                var result = mainForm.ShowMessage("Are you sure yo want to delete?", true);

                if (result == DialogResult.No) return;

                var success = false;

                foreach (DataGridViewRow row in dgvItems.SelectedRows)
                {
                    var id = 0;

                    if (row.Tag == null) continue;

                    int.TryParse(row.Tag.ToString(), out id);

                    var salesReturn = await salesReturnController.Find(id);

                    var detailList = salesReturn.SalesReturnDetailDtosList;

                    success = await salesReturnController.Delete(id);

                    if (!success) break;
                    else
                    {
                        dgvItems.Rows.Remove(row);

                        foreach (var item in detailList)
                        {
                            await userController.SaveItemActivity(
                                new UserActivityDtos
                                {
                                    Action = "Sales Invoice Return (OUT-DELETED)",
                                    CurrentStock = item.CurrentStock,
                                    Date = DateTime.Now,
                                    ItemId = item.ItemDtos.ItemId,
                                    Quantity = item.Quantity,
                                    Amount = item.Amount,
                                    ReferenceNumber = salesReturn.ReferenceNumber,
                                    UserId = mainForm.UserDtos.UserId
                                });
                        }
                    }
                }

                if (success) mainForm.ShowMessage("Successfully deleted.", true);
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnAdd.Enabled) return;

            var salesReturnItemsForm = new SalesReturnItemsForm();

            mainForm.ChangeForm(salesReturnItemsForm);
        }

        private void lnkMyInventory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dateRangeForm = new DateRangeForm(new ConfirmDataRangeEventMessenger(ConfirmDateRangeInvoked), from, to);

            dateRangeForm.ShowDialog();

            dateRangeForm.BringToFront();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || dgvItems.SelectedRows.Count < 1 || !btnDetails.Enabled) return;

            var row = dgvItems.SelectedRows[0];

            var id = 0;

            if (row.Tag == null) return;

            int.TryParse(row.Tag.ToString(), out id);

            if (id < 1) return;

            var viewDetailForm = new SalesReturnViewDetailForm(id);

            mainForm.ChangeForm(viewDetailForm, true);
        }

        private void btnClose2_Click(object sender, EventArgs e)
        {
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

            //await InitializeSalesReturn(key);

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
                ConfirmDateRangeInvoked(from, to);

                started = true;
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }
    }
}
