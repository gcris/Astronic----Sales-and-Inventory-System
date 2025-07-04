using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Shared;
using CommonLibrary.Dtos;
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

namespace AstronicAutoSupplyInventory.Items
{
    public partial class ChangePriceForm : Form
    {
        private ItemController itemController = new ItemController();
        private CategoryController categoryController = new CategoryController();
        private ItemPriceController itemPriceController = new ItemPriceController();
        private SupplierController supplierController = new SupplierController();
        private UserController userController = new UserController();

        private List<ItemPriceDtos> itemPriceDtosList;
        //private IEnumerable<ItemDtos> itemDtosList;
        private int itemCount, categoryId, supplierId;
        private bool started, madeChanges, onSearch;
        private DateTime dateChanged;

        //private int pageSize = 20, pageNumber = 1, totalPage = 0;

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private IEnumerable<CategoryDtos> categoryList;
        private IEnumerable<SupplierDtos> supplierList;
        private bool isCategory;
        private bool isConfirmSelection;
        private SearchKeyEventMessenger searchKeyEventMessenger;
        private ProcessCmdEventMessenger listViewEventMessenger;
        private ProcessCmdEventMessenger confirmSelectionEventMessenger;

        public ChangePriceForm(DateTime dateChanged, int categoryId = 0)
        {
            this.categoryId = categoryId;

            this.dateChanged = dateChanged;

            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Enter:
                    if (pnlSelectCategoryOrSupplier.Visible)
                    {
                        return confirmSelectionEventMessenger();
                    }

                    break;
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
                case Keys.F1:
                    btnConfirm_Click(this, new EventArgs());

                    return true;
                case Keys.Escape:
                    if (pnlSelectCategoryOrSupplier.Visible)
                    {
                        pnlSelectCategoryOrSupplier.Visible = false;

                        return true;
                    }

                    btnCancel_Click(this, new EventArgs());

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

        public async Task InitializeItemPriceChanged()
        {
            this.itemPriceDtosList = new List<ItemPriceDtos>();

            if (categoryId > 0 && !started)
            {
                var category = await categoryController.Find(categoryId);

                if (category != null)
                {
                    SelectInvoked(category.CategoryId, category.Name, true);
                }
            }

            if (this.dateChanged == DateTime.MinValue) return;

            this.itemPriceDtosList.AddRange(await itemPriceController.GetPriceListByDate(this.categoryId, this.dateChanged));
        }

        public async Task InitializeItems(int categoryId, int supplierId = 0, string key = "")
        {
            if (categoryId < 1) return;

            var itemDtosList = await itemController.GetAllByCategoryAndSupplier(null, key, categoryId, supplierId);

            itemCount = itemDtosList.Count();

            dgvItems.Rows.Clear();

            if (itemCount < 1) return;

            itemDtosList = itemDtosList.OrderBy(item => item.PartNo);

            foreach (var item in itemDtosList)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = item.ItemId;

                row.Cells[0].Value = item.PartNo;

                row.Cells[1].Value = item.BrandName;

                row.Cells[2].Value = item.Model;

                row.Cells[3].Value = item.Make;

                row.Cells[4].Value = item.Made;

                row.Cells[5].Value = item.Size;

                row.Cells[6].Value = item.SupplierName;

                for (int i = 0; i < 7; i++) row.Cells[i].ReadOnly = true;

                decimal qoh = 0m, price1 = 0m, price2 = 0m, currentCost = 0m;

                var exists = false;

                if (this.itemPriceDtosList != null) exists = this.itemPriceDtosList.Any(price => price.ItemId == item.ItemId);

                ItemPriceDtos itemPrice = null;

                if (exists) itemPrice = this.itemPriceDtosList.FirstOrDefault(price => price.ItemId == item.ItemId);

                qoh = exists ? itemPrice.QuantityOnHand : item.QuantityOnHand;

                price1 = exists ? itemPrice.Price1 : item.Price1;

                price2 = exists ? itemPrice.Price2 : item.Price2;

                currentCost = exists ? itemPrice.CurrentCost : item.CurrentCost;

                row.Cells[7].Value = qoh.ToString("#,0.00;-#,0.00;''");

                row.Cells[7].Tag = item.QuantityOnHand;

                row.Cells[8].Value = price1.ToString("#,0.00;(#,0.00);''");

                row.Cells[8].Tag = item.Price1;

                row.Cells[9].Value = price2.ToString("#,0.00;(#,0.00);''");

                row.Cells[9].Tag = item.Price2;

                row.Cells[10].Value = currentCost.ToString("#,0.00;(#,0.00);''");

                row.Cells[10].Tag = item.CurrentCost;
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
            var supplierList = await supplierController.GetAll();

            this.supplierList = supplierList;
        }

        private async Task InitializeCategories()
        {
            var categoryList = await categoryController.GetAll();

            this.categoryList = categoryList;
        }

        private async void ChangePriceForm_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                await InitializeSupplier();

                await InitializeCategories();

                await InitializeItemPriceChanged();

                started = true;

                txtCategory.Focus();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
                //MessageBox.Show(ex.Message);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 7 || !started || onSearch || isConfirmSelection || mainForm.IsLoading) return;

            var row = dgvItems.Rows[e.RowIndex];

            var itemId = 0;

            int.TryParse(row.Tag.ToString(), out itemId);

            if (itemPriceDtosList == null) itemPriceDtosList = new List<ItemPriceDtos>();

            var exists = itemPriceDtosList.Any(item => item.ItemId == itemId);

            decimal qoh = 0m, price1 = 0m, price2 = 0m, currentCost = 0m;

            if (row.Cells[7].Value != null)
            {
                decimal.TryParse(row.Cells[7].Value.ToString(), out qoh);

                //if (!valid) row.Cells[7].Value = row.Cells[7].Tag;
            }

            if (row.Cells[8].Value != null)
            {
                decimal.TryParse(row.Cells[8].Value.ToString(), out price1);

                //if (!valid) row.Cells[8].Value = row.Cells[8].Tag;
            }

            if (row.Cells[9].Value != null)
            {
                decimal.TryParse(row.Cells[9].Value.ToString(), out price2);

                //if (!valid) row.Cells[9].Value = row.Cells[9].Tag;
            }

            if (row.Cells[10].Value != null)
            {
                decimal.TryParse(row.Cells[10].Value.ToString(), out currentCost);

                //if (!valid) row.Cells[10].Value = row.Cells[10].Tag;
            }

            if (qoh <= 0 && price1 <= 0 && price2 <= 0 && currentCost <= 0) return;

            if (exists)
            {
                var itemPrice = itemPriceDtosList.FirstOrDefault(item => item.ItemId == itemId);

                itemPrice.QuantityOnHand = qoh;

                if (price1 > 0) itemPrice.Price1 = price1;

                if (price2 > 0) itemPrice.Price2 = price2;

                if (currentCost > 0) itemPrice.CurrentCost = currentCost;                
            }
            else 
                itemPriceDtosList.Add(new ItemPriceDtos
                {
                    Price1 = price1,
                    Price2 = price2,
                    CurrentCost = currentCost,
                    ItemId = itemId,
                    QuantityOnHand = qoh
                });

            if (!madeChanges && !isConfirmSelection) madeChanges = true;

            Trace.WriteLine("Please count.");
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!started || !txtSearch.Focused) return;

