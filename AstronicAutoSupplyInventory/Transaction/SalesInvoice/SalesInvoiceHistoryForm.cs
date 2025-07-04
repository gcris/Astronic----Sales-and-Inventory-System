using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Shared;
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
//using System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace AstronicAutoSupplyInventory.Transaction.SalesInvoice
{
    public partial class SalesInvoiceHistoryForm : Form
    {
        private SalesInvoiceController controller = new SalesInvoiceController();
        private SalesReturnController salesReturnController = new SalesReturnController();
        private UserController userController = new UserController();
        
        private IEnumerable<SalesInvoiceDtos> salesInvoices = new List<SalesInvoiceDtos>();

        private DateRangeForm dateRangeForm;
        private EnterRemarksForm enterRemarksForm;

        private DateTime from, to;
        //private IEnumerable<SalesInvoiceDtos> salesInvoiceDtosList;
        private bool started;

        //private int pageSize = 20, pageNumber = 1, totalPage = 0;

        private Timer typingTimer;  // Timer to handle debounce
        private const int TypingDelay = 300; // Delay in milliseconds (e.g., 500ms)


        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public SalesInvoiceHistoryForm()
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
                    btnReturnItems_Click(btnReturnItems, new EventArgs());

                    return true;
                case Keys.F2:
                    btnReturnSalesInvoice_Click(btnReturnSalesInvoice, new EventArgs());

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
                btnReturnItems.Enabled = privileges.Any(privilege => privilege.Action == "Return Sales Invoice" && privilege.IsEnable);

                btnReturnSalesInvoice.Enabled = btnReturnItems.Enabled;

                btnViewDetail.Enabled = privileges.Any(privilege => privilege.Action == "View Sales Invoice History" && privilege.IsEnable);
            }
        }

        private async Task InitializeSalesInvoice(string key = "")
        {
            //if (this.salesInvoiceDtosList == null)
            //    this.salesInvoiceDtosList = await controller.GetAll(null, DateTime.MinValue, DateTime.MinValue);



            salesInvoices = await controller.GetAll(null, from, to, key);

            salesInvoices = salesInvoices.OrderByDescending(t => t.Date);

            var itemCount = salesInvoices.Count();

            //if (pageSize < itemCount) salesInvoiceDtosList = salesInvoiceDtosList.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

            //var pages = itemCount < 1 || itemCount < pageSize ? 1 : (decimal)itemCount / pageSize;

            //var excess = pages - (int)Math.Truncate(pages);

            //if (excess > 0) excess = 1;

            //pages += excess;
            
            //totalPage = (int)pages;

            //lblRecord.Invoke(new MethodInvoker(() => lblRecord.Text =
            //    string.Format("Record: {0} of {1}", pageSize * (pageNumber - 1) + salesInvoiceDtosList.Count(),
            //        itemCount)));

            dgvItems.Rows.Clear();

            foreach (var invoiceDtos in salesInvoices)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = invoiceDtos.SalesInvoiceId;

                row.Cells[0].Value = invoiceDtos.ORNumber;

                row.Cells[1].Value = invoiceDtos.CustomerName;

                row.Cells[2].Value = invoiceDtos.Date.ToString();

                row.Cells[3].Value = invoiceDtos.TotalQuantity.ToString("#,0.00;(#,0.00);''");

                row.Cells[4].Value = invoiceDtos.TotalDiscount.ToString("#,0.00;(#,0.00);''");

                row.Cells[5].Value = invoiceDtos.TotalAmount.ToString("#,0.00;(#,0.00);''");

                row.Cells[6].Value = invoiceDtos.Remarks;

                row.Cells[7].Value = invoiceDtos.UserName;

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

        private async void SalesInvoiceHistoryForm_Load(object sender, EventArgs e)
        {
            try
            {
                var dateTime1 = DateTime.Now.Date;
                var dateTime2 = dateTime1.AddDays(-(dateTime1.Day - 1));

                from = dateTime1.AddDays(-(dateTime1.Day - 1));

                var lastDay = DateTime.DaysInMonth(dateTime1.Year, dateTime1.Month);
                to = dateTime1.AddDays(lastDay - dateTime1.Day);

                InitializePrivileges();

                await InitializeSalesInvoice();
                ConfirmDateRangeInvoked(DateTime.MinValue, DateTime.MinValue);
                //await InitializeSalesInvoice();

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
             SearchInvoice(key);
        }


  
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!started || !txtSearch.Focused) return;
            //if (!started || txtSearch.Text == "SEARCH HERE!" ||
            //    string.IsNullOrWhiteSpace(txtSearch.Text)) return;

            //timer.Enabled = true;

            //timer.Interval = 100;

            //timer.Start();
            typingTimer.Stop();
            typingTimer.Start();
            try
            {
                //mainForm.ShowProgressStatus();
                //pageNumber = 1;
                //await DoSearch();

            }
            catch { /*mainForm.HandleException(ex);*/ }
        }
        private void SearchInvoice(string key = "")
        {
            key = key.ToLower();
            IEnumerable<SalesInvoiceDtos> invoices = this.salesInvoices.Where(t => t.ORNumber.ToLower().Contains(key.ToLower())).ToList();

            invoices = invoices.OrderByDescending(t => t.Date).ToList();

            dgvItems.Rows.Clear();


            foreach (var invoiceDtos in invoices)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = invoiceDtos.SalesInvoiceId;

                row.Cells[0].Value = invoiceDtos.ORNumber;

                row.Cells[1].Value = invoiceDtos.CustomerName;

                row.Cells[2].Value = invoiceDtos.Date.ToString();

                row.Cells[3].Value = invoiceDtos.TotalQuantity.ToString("#,0.00;(#,0.00);''");

                row.Cells[4].Value = invoiceDtos.TotalDiscount.ToString("#,0.00;(#,0.00);''");

                row.Cells[5].Value = invoiceDtos.TotalAmount.ToString("#,0.00;(#,0.00);''");

                row.Cells[6].Value = invoiceDtos.Remarks;

                row.Cells[7].Value = invoiceDtos.UserName;

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
                pnlDateRange.Visible = from > DateTime.MinValue && to > DateTime.MinValue;

                lblDateRange.Text = from.Date == to.Date ? string.Format("Sales Invoice for {0}", from.ToShortDateString()) :
                    string.Format("Sales Invoice from {0} to {1}", from.ToShortDateString(), to.ToShortDateString());

                if (dateRangeForm != null) dateRangeForm.Close();

                var key = txtSearch.Text;

                if (from > DateTime.MinValue && to > DateTime.MinValue)
                {
                    this.from = from;

                    this.to = to;

                    mainForm.ShowProgressStatus();
                }

                await InitializeSalesInvoice(key);
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

        private void btnReturnSalesInvoice_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnReturnSalesInvoice.Enabled) return;

            var orNumber = "";

            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            if (row.Cells[0].Value == null) return;

            orNumber = row.Cells[0].Value.ToString();

            if (string.IsNullOrWhiteSpace(orNumber)) return;

            var result = mainForm.ShowMessage("Are you sure you want to return this sales invoice?", true);

            if (result == System.Windows.Forms.DialogResult.No) return;

            enterRemarksForm = new EnterRemarksForm(
                new ConfirmRemarksEventMessenger(ConfirmRemarksInvoked),
                orNumber,
                row);

            enterRemarksForm.ShowDialog();
        }

        private async void ConfirmRemarksInvoked(string orNumber, string remarks, DateTime date, DataGridViewRow row)
        {
            enterRemarksForm.Close();

            enterRemarksForm = null;

            try
            {
                var salesDtos = await controller.Find(orNumber);

                salesDtos.UserId = mainForm.UserDtos.UserId;

                var success = await salesReturnController.Save(
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

                    dgvItems.Rows.Remove(row);

                    mainForm.ShowMessage("Successfully returned.");
                }

            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }

        private void btnReturnItems_Click(object sender, EventArgs e)
        {
            var orNumber = "";

            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            if (row.Cells[0].Value == null) return;

            orNumber = row.Cells[0].Value.ToString();

            var salesReturnForm = new SalesReturnItemsForm(orNumber);

            mainForm.ChangeForm(salesReturnForm, true);
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

            //try
            //{
            //    mainForm.ShowProgressStatus();

            //    await InitializeSalesInvoice(key);
            //}
            //catch (Exception ex) { mainForm.HandleException(ex); }

            //finally { mainForm.ShowProgressStatus(false); }
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

            var siViewDetailForm = new SalesInvoiceViewDetailForm(poNumber);

            mainForm.ChangeForm(siViewDetailForm, true);
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
                    SearchInvoice(key);
                }
                catch { /*mainForm.HandleException(ex);*/ }
            }
            else timer.Interval += 300;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                //mainForm.ShowProgressStatus();

                //InitializePrivileges();

                //this.salesInvoiceDtosList = null;

                ConfirmDateRangeInvoked(from, to);
                //await InitializeSalesInvoice();

                started = true;
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }
    }
}
