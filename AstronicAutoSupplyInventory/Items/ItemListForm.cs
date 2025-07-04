using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Shared;
using CommonLibrary.Dtos;
using CommonLibrary.Enums;
using CommonLibrary.Services;
using InventoryServices.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace AstronicAutoSupplyInventory.Items
{
    public partial class ItemListForm : Form
    {
        private ItemController itemController = new ItemController();
        private CategoryController categoryController = new CategoryController();
        private SupplierController supplierController = new SupplierController();
        private UserController userController = new UserController();

        private int itemCount, categoryId, supplierId;
        private bool started, doDateRange;
        private IEnumerable<CategoryDtos> categoryList;
        private IEnumerable<ItemDtos> itemList = new List<ItemDtos>();
        private DateTime from;
        private DateTime to;

        private SearchKeyEventMessenger searchKeyEventMessenger;
        private ProcessCmdEventMessenger listViewEventMessenger;
        private ProcessCmdEventMessenger confirmSelectionEventMessenger;

        //private int pageSize = 20, pageNumber = 1, totalPage = 0;

        private DateRangeForm dateRangeForm;

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private bool isConfirmSelection, isCategory;
        private IEnumerable<SupplierDtos> supplierList;

        private Timer typingTimer;  // Timer to handle debounce
        private const int TypingDelay = 100; // Delay in milliseconds (e.g., 500ms)

        public ItemListForm(int categoryId = 0)
        {
           
            this.categoryId = categoryId;

            // Initialize the Timer
            typingTimer = new Timer();
            typingTimer.Interval = TypingDelay;
            typingTimer.Tick += TypingTimer_Tick;

            InitializeComponent();

            var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            Width = screen.Width - 50;

            Height = screen.Height - 50;
        }

        public void NotifierInvoked(bool success) 
        {
            if (success) btnRefresh_Click(btnRefresh, new EventArgs());
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.F1:
                    btnAdd_Click(this, new EventArgs());

                    return true;
                case Keys.F2:
                    btnEdit_Click(this, new EventArgs());

                    return true;
                case Keys.F3:
                    btnDelete_Click(this, new EventArgs());

                    return true;
                case Keys.F4:
                    btnHistory_Click(this, new EventArgs());

                    return true;
                case Keys.F5:
                    btnRefresh_Click(this, new EventArgs());

                    return true;
                case Keys.Tab:
                    if (pnlSelectCategoryOrSupplier.Visible)
                    {
                        var msg = "";

                        if (isCategory)
                        {
                            msg = txtCategory.Text;
                        }
                        else
                        {
                            msg = txtSupplier.Text;
                        }

                        if (msg.Length == 0 || msg == "SELECT CATEGORY" || msg == "ALL SUPPLIERS") return true;

                        return confirmSelectionEventMessenger();
                    }

                    break;
                case Keys.Enter:
                    if (pnlSelectCategoryOrSupplier.Visible)
                    {
                        return confirmSelectionEventMessenger();
                    }

                    btnViewDetails_Click(btnViewDetails, new EventArgs());

                    return true;
                case Keys.Escape:
                    if (pnlSelectCategoryOrSupplier.Visible)
                    {
                        pnlSelectCategoryOrSupplier.Visible = false;

                        return true;
                    }

                    btnClose2_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.S:
                    txtSearch.Focus();
                    txtSearch.SelectAll();

                    return true;
                case Keys.Back:
                    if (pnlSelectCategoryOrSupplier.Visible)
                    {
                        if (isCategory)
                        {
                            if (txtCategory.Focused) return false;

                            txtCategory.Focus();

                            if (txtCategory.Text.Length > 0)
                            {
                                txtCategory.Text = txtCategory.Text.Substring(0, txtCategory.Text.Length - 1);
                            }
                        }
                        else
                        {
                            if (txtSupplier.Focused) return false;

                            txtSupplier.Focus();

                            if (txtSupplier.Text.Length > 0)
                            {
                                txtSupplier.Text = txtSupplier.Text.Substring(0, txtSupplier.Text.Length - 1);
                            }
                        }

                        return true;
                    }

                    break;
                case Keys.Alt | Keys.C:
                    txtCategory.Focus();

                    return true;
                case Keys.Alt | Keys.U:
                    txtSupplier.Focus();

                    return true;
                case Keys.Alt | Keys.D:
                    lnkMyInventory_LinkClicked(this, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
                case Keys.Down:
                    if (pnlSelectCategoryOrSupplier.Visible)
                    {
                       return listViewEventMessenger();
                    }

                    if (dgvItems.Focused) break;

                    dgvItems.Focus();

                    return true;
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
                btnAdd.Enabled = privileges.Any(privilege => privilege.Action == "Add Item" && privilege.IsEnable);

                btnEdit.Enabled = privileges.Any(privilege => privilege.Action == "Edit Item" && privilege.IsEnable);

                btnDelete.Enabled = privileges.Any(privilege => privilege.Action == "Delete Item" && privilege.IsEnable);

                btnHistory.Enabled = privileges.Any(privilege => privilege.Action == "Change Item Price" && privilege.IsEnable);
            }
        }

        public async Task InitializeItems(int categoryId, int supplierId = 0, string key = "")
        {
            if (categoryId < 1) return;

            this.categoryId = categoryId;

            this.supplierId = supplierId;

            itemList = await itemController.GetAllByCategoryAndSupplier(null, key, categoryId, supplierId);

            if (doDateRange)
            {
                itemList = itemList.Where(item => item.LastUpdate.Date >= from.Date &&
                    item.LastUpdate.Date <= to.Date);

                doDateRange = false;
            }
            
            itemCount = itemList.Count();

            dgvItems.Rows.Clear();

            if (itemCount < 1) return;

            itemList = itemList.OrderBy(item => item.PartNo);

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

                row.Cells[7].Value = item.SupplierName;

                row.Cells[8].Value = item.QuantityOnHand.ToString("#,0.00;-#,0.00");

                if (item.MinimumStock > 0)
                {
                    var percentage = item.QuantityOnHand / item.MinimumStock;

                    row.Cells[8].Style.ForeColor = percentage <= 0.4m ? Color.Red :
                        percentage <= 0.7m ? Color.Orange : Color.Green;
                }

                row.Cells[9].Value = item.MinimumStock;

                row.Cells[10].Value = item.OtherPartNo;

                row.Cells[11].Value = item.Price1.ToString("#,0.00;(#,0.00);''");

                row.Cells[12].Value = item.Price2.ToString("#,0.00;(#,0.00);''");

                row.Cells[13].Value = item.CurrentCost.ToString("#,0.00;(#,0.00);''");

                row.Cells[14].Value = item.LastUpdate.ToShortDateString();
            }

            if (categoryId > 0 && !started)
            {
                var category = await categoryController.Find(categoryId);

                if (category != null)
                {
                    SelectInvoked(category.CategoryId, category.Name, true);
                }
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

        private async Task InitializeSupplier()
        {
            var supplierDtosList = await supplierController.GetAll();

            this.supplierList = supplierDtosList;
        }

        private async Task InitializeCategories()
        {
            var queryCategoryList = await categoryController.GetAll();

            this.categoryList = queryCategoryList;

            //txtca.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            //cboCategory.AutoCompleteSource = AutoCompleteSource.ListItems;

            //if (queryCategoryList.Count() > 0)
            //{
            //    var list = new List<CategoryDtos>();

            //    list.AddRange(queryCategoryList);

            //    list.Add(new CategoryDtos { CategoryId = 0, Name = "SELECT CATEGORY" });

            //    cboCategory.DataSource = list;

            //    cboCategory.DisplayMember = "Name";

            //    cboCategory.ValueMember = "CategoryId";

            //    cboCategory.SelectedValue = categoryId;
            //}
        }

        private async Task DoSearch()
        {
            var key = txtSearch.Text;

            await InitializeItems(categoryId, supplierId, key);
        }

        private async void Search(string key = "")
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

            if (doDateRange)
            {
                items = items.Where(item => item.LastUpdate.Date >= from.Date &&
                    item.LastUpdate.Date <= to.Date);

                doDateRange = false;
            }

            itemCount = items.Count();

            dgvItems.Rows.Clear();

            if (itemCount < 1) return;

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

                row.Cells[7].Value = item.SupplierName;

                row.Cells[8].Value = item.QuantityOnHand.ToString("#,0.00;-#,0.00");

                if (item.MinimumStock > 0)
                {
                    var percentage = item.QuantityOnHand / item.MinimumStock;

                    row.Cells[8].Style.ForeColor = percentage <= 0.4m ? Color.Red :
                        percentage <= 0.7m ? Color.Orange : Color.Green;
                }

                row.Cells[9].Value = item.MinimumStock;

                row.Cells[10].Value = item.OtherPartNo;

                row.Cells[11].Value = item.Price1.ToString("#,0.00;(#,0.00);''");

                row.Cells[12].Value = item.Price2.ToString("#,0.00;(#,0.00);''");

                row.Cells[13].Value = item.CurrentCost.ToString("#,0.00;(#,0.00);''");

                row.Cells[14].Value = item.LastUpdate.ToShortDateString();
            }

            if (categoryId > 0 && !started)
            {
                var category = await categoryController.Find(categoryId);

                if (category != null)
                {
                    SelectInvoked(category.CategoryId, category.Name, true);
                }
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

        private async void ItemListForm_Load(object sender, EventArgs e)
        {
            
            try
            {
                mainForm.ShowProgressStatus();

                InitializePrivileges();

                await InitializeSupplier();

                await InitializeCategories();

                await DoSearch();

                txtCategory.Focus();

                started = true;
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!started || !txtSearch.Focused) return;

            //timer.Enabled = true;

            //timer.Interval = 100;

            //timer.Start();

            try
            {
                //mainform.showprogressstatus();

                // pagenumber = 1;

                //await DoSearch();

                typingTimer.Stop();
                typingTimer.Start();

                if (txtSearch.Text == "") await DoSearch();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer
            typingTimer.Stop();

            // Perform the action, e.g., call search function
            Search(txtSearch.Text);
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

        private async void cboCategoryAndSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!started) return;

            txtSearch.Text = string.Empty;

            //itemDtosList = null;

            try
            {
                mainForm.ShowProgressStatus();

                await DoSearch();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnHistory.Enabled) return;

            var changePriceHistoryForm = new ChangePriceHistoryForm(categoryId);

            mainForm.ChangeForm(changePriceHistoryForm);
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                await DoSearch();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnEdit.Enabled) return;

            var itemId = 0;

            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            int.TryParse(row.Tag.ToString(), out itemId);

            if (itemId < 1) return;

            var addEditItemForm = new AddEditItemForm(new AddNewEventMessenger(NewItemInvoked), itemId);

            addEditItemForm.Text = "Edit Item";

            mainForm.ChangeForm(addEditItemForm, true);
        }

        private async void NewItemInvoked(int id)
        {
            var item = await itemController.Find(id);

            if (item == null) return;

            txtSearch.Text = item.PartNo;

            await DoSearch();

            txtSearch.SelectionLength = txtSearch.Text.Length;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnAdd.Enabled) return;

            var addEditItemForm = new AddEditItemForm(new AddNewEventMessenger(NewItemInvoked));

            mainForm.ChangeForm(addEditItemForm, true);
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnDelete.Enabled) return;

            var result = mainForm.ShowMessage("Are you sure you want to delete?", true);

            if (result == System.Windows.Forms.DialogResult.No) return;

            var msg = "";

            var success = false;

            try
            {
                mainForm.ShowProgressStatus();

                foreach (DataGridViewRow row in dgvItems.SelectedRows) 
                {
                    var itemId = 0;

                    int.TryParse(row.Tag.ToString(), out itemId);

                    if (itemId < 1) continue;

                    success = await itemController.Delete(itemId);

                    if (!success) break;
                    else await userController.SaveActivity(
                        string.Format("Deleted Item with Part # '{0}'", row.Cells[0].Value),
                        mainForm.UserDtos.UserId);

                    dgvItems.Rows.Remove(row);
                }

                msg = success ? "Successfully deleted." : "Cannot be deleted because the selected item(s) has referenced."; 
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);

                msg = "Sorry for the inconvenience. Some item(s) is/are not deleted because of internal issue. " +
                    "Contact the administrator for assistance. ";
            }

            finally 
            {
                mainForm.ShowMessage(msg, false, !success);

                mainForm.ShowProgressStatus(false);
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
                if (dateRangeForm == null) return;

                dateRangeForm.Close();

                if (from > DateTime.MinValue && to > DateTime.MinValue)
                {
                    this.from = from;

                    this.to = to;

                    doDateRange = true;

                    await InitializeItems(categoryId, supplierId, txtSearch.Text);
                }
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            var itemTransactionHistoryForm = (ItemTransactionHistoryForm)Application.OpenForms["ItemTransactionHistoryForm"];

            if (itemTransactionHistoryForm != null) itemTransactionHistoryForm.Close();

            if (mainForm.IsLoading || !btnViewDetails.Enabled) return;
            
            var itemId = 0;

            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            int.TryParse(row.Tag.ToString(), out itemId);

            if (itemId < 1) return;

            var itemHistory = new ItemTransactionHistoryForm(itemId);

            mainForm.ChangeForm(itemHistory, true);

            #region Upload item from excel
            /*
            var open = new OpenFileDialog
            {
                Filter = "Excel File(*.xlsx)|*.xlsx",
                Title = "Upload Excel File"
            };
            var openResult = open.ShowDialog();
            if (openResult == DialogResult.Cancel
                || openResult == DialogResult.None) return;

            var success = false;

            var msg = "";

            try
            {
                var excel = ExcelDb.GetInstance();

                excel.Location = open.FileName;

                excel.ConnectToExcel();

                var dr = await excel.Read(string.Format("SELECT * FROM [{0}]", excel.GetTableNames()[0]));

                var itemDtosList = new List<ItemDtos>();

                CategoryDtos categoryDtos = null; 

                var random = new Random();

                while (await dr.ReadAsync())
                {
                    var categoryName = dr[0].ToString().Split(new [] { '3' });

                    if (categoryDtos == null) 
                        categoryDtos = new CategoryDtos 
                        {
                            Name = categoryName[0].Trim(), 
                            LastUpdate = DateTime.Now 
                        };

                    UnitOfMeasure unitOfMeasure = 0;

                    Enum.TryParse(dr[6].ToString(), out unitOfMeasure);

                    var itemDtos = new ItemDtos
                    {
                        CategoryName = categoryName[0].Trim(),
                        PartNo = dr[1].ToString(),
                        BrandName = dr[2].ToString(),
                        Model = dr[3].ToString(),
                        Make = dr[4].ToString(),
                        Made = dr[5].ToString(),
                        Size = dr[7].ToString(),
                        QuantityOnHand = decimal.Parse(dr[8].ToString()),
                        UnitOfMeasure = unitOfMeasure,
                        CurrentCost = random.Next(200, 800)
                    };

                    itemDtos.Price1 = itemDtos.CurrentCost + (itemDtos.CurrentCost * 0.7m);

                    itemDtos.Price2 = itemDtos.Price1 + (itemDtos.Price1 * 0.5m);

                    var itemPriceDtosList = new List<ItemPriceDtos>();

                    itemPriceDtosList.Add(new ItemPriceDtos
                    {
                        Price1 = itemDtos.Price1,
                        Price2 = itemDtos.Price2,
                        CurrentCost = itemDtos.CurrentCost
                    });

                    itemDtos.ItemPriceDtosList = itemPriceDtosList;

                    itemDtosList.Add(itemDtos);
                }

                var categoryId = 0;

                if (categoryDtos != null) categoryId = await categoryController.Save(categoryDtos);

                foreach (var itemDtos in itemDtosList)
                {
                    itemDtos.CategoryId = categoryId;

                    var id = await new ItemController().Save(itemDtos);

                    if (id < 1) break;
                }

                msg = success ? "Successfully saved." : "Failed to import.";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            finally
            {
                mainForm.ShowMessage(msg, false, !success);
            }
             */
            #endregion
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            pnlDateRange.Visible = false;

            from = DateTime.MinValue;

            to = DateTime.MinValue;
        }

        private void btnClose2_Click(object sender, EventArgs e)
        {
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

            //await InitializeItems(categoryId, supplierId, key);

            //ActiveControl = btn;
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }

        private void cboCategory_KeyPress(object sender, KeyPressEventArgs e)
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

                pnlSelectCategoryOrSupplier.Controls.Clear();

                pnlSelectCategoryOrSupplier.Visible = false;
            }
        }

        private void txtCategory_TextChanged(object sender, EventArgs e)
        {
            var txtBox = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(txtBox.Text) || isConfirmSelection || !started) return;

            var isCategory = txtBox.Name == txtCategory.Name;

            if (isCategory && txtBox.Text == "SELECT CATEGORY") return;
            if (!isCategory && txtBox.Text == "ALL SUPPLIERS") return;

            if (this.isCategory != isCategory && pnlSelectCategoryOrSupplier.Visible)
            {
                pnlSelectCategoryOrSupplier.Controls.Clear();

                pnlSelectCategoryOrSupplier.Visible = false;
            }

            this.isCategory = isCategory;

            try
            {
                if (searchKeyEventMessenger != null) searchKeyEventMessenger(txtBox.Text);

                if (isCategory)
                {
                    pnlSelectCategoryOrSupplier.Location = new Point(5, pnlSelectCategoryOrSupplier.Location.Y);

                    SearchPanel(this.categoryList, isCategory);
                }
                else
                {
                    pnlSelectCategoryOrSupplier.Location = new Point(414, pnlSelectCategoryOrSupplier.Location.Y);

                    SearchPanel(this.supplierList, isCategory);
                }
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }

        private void SearchPanel(IEnumerable<object> myList, bool isCategory)
        {
            if (pnlSelectCategoryOrSupplier.Visible) return;

            var selectCategoryOrSupplierUI = new SelectCategoryOrSupplierUI(new SelectCategoryOrSupplierEventMessenger(SelectInvoked), myList, isCategory);

            pnlSelectCategoryOrSupplier.Controls.Add(selectCategoryOrSupplierUI);

            pnlSelectCategoryOrSupplier.Visible = true;

            selectCategoryOrSupplierUI.Dock = DockStyle.Fill;

            searchKeyEventMessenger = new SearchKeyEventMessenger(selectCategoryOrSupplierUI.SearchKeyInvoked);

            listViewEventMessenger = new ProcessCmdEventMessenger(selectCategoryOrSupplierUI.ListViewFocusInvoked);

            confirmSelectionEventMessenger = new ProcessCmdEventMessenger(selectCategoryOrSupplierUI.ConfirmSelectionInvoked);
        }

        private async void SelectInvoked(int id, string name, bool isCategory)
        {
            pnlSelectCategoryOrSupplier.Controls.Clear();

            pnlSelectCategoryOrSupplier.Visible = false;

            isConfirmSelection = true;

            if (isCategory)
            {
                this.categoryId = id;

                txtCategory.Text = name;
            }
            else
            {
                this.supplierId = id;

                txtSupplier.Text = name;
            }

            isConfirmSelection = false;
            txtSearch.Text = string.Empty;
            txtSearch.Focus();
            await DoSearch();
        }

        private void btnViewCategoriesOrSupplier_Click(object sender, EventArgs e)
        {
            pnlSelectCategoryOrSupplier.Visible = false;

            var isCategory = ((Button)sender).Name == btnViewCategories.Name;

            if (this.isCategory != isCategory)
            {
                pnlSelectCategoryOrSupplier.Controls.Clear();
            }

            if (isCategory)
            {
                pnlSelectCategoryOrSupplier.Location = new Point(5, pnlSelectCategoryOrSupplier.Location.Y);

                SearchPanel(this.categoryList, isCategory);
            }
            else
            {
                pnlSelectCategoryOrSupplier.Location = new Point(414, pnlSelectCategoryOrSupplier.Location.Y);

                SearchPanel(this.supplierList, isCategory);
            }

            this.isCategory = isCategory;
        }

        private void txtSupplier_Enter(object sender, EventArgs e)
        {
            if (txtSupplier.Text == "ALL SUPPLIERS")
            {
                txtSupplier.Text = string.Empty;
            }
        }

        private async void txtSupplier_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSupplier.Text))
            {
                supplierId = 0;

                txtSupplier.Text = "ALL SUPPLIERS";

                pnlSelectCategoryOrSupplier.Controls.Clear();

                pnlSelectCategoryOrSupplier.Visible = false;

                await DoSearch();
            }
        }
    }
}