            try
            {
                //mainForm.ShowProgressStatus();
                //pageNumber = 1;
                onSearch = true;

                await InitializeItems(categoryId, supplierId, txtSearch.Text);
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            if (timer.Interval >= 200 && timer.Enabled)
            {
                timer.Stop();

                timer.Enabled = false;

                try
                {
                    mainForm.ShowProgressStatus();

                    await InitializeItems(categoryId, supplierId, txtSearch.Text);
                }
                catch (Exception ex)
                {
                    mainForm.HandleException(ex);
                }

                finally 
                { 
                    mainForm.ShowProgressStatus(false);

                    onSearch = false;
                }
            }
            else timer.Interval += 100;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ChangePriceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var cancel = false;

            if (madeChanges)
            {
                var message = mainForm.ShowMessage("Are you sure you want to discard the changes made?", true);

                cancel = message == System.Windows.Forms.DialogResult.No;
            }

            e.Cancel = cancel;
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            btnConfirm.Focus();
            if (!madeChanges) return;

            var result = mainForm.ShowMessage("Are you sure you want to save changes?", true);
            if (result == System.Windows.Forms.DialogResult.No) return;
            var msg = "";
            var success = false;
            try
            {
                mainForm.ShowProgressStatus();

                var itemPriceDtosList = this.itemPriceDtosList.Where(item => 
                    item.QuantityOnHand > 0 || item.Price1 > 0 || item.Price2 > 0 || item.CurrentCost > 0);

                success = await itemPriceController.Save(itemPriceDtosList, mainForm.NotifierEventMessenger);

                msg = success ? "Successfully saved." : "Failed to save. Please contact the administartor.";

                if (success)
                {
                    Trace.WriteLine("Price change count: " + itemPriceDtosList.Count());
                    foreach (var item in itemPriceDtosList)
                    {
                        var queryItem = await itemController.Find(item.ItemId);

                        var found = false;

                        var index = 0;

                        while (index < dgvItems.Rows.Count && !found)
                        {
                            var itemId = 0;

                            int.TryParse(dgvItems.Rows[index].Tag.ToString(), out itemId);

                            found = itemId == item.ItemId;

                            if (!found) index++;
                        }

                        if (found)
                        {
                            var oldQoh = 0m;

                            decimal.TryParse(dgvItems.Rows[index].Cells[7].Tag.ToString(), out oldQoh);

                            if (queryItem.QuantityOnHand != oldQoh)
                            {
                                await userController.SaveItemActivity(
                                    new UserActivityDtos
                                    {
                                        Action = string.Format("Quantity on hand has been updated from {0} to {1}", oldQoh, item.QuantityOnHand),
                                        CurrentStock = queryItem.QuantityOnHand,
                                        Date = DateTime.Now,
                                        ItemId = queryItem.ItemId,
                                        Quantity = item.QuantityOnHand,
                                        UserId = mainForm.UserDtos.UserId
                                    });
                            }
                        }
                    }

                    await userController.SaveActivity(
                        string.Format("Price Change for Category '{0}'", txtCategory.Text),
                        mainForm.UserDtos.UserId);

                    this.madeChanges = false;

                    this.itemPriceDtosList = null;

                    this.started = false;

                    await InitializeItems(categoryId, supplierId);

                    this.started = true;
                }
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);

                msg = "Sorry for the inconvenience. Some category are not deleted because of internal issue. " +
                    "Contact the administrator for assistance. ";
            }

