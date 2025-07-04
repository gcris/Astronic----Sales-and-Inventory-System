using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Items;
using CommonLibrary.Dtos;
using Infrastructure;
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
//using System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace AstronicAutoSupplyInventory.Shared
{
    public partial class SearchItemForm : Form
    {
        private ItemController itemController = new ItemController();
        private CategoryController categoryController = new CategoryController();
        //private IEnumerable<ItemDtos> itemDtosList;
        private bool started;
        //private int pageSize = 20, pageNumber = 1, totalPage = 0;

        private readonly ConfirmSearchItemEventMessenger confirmSearchItemEventMessenger;
        private readonly bool isPurchaseOrder, isSalesInvoice;

        private int categoryId;
        private bool isLoading;
        private IEnumerable<CategoryDtos> categoryList;
        private IEnumerable<ItemDtos> itemList = new List<ItemDtos>();
        private bool isConfirmSelection;
        private SearchKeyEventMessenger searchKeyEventMessenger;
        private ProcessCmdEventMessenger listViewEventMessenger;
        private ProcessCmdEventMessenger confirmSelectionEventMessenger;
        private MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        //private int itemCount;

        private Timer typingTimer;  // Timer to handle debounce
        private const int TypingDelay = 100; // Delay in milliseconds (e.g., 500ms)

        public SearchItemForm(ConfirmSearchItemEventMessenger confirmSearchItemEventMessenger, bool isPurchaseOrder = false, bool isSalesInvoice = false)
        {
            this.confirmSearchItemEventMessenger = confirmSearchItemEventMessenger;

            this.isPurchaseOrder = isPurchaseOrder;
            this.isSalesInvoice = isSalesInvoice;

            // Initialize the Timer
            typingTimer = new Timer();
            typingTimer.Interval = TypingDelay;
            typingTimer.Tick += TypingTimer_Tick;

            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Enter:
                    if (pnlSelectCategory.Visible)
                    {
                        return confirmSelectionEventMessenger();
                    }

                    btnConfirm_Click(this, new EventArgs());

                    return true;
                case Keys.F1:
                    if (pnlTransactions.Visible)
                    {
                        btnHistory_Click(btnHistory, new EventArgs());
                    }

                    return true;
                case Keys.Escape:
                    if (pnlSelectCategory.Visible)
                    {
                        pnlSelectCategory.Visible = false;

                        return true;
                    }

                    this.Close();

                    return true;
                case Keys.Down:
                    if (pnlSelectCategory.Visible)
                    {
                        return listViewEventMessenger();
                    }

                    if (dgvItems.Focused) break;

                    dgvItems.Focus();

                    return true;
                case Keys.Alt | Keys.S:
                    txtSearch.Focus();
                    txtSearch.SelectAll();

                    return true;
                case Keys.Alt | Keys.C:
                    txtCategory.Focus();

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
                case Keys.Tab:
                    if (pnlSelectCategory.Visible)
                    {
                        var msg = txtCategory.Text;

                        if (msg.Length == 0 || msg == "SELECT CATEGORY") return true;

                        return confirmSelectionEventMessenger();
                    }

                    break;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task InitializeCategories()
        {
            var categoryList = await categoryController.GetAll();

            this.categoryList = categoryList;
        }

        public async Task InitializeItems(int categoryId, string key = "")
        {
            if (categoryId < 1) return;

            itemList = await itemController.GetAllByCategoryAndSupplier(null, key, categoryId);

            var itemCount = itemList.Count();

            dgvItems.Rows.Clear();

            foreach (var item in itemList)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = item.ItemId;

                row.Cells[0].Value = item.PartNo;

                row.Cells[1].Value = item.BrandName;

                row.Cells[2].Value = item.Model;

                row.Cells[3].Value = item.Make;

                row.Cells[4].Value = item.Made;

                row.Cells[5].Value = item.UnitOfMeasure.ToString();

                row.Cells[6].Value = item.Size;

                row.Cells[7].Value = item.OtherPartNo;

                row.Cells[8].Value = item.SupplierName;

                if (item.MinimumStock > 0)
                {
                    var percentage = item.QuantityOnHand / item.MinimumStock;

                    row.Cells[9].Style.ForeColor = percentage <= 0.4m ? Color.Red :
                        percentage <= 0.7m ? Color.Orange : Color.Green;
                }

                row.Cells[9].Value = item.QuantityOnHand;

                row.Cells[10].Value = item.CurrentCost.ToString("#,0.00;(#,0.00);''");

                row.Cells[11].Value = item.Price1.ToString("#,0.00;(#,0.00);''");
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

        private async Task DoSearch()
        {
            var key = txtSearch.Text;

            await InitializeItems(categoryId, key);
        }

        private void Search(string key = "")
        {
            key = key.ToLower();

            if (itemList == null || itemList.Count() == 0) return;

            IEnumerable<ItemDtos> items = this.itemList.Where(t => t.BrandName.ToLower().Contains(key.ToLower())
                          || t.Made.ToLower().Contains(key.ToLower())
                          || t.Make.ToLower().Contains(key.ToLower())
                          || t.Model.ToLower().Contains(key.ToLower())
                          || t.PartNo.ToLower().Contains(key.ToLower())
                          || t.OtherPartNo.ToLower().Contains(key.ToLower())
                          || t.Size.ToLower().Contains(key.ToLower())).ToList();

            dgvItems.Rows.Clear();

            items = items.OrderBy(item => item.PartNo);

            foreach (var item in items)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = item.ItemId;

                row.Cells[0].Value = item.PartNo;

                row.Cells[1].Value = item.BrandName;

                row.Cells[2].Value = item.Model;

                row.Cells[3].Value = item.Make;

                row.Cells[4].Value = item.Made;

                row.Cells[5].Value = item.UnitOfMeasure.ToString();

                row.Cells[6].Value = item.Size;

                row.Cells[7].Value = item.OtherPartNo;

                row.Cells[8].Value = item.SupplierName;

                if (item.MinimumStock > 0)
                {
                    var percentage = item.QuantityOnHand / item.MinimumStock;

                    row.Cells[9].Style.ForeColor = percentage <= 0.4m ? Color.Red :
                        percentage <= 0.7m ? Color.Orange : Color.Green;
                }

                row.Cells[9].Value = item.QuantityOnHand;

                row.Cells[10].Value = item.CurrentCost.ToString("#,0.00;(#,0.00);''");

                row.Cells[11].Value = item.Price1.ToString("#,0.00;(#,0.00);''");
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

        public void ShowProgressStatus(bool visible = true)
        {
            isLoading = visible;

            lblStatus.Text = visible ? "Loading, please wait..." : "Ready.";

            progressBar.Visible = visible;

            pnlStatus.BackColor = visible ? Color.Orange : Color.DodgerBlue;
        }

        public DialogResult ShowMessage(string message, bool question = false, bool error = false)
        {
            var floaterFormUI = (FloatForm)Application.OpenForms["FloatForm"];

            if (floaterFormUI != null) floaterFormUI.Close();

            return MessageBox.Show(this, message,
                "AASIS App",
                question ? MessageBoxButtons.YesNo : MessageBoxButtons.OK,
                error ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }

        public void HandleException(Exception ex)
        {
            ShowProgressStatus(false);

            var msg = ex.Message;

            if (ex.HResult == -2146233087) msg = "Operation failed. Selected items cannot be deleted because they are referenced to the other process.";
            else if (ex.HResult == -2146232060) msg = "Operation failed. You're sql server is not running. " +
                "Please contact the administrator for correction. Thank you.";
            else if (ex.HResult == -2146233079) msg = string.Empty;

            if (msg.Length > 0) ShowMessage(msg);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (timer.Interval >= 100 && timer.Enabled)
            {
                timer.Stop();

                timer.Enabled = false;

                try
                {
                     Search(txtSearch.Text);
                }
                catch { /*mainForm.HandleException(ex);*/ }
            }
            else timer.Interval += 100;
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


        // Triggered when the timer elapses (i.e., the user stopped typing)
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer
            typingTimer.Stop();

            // Perform the action, e.g., call search function
             Search(txtSearch.Text);
        }
        private async void SearchItemForm_Load(object sender, EventArgs e)
        {
            try
            {
                ShowProgressStatus();
                dgvItems.AutoGenerateColumns = false;
                await InitializeCategories();
                txtCategory.Focus();
                started = true;
                pnlTransactions.Visible = mainForm != null;
                var hasPrivilage = false;
                if (mainForm != null)
                {
                    if (mainForm.UserDtos != null)
                    {
                        hasPrivilage = mainForm.UserDtos.UserPrivilegeDtosList
                            .Any(d => d.Action == (isPurchaseOrder ? "Encode Purchase Order" :
                                "Encode Sales Invoice"));
                    }
                }
                dgvItems.Columns[10].Visible = hasPrivilage;
                dgvItems.Columns[10].HeaderText = isPurchaseOrder ? "Current Cost" : "Net Price";
                dgvItems.Columns[11].HeaderText = "Selling Price";
            }
            catch (Exception ex) { HandleException(ex); }

            finally { ShowProgressStatus(false); }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (isLoading || dgvItems.SelectedRows.Count < 1) return;

            var idList = new List<int>();

            foreach (DataGridViewRow row in dgvItems.SelectedRows)
            {
                var itemId = 0;
                int.TryParse(row.Tag.ToString(), out itemId);
                if (itemId < 1) return;
                if (isSalesInvoice)
                {
                    var qty = 0m;
                    var qtyStr = row.Cells[9].Value.ToString();
                    decimal.TryParse(qtyStr, out qty);
                    if (qty <= 0)
                    {
                        var result = ShowMessage(string.Format("Item {0} is out of stock. Are you sure you want to add the item?", row.Cells[0].Value), true);
                        if (result == System.Windows.Forms.DialogResult.No) continue;
                    }
                }
                idList.Add(itemId);
            }
            if (idList.Count < 1) return;
            confirmSearchItemEventMessenger(idList);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (isLoading) return;

            this.Close();
        }
        private void btnPaging_Click(object sender, EventArgs e)
        {
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

            //await InitializeItems(categoryId, key);

            //ActiveControl = btn;
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
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
            catch (Exception ex) { HandleException(ex); }
        }

        private void SearchPanel(IEnumerable<object> myList)
        {
            if (pnlSelectCategory.Visible) return;

            var selectCategoryUI = new SelectCategoryOrSupplierUI(new SelectCategoryOrSupplierEventMessenger(SelectInvoked), myList);

            pnlSelectCategory.Controls.Add(selectCategoryUI);

            pnlSelectCategory.Visible = true;

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

            txtSearch.Text = string.Empty;
            txtSearch.Focus();

            await DoSearch();

            isConfirmSelection = false;
        }

        private void btnViewCategories_Click(object sender, EventArgs e)
        {
            pnlSelectCategory.Visible = false;

            SearchPanel(this.categoryList);
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            if (mainForm == null) return;
            var itemTransactionHistoryForm = (ItemTransactionHistoryForm)Application.OpenForms["ItemTransactionHistoryForm"];

            if (itemTransactionHistoryForm != null) itemTransactionHistoryForm.Close();

            if (!pnlTransactions.Visible) return;

            var itemId = 0;

            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            int.TryParse(row.Tag.ToString(), out itemId);

            if (itemId < 1) return;

            var itemHistory = new ItemTransactionHistoryForm(itemId);

            mainForm.ChangeForm(itemHistory, true);

            mainForm.BringToFront();
        }
    }
}