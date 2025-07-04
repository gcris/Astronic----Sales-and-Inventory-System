using AstronicAutoSupplyInventory.Categories;
using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Shared;
using AstronicAutoSupplyInventory.Supplier;
using CommonLibrary.Dtos;
using CommonLibrary.Enums;
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

namespace AstronicAutoSupplyInventory.Items
{
    public partial class AddEditItemForm : Form
    {
        private ItemController itemController = new ItemController();
        private CategoryController categoryController = new CategoryController();
        private SupplierController supplierController = new SupplierController();
        private UserController userController = new UserController();

        private readonly int itemId;
        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private readonly AddNewEventMessenger addNewEventMessenger;

        private string partNo, model;

        private ItemPriceDtos currentPrice;
        private decimal minimumStock, quantityOnHand, price1, price2, currentCost;
        private bool onSaved;
        private IEnumerable<CategoryDtos> categoryList;
        private IEnumerable<SupplierDtos> supplierList;
        private int categoryId;
        private int supplierId;
        private bool isCategory;
        private bool isConfirmSelection;
        private ProcessCmdEventMessenger confirmSelectionEventMessenger;
        private ProcessCmdEventMessenger listViewEventMessenger;
        private SearchKeyEventMessenger searchKeyEventMessenger;

        public AddEditItemForm(AddNewEventMessenger addNewEventMessenger = null, int itemId = 0)
        {
            this.addNewEventMessenger = addNewEventMessenger;

            this.itemId = itemId;

            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
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
                case Keys.Alt | Keys.I:
                    txtCategory.Focus();

                    return true;
                case Keys.Alt | Keys.S:
                    txtQOH.Focus();

                    return true;
                case Keys.Alt | Keys.P:
                    txtPrice1.Focus();

                    return true;
                case Keys.Alt | Keys.A:
                    btnAddCategory_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

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

                        if (msg.Length == 0 || msg == "SELECT CATEGORY" || msg == "SELECT SUPPLIER") return true;

                        return confirmSelectionEventMessenger();
                    }

                    break;
                case Keys.Enter:
                    if (pnlSelectCategoryOrSupplier.Visible)
                    {
                        return confirmSelectionEventMessenger();
                    }

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
                case Keys.Down:
                    if (pnlSelectCategoryOrSupplier.Visible)
                    {
                        return listViewEventMessenger();
                    }

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task InitializeCategories(int categoryId = 0)
        {
            var categoryList = await categoryController.GetAll();

            this.categoryList = categoryList;
        }

        private async Task InitializeSupplier(int supplierId = 0)
        {
            var supplierList = await supplierController.GetAll();

            this.supplierList = supplierList;
        }

        private async Task InitializeItem()
        {
            var queryItemDtos = await itemController.Find(itemId);

            if (queryItemDtos == null) return;

            this.Text = "Edit Item";

            SelectInvoked(this.supplierId, queryItemDtos.SupplierName, false);

            SelectInvoked(queryItemDtos.CategoryId, queryItemDtos.CategoryName, true);

            txtPartNo.Text = queryItemDtos.PartNo;

            partNo = queryItemDtos.PartNo;

            txtBrand.Text = queryItemDtos.BrandName;

            txtMake.Text = queryItemDtos.Make;

            txtMade.Text = queryItemDtos.Made;

            txtModel.Text = queryItemDtos.Model;

            model = queryItemDtos.Model;

            cboUOM.Text = queryItemDtos.UnitOfMeasure.ToString();

            txtSize.Text = queryItemDtos.Size;

            txtOtherPartNo.Text = queryItemDtos.OtherPartNo;

            txtQOH.Text = queryItemDtos.QuantityOnHand.ToString("#,0.00;");

            quantityOnHand = queryItemDtos.QuantityOnHand;

            txtMinimumStock.Text = queryItemDtos.MinimumStock.ToString("#,0");

            minimumStock = queryItemDtos.MinimumStock;

            txtPrice1.Text = queryItemDtos.Price1.ToString("#,0.00;");

            price1 = queryItemDtos.Price1;

            txtPrice2.Text = queryItemDtos.Price2.ToString("#,0.00;");

            price2 = queryItemDtos.Price2;

            txtCurrentCost.Text = queryItemDtos.CurrentCost.ToString("#,0.00;");

            currentCost = queryItemDtos.CurrentCost;

            if (queryItemDtos.ItemPriceDtosList == null) return;

            this.currentPrice = queryItemDtos.ItemPriceDtosList.OrderByDescending(item => item.LastUpdate).FirstOrDefault();
        }

        private void InitializeUOMDropDownList()
        {
            var query = itemController.GetUnitOfMeasureList();

            cboUOM.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            cboUOM.AutoCompleteSource = AutoCompleteSource.ListItems;

            cboUOM.DataSource = query;

            if (query.Count() > 0) cboUOM.SelectedIndex = 0;
        }