            finally
            {
                mainForm.ShowProgressStatus(false);

                mainForm.ShowMessage(msg, false, !success);
            }
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

            //if (key == "SEARCH HERE!") key = "";

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
            if (madeChanges)
            {
                var message = mainForm.ShowMessage("Are you sure you want to discard the changes made?", true);

                if (message == System.Windows.Forms.DialogResult.No) return;

                this.itemPriceDtosList = null;

                txtSearch.Text = string.Empty;

                madeChanges = false;
            }

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
            txtSearch.Text = string.Empty;
            txtSearch.Focus();
            await InitializeItems(categoryId, supplierId);
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
                if (madeChanges)
                {
                    var message = mainForm.ShowMessage("Are you sure you want to discard the changes made?", true);

                    if (message == System.Windows.Forms.DialogResult.No) return;

                    this.itemPriceDtosList = null;

                    txtSearch.Text = string.Empty;

                    madeChanges = false;
                }

                txtSupplier.Text = "ALL SUPPLIERS";

                supplierId = 0;

                pnlSelectCategoryOrSupplier.Controls.Clear();

                pnlSelectCategoryOrSupplier.Visible = false;

                await InitializeItems(categoryId, supplierId);
            }
        }

        private void dgvItems_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (onSearch) onSearch = false;
            if (isConfirmSelection) isConfirmSelection = false;
        }
    }
}
