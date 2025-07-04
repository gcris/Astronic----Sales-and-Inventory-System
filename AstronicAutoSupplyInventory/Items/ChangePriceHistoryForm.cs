using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Shared;
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
    public partial class ChangePriceHistoryForm : Form
    {
        private int categoryId;

        private ItemPriceController itemPriceController = new ItemPriceController();
        private CategoryController categoryController = new CategoryController();

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private IEnumerable<CommonLibrary.Dtos.CategoryDtos> categoryList;
        private bool isConfirmSelection;
        private SearchKeyEventMessenger searchKeyEventMessenger;
        private ProcessCmdEventMessenger listViewEventMessenger;
        private ProcessCmdEventMessenger confirmSelectionEventMessenger;
        private bool started;

        public ChangePriceHistoryForm(int categoryId)
        {
            this.categoryId = categoryId;

            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Alt | Keys.C:
                    txtCategory.Focus();

                    return true;
                case Keys.F1:
                    btnAdd_Click(this, new EventArgs());

                    return true;
                case Keys.F2:
                    btnEdit_Click(this, new EventArgs());

                    return true;
                case Keys.Escape:
                    if (pnlSelectCategory.Visible)
                    {
                        pnlSelectCategory.Visible = false;

                        return true;
                    }

                    btnCancel_Click(this, new EventArgs());

                    return true;
                case Keys.Down:
                    if (pnlSelectCategory.Visible)
                    {
                        return listViewEventMessenger();
                    }

                    dgvItems.Focus();

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

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
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task InitializePriceHistory()
        {
            dgvItems.Rows.Clear();

            var itemPriceDtosList = await itemPriceController.GetPriceListByDate(this.categoryId, DateTime.MinValue);

            itemPriceDtosList = itemPriceDtosList.OrderByDescending(item => item.LastUpdate);

            foreach (var itemGroup in itemPriceDtosList.GroupBy(item => item.LastUpdate.Date))
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Cells[0].Value = itemGroup.FirstOrDefault().CategoryName;

                row.Cells[1].Value = itemGroup.FirstOrDefault().LastUpdate.ToShortDateString();
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

        private async Task InitializeCategories()
        {
            var categoryList = await categoryController.GetAll();

            this.categoryList = categoryList;
        }

        private void ChangePrice(DateTime date)
        {
            if (categoryId == 0) return;

            var changePriceForm = new ChangePriceForm(date, categoryId);

            mainForm.ChangeForm(changePriceForm, true);
        }

        private async void ChangePriceHistory_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                await InitializeCategories();

                await InitializePriceHistory();

                started = true;
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            ChangePrice(DateTime.MinValue);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count < 1 || mainForm.IsLoading) return;

            var row = dgvItems.SelectedRows[0];

            var dateAsString = row.Cells[1].Value.ToString();

            if (!string.IsNullOrWhiteSpace(dateAsString))
            {
                var date = DateTime.MinValue;

                DateTime.TryParse(dateAsString, out date);

                ChangePrice(date);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            this.Close();
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                await InitializeCategories();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }
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
            catch (Exception ex) { mainForm.HandleException(ex); }
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

            txtCategory.Focus();

            await InitializeCategories();

            isConfirmSelection = false;
        }

        private void btnViewCategories_Click(object sender, EventArgs e)
        {
            pnlSelectCategory.Visible = false;

            SearchPanel(this.categoryList);
        }
    }
}