        private bool IsValid()
        {
            var msg = "";

            if (categoryId < 1)
            {
                msg = "Category is required.";

                txtCategory.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtPartNo.Text))
            {
                msg = "Part # is required.";

                txtPartNo.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtBrand.Text))
            {
                msg = "Brand is required.";

                txtBrand.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtMake.Text))
            {
                msg = "Make is required.";

                txtMake.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtMade.Text))
            {
                msg = "Made is required.";

                txtMade.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                msg = "Model is required.";

                txtModel.Focus();
            }
            else if (cboUOM.SelectedValue == null)
            {
                msg = "UOM is required.";

                cboUOM.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtPrice1.Text))
            {
                msg = "Price 1 is required.";

                txtPrice1.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtPrice2.Text))
            {
                msg = "Price 2 is required.";

                txtPrice2.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtCurrentCost.Text))
            {
                msg = "Current Cost is required.";

                txtCurrentCost.Focus();
            }

            //if (msg.Length == 0 && !string.IsNullOrWhiteSpace(txt)
            //{
            //    var validPartNo = true;

            //    if (!string.IsNullOrWhiteSpace(txtPartNo.Text) && partNo != txtPartNo.Text.Trim())
            //        validPartNo = await itemController.IsValidPartNo(
            //            categoryId,
            //            partNo, 
            //            txtPartNo.Text.Trim(),  
            //            txtModel.Text.Trim(),
            //            txtSize.Text.Trim(), 
            //            txtCategory.Text);

            //    var validModel = true;

            //    if (!string.IsNullOrWhiteSpace(txtModel.Text) && model != txtModel.Text.Trim())
            //        validModel = await itemController.IsValidModel(categoryId, model, txtModel.Text.Trim(), txtPartNo.Text.Trim(), txtCategory.Text);

            //    if (!validPartNo) 
            //    {
            //        msg = "Part # already used.";

            //        txtPartNo.Focus();
            //    }
            //    //else if (!validModel)
            //    //{
            //    //    msg = "Model already used.";

            //    //    txtModel.Focus();
            //    //}
            //}

            if (msg.Length > 0) mainForm.ShowMessage(msg);

            return msg.Length == 0;
        }

        private async void AddEditItem_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                await InitializeCategories();

                await InitializeSupplier();

                InitializeUOMDropDownList();

                await InitializeItem();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
                //MessageBox.Show(this, ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!IsValid() || mainForm.IsLoading) return;

            var result = mainForm.ShowMessage("Are you sure you want to save changes?", true);

            if (result == System.Windows.Forms.DialogResult.No) return;

            try
            {
                mainForm.ShowProgressStatus();

                var uom = "";

                decimal quantityOnHand = 0m, price1 = 0m, price2 = 0m, currentCost = 0m, minimumStock = 0m;
                
                if (cboUOM.SelectedValue != null) uom = cboUOM.SelectedValue.ToString();

                decimal.TryParse(txtQOH.Text, out quantityOnHand);

                decimal.TryParse(txtPrice1.Text, out price1);

                decimal.TryParse(txtPrice2.Text, out price2);

                decimal.TryParse(txtCurrentCost.Text, out currentCost);

                decimal.TryParse(txtMinimumStock.Text, out minimumStock);

                var item = new ItemDtos
                {
                    ItemId = itemId,
                    CategoryId = categoryId,
                    SupplierId = supplierId,
                    PartNo = txtPartNo.Text.Trim(),
                    BrandName = txtBrand.Text.Trim(),
                    Make = txtMake.Text.Trim(),
                    Made = txtMade.Text.Trim(),
                    Model = txtModel.Text.Trim(),
                    UnitOfMeasure = uom,
                    Size = txtSize.Text.Trim(),
                    OtherPartNo = txtOtherPartNo.Text.Trim(),
                    QuantityOnHand = quantityOnHand,
                    Price1 = price1,
                    Price2 = price2,
                    CurrentCost = currentCost,
                    MinimumStock = minimumStock
                };

                if (this.currentPrice == null) this.currentPrice = new ItemPriceDtos { ItemId = itemId };

                this.currentPrice.Price1 = price1;

                this.currentPrice.Price2 = price2;

                this.currentPrice.CurrentCost = currentCost;

                var itemPriceDtosList = new List<ItemPriceDtos>();

                itemPriceDtosList.Add(this.currentPrice);

                item.ItemPriceDtosList = itemPriceDtosList;

                var id = await itemController.Save(item);

                if (itemId > 0)
                {
                    var changes = new Tuple<string, decimal>[] 
                    { 
                        new Tuple<string, decimal>("minimum stock", minimumStock),
                        new Tuple<string, decimal>("quantity on hand", quantityOnHand),
                        new Tuple<string, decimal>("price 1", price1),
                        new Tuple<string, decimal>("price 2", price2),
                        new Tuple<string, decimal>("current cost", currentCost),

                    };

                    var oldValues = new[] { this.minimumStock, this.quantityOnHand, this.price1, this.price2, this.currentCost };

                    for (int i = 0; i < changes.Length; i++)
                    {
                        var isChanged = changes[i].Item2 != oldValues[i];

                        if (!isChanged) continue;

                        await userController.SaveItemActivity(
                           new UserActivityDtos
                           {
                                Action = string.Format("Updates {0} from {1} to {2}", 
                                    changes[i].Item1, oldValues[i], changes[i].Item2),
                                CurrentStock = quantityOnHand,
                                Date = DateTime.Now,
                                ItemId = itemId,
                                Quantity = changes[i].Item2,
                                ReferenceNumber = partNo,
                                UserId = mainForm.UserDtos.UserId
                           });
                    }
                }
                else
                {
                    await userController.SaveItemActivity(
                        new UserActivityDtos
                        {
                            Action = string.Format("New Item with Quantity On Hand (QOH) of {0}", quantityOnHand),
                            CurrentStock = quantityOnHand,
                            Date = DateTime.Now,
                            ItemId = itemId,
                            Quantity = quantityOnHand,
                            ReferenceNumber = partNo,
                            UserId = mainForm.UserDtos.UserId
                        });
                }

                mainForm.ShowMessage("Successfully saved");

                if (addNewEventMessenger != null) addNewEventMessenger(id);

                onSaved = true;

                this.Close();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void txtFields_Enter(object sender, EventArgs e)
        {
            var txtControl = (TextBox)sender;

            txtControl.SelectionStart = 0;

            txtControl.SelectionLength = txtControl.Text.Length;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            var addCategoryForm = new AddEditCategoryForm(0, NewCategoryInvoked);

            addCategoryForm.ShowDialog();
        }

        private async void NewCategoryInvoked(int categoryId)
        {
            try
            {
                mainForm.ShowProgressStatus(); 

                await InitializeCategories(categoryId);
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnAddSupplier_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            var addEditSupplierForm = new AddEditSupplierForm(NewSupplierInvoked, 0);

            addEditSupplierForm.ShowDialog();
        }

        private async void NewSupplierInvoked(int id)
        {
            try
            {
                mainForm.ShowProgressStatus();

                await InitializeSupplier(id);
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void AddEditItemForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = mainForm.IsLoading;
            if (!onSaved)
            {
                var result = mainForm.ShowMessage("Are you sure you want to discard?", true);

                if (result == System.Windows.Forms.DialogResult.No) return;
            }

            onSaved = true;
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

                pnlSelectCategoryOrSupplier.Controls.Clear();

                pnlSelectCategoryOrSupplier.Visible = false;

                categoryId = 0;
            }
        }

        private void txtSupplier_Enter(object sender, EventArgs e)
        {
            if (txtSupplier.Text == "SELECT SUPPLIER")
            {
                txtSupplier.Text = string.Empty;
            }
        }

        private void txtSupplier_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSupplier.Text))
            {
                txtSupplier.Text = "SELECT SUPPLIER";

                pnlSelectCategoryOrSupplier.Controls.Clear();

                pnlSelectCategoryOrSupplier.Visible = false;

                supplierId = 0;
            }
        }

        private void txtCategory_TextChanged(object sender, EventArgs e)
        {
            var txtBox = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(txtBox.Text) || isConfirmSelection) return;

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
                    pnlSelectCategoryOrSupplier.Location = new Point(pnlSelectCategoryOrSupplier.Location.X, 143);

                    SearchPanel(this.categoryList, isCategory);
                }
                else
                {
                    pnlSelectCategoryOrSupplier.Location = new Point(pnlSelectCategoryOrSupplier.Location.X, 206);

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

        private void SelectInvoked(int id, string name, bool isCategory)
        {
            pnlSelectCategoryOrSupplier.Controls.Clear();

            pnlSelectCategoryOrSupplier.Visible = false;

            isConfirmSelection = true;

            if (isCategory)
            {
                this.categoryId = id;

                txtCategory.Text = name;

                txtCategory.Focus();
            }
            else
            {
                this.supplierId = id;

                txtSupplier.Text = name;

                txtSupplier.Focus();
            }

            isConfirmSelection = false;
        }

        private void btnViewCategoriesOrSupplier_Click(object sender, EventArgs e)
        {
            pnlSelectCategoryOrSupplier.Visible = !pnlSelectCategoryOrSupplier.Visible;

            var isCategory = ((Button)sender).Name == btnViewCategories.Name;

            if (this.isCategory != isCategory)
            {
                pnlSelectCategoryOrSupplier.Controls.Clear();
            }

            if (isCategory)
            {
                pnlSelectCategoryOrSupplier.Location = new Point(pnlSelectCategoryOrSupplier.Location.X, 143);

                SearchPanel(this.categoryList, isCategory);
            }
            else
            {
                pnlSelectCategoryOrSupplier.Location = new Point(pnlSelectCategoryOrSupplier.Location.X, 206);

                SearchPanel(this.supplierList, isCategory);
            }

            this.isCategory = isCategory;
        }
    }
}
