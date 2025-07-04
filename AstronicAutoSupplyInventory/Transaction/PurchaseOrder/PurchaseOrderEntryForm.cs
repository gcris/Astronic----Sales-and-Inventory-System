using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Shared;
using AstronicAutoSupplyInventory.Supplier;
using CommonLibrary.Dtos;
using CommonLibrary.Enums;
using InventoryServices.Controllers;
using InventoryServices.EventMessenger;
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

namespace AstronicAutoSupplyInventory.Transaction.PurchaseOrder
{
    public partial class PurchaseOrderEntryForm : Form
    {
        private ItemController itemController = new ItemController();
        private SupplierController supplierController = new SupplierController();
        private PurchaseOrderController poController = new PurchaseOrderController();
        private UserController userController = new UserController();

        //private IEnumerable<ItemDtos> itemDtosList;
        private List<int> deletedIdList;
        private bool started, madeChanges;

        private readonly string numberFormat = "#,0.00;(#,0.00);''";
        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private decimal totalDiscount;

        public PurchaseOrderEntryForm()
        {
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
                    btnConfirm_Click(this, new EventArgs());

                    return true;
                case Keys.F2:
                    btnSelectItem_Click(this, new EventArgs());

                    return true;
                case Keys.F3:
                    btnRemove_Click(this, new EventArgs());

                    return true;
                case Keys.Escape:
                    btnCancel_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.S:
                    cboSupplier.Focus();

                    return true;
                case Keys.Alt | Keys.R:
                    txtPONumber.Focus();

                    return true;
                case Keys.Alt | Keys.U:
                    btnAddSupplier_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task InitializeSupplier()
        {
            var supplierDtosList = await supplierController.GetAll();

            cboSupplier.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            cboSupplier.AutoCompleteSource = AutoCompleteSource.ListItems;

            cboSupplier.DataSource = supplierDtosList.ToList();

            cboSupplier.DisplayMember = "Company";

            cboSupplier.ValueMember = "SupplierId";

            cboSupplier.SelectedValue = 0;
        }

        private void btnAddSupplier_Click(object sender, EventArgs e)
        {
            var addEditCustomer = new AddEditSupplierForm(NewCustomerInvoked);

            addEditCustomer.ShowDialog();

            addEditCustomer.BringToFront();
        }

        private async void NewCustomerInvoked(int id)
        {
            try
            {
                mainForm.ShowProgressStatus();

                if (id > 0)
                {
                    await InitializeSupplier();

                    cboSupplier.SelectedValue = id;
                }

                var addEditSupplierForm = (AddEditSupplierForm)Application.OpenForms["AddEditSupplierForm"];

                if (addEditSupplierForm != null) addEditSupplierForm.Close();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        public async void NotifierInvoked(bool success)
        {
            if (success)
            {
                mainForm.ShowProgressStatus(false);

                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    if (row.Cells[0].Tag == null) continue;

                    var itemId = 0;

                    int.TryParse(row.Cells[0].Tag.ToString(), out itemId);

                    if (itemId > 0)
                    {
                        var itemDtos = await itemController.Find(itemId);

                        if (itemDtos != null)
                        {
                            row.Cells[4].Value = itemDtos.CurrentCost.ToString(numberFormat);

                            row.Cells[5].Value = itemDtos.Price1.ToString(numberFormat);
                        }
                    }
                }
            }
        }

        private async Task<bool> IsValid()
        {
            var msg = "";

            if (!await poController.IsValidPoNumber(txtPONumber.Text.Trim()))
            {
                msg = "P.O. Number already used.";

                txtPONumber.Focus();
            }
            else if (cboSupplier.SelectedValue == null)
            {
                msg = "Supplier is required.";

                cboSupplier.Focus();
            }
            else if (dgvItems.Rows.Count < 1)
            {
                msg = "Select item(s) before you proceed.";

                dgvItems.Focus();
            }

            if (msg.Length > 0) MessageBox.Show(this, msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

            return msg.Length == 0;
        }

        private Tuple<decimal, decimal, decimal> CalculateRowTotal(DataGridViewCell qtyCell, DataGridViewCell discountCell, DataGridViewCell unitPriceCell)
        {
            decimal qty = 0m, discount = 0m, unitPrice = 0m;

            if (qtyCell.Value != null) decimal.TryParse(qtyCell.Value.ToString(), out qty);

            if (discountCell.Value != null) decimal.TryParse(discountCell.Value.ToString(), out discount);

            if (unitPriceCell.Value != null) decimal.TryParse(unitPriceCell.Value.ToString(), out unitPrice);

            return new Tuple<decimal, decimal, decimal>(qty, discount, (qty * unitPrice) - discount);
        }

        private void ClearControls()
        {
            deletedIdList = null;
            txtPONumber.Text = string.Empty;
            dgvItems.Rows.Clear();
            txtTotalQuantity.Text = string.Empty;
            txtGrandTotal.Text = string.Empty;
            txtTotalDiscount.Text = string.Empty;
            txtRemarks.Text = string.Empty;
        }

        private async void PurchaseOrderEntryForm_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();
                dgvItems.AutoGenerateColumns = false;
                await InitializeSupplier();

                //await InitializeItemList();

                started = true;
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void dgvItems_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var currentColIndex = dgvItems.CurrentCell.ColumnIndex;

            var itemCell = (TextBox)e.Control;

            itemCell.CharacterCasing = CharacterCasing.Upper;

            // Get Item Column Index
            if (currentColIndex == 0)
            {
                //if (itemCell != null && itemDtosList.Count() > 0)
                //{
                //    itemCell.AutoCompleteMode = AutoCompleteMode.Suggest;

                //    itemCell.AutoCompleteSource = AutoCompleteSource.CustomSource;

                //    var source = new AutoCompleteStringCollection();

                //    var queryItemList = itemDtosList
                //        .Select(item => new
                //        {
                //            Name = string.Format("{0} | {1} | {2}{3}{4}{5}{6}{7}",
                //                item.PartNo,
                //                item.CategoryName,
                //                item.BrandName,
                //                string.IsNullOrWhiteSpace(item.Model) ? "" :
                //                    string.Format(" | {0}", item.Model),
                //                string.IsNullOrWhiteSpace(item.Size) ? "" :
                //                    string.Format(" | {0}", item.Size),
                //                string.IsNullOrWhiteSpace(item.Make) ? "" :
                //                    string.Format(" | {0}", item.Make),
                //                string.IsNullOrWhiteSpace(item.Made) ? "" :
                //                    string.Format(" | {0}", item.Made),
                //                string.IsNullOrWhiteSpace(item.OtherPartNo) ? "" :
                //                    string.Format(" | {0}", item.OtherPartNo))
                //        }).OrderBy(item => item.Name).ToList();

                //    foreach (var item in queryItemList) source.Add(item.Name);

                //    itemCell.AutoCompleteCustomSource = source;
                //}
            }
            else if (currentColIndex == 3)
            {
                itemCell.AutoCompleteMode = AutoCompleteMode.Suggest;

                itemCell.AutoCompleteSource = AutoCompleteSource.CustomSource;

                var source = new AutoCompleteStringCollection();

                var uomList = itemController.GetUnitOfMeasureList();

                foreach (var item in uomList) source.Add(item);

                itemCell.AutoCompleteCustomSource = source;
            }
            else itemCell.AutoCompleteCustomSource = null;
        }

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!started || mainForm.IsLoading) return;

            var currentCell = dgvItems.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (currentCell.Value == null) return;

            if (e.ColumnIndex >= 1 && e.ColumnIndex <= 4)
            {
                var qtyCell = dgvItems.Rows[e.RowIndex].Cells[1];

                var discountCell = dgvItems.Rows[e.RowIndex].Cells[2];

                var unitPriceCell = dgvItems.Rows[e.RowIndex].Cells[4];

                dgvItems.Rows[e.RowIndex].Cells[6].Value = CalculateRowTotal(qtyCell, discountCell, unitPriceCell).Item3.ToString(numberFormat);
            }

            decimal totalQty = 0m, totalAmount = 0m;

            totalDiscount = 0m;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                var qtyCell = row.Cells[1];

                var discountCell = row.Cells[2];

                var unitPriceCell = row.Cells[4];

                var result = CalculateRowTotal(qtyCell, discountCell, unitPriceCell);

                totalQty += result.Item1;

                totalDiscount += result.Item2;

                totalAmount += result.Item3;
            }

            txtTotalQuantity.Text = totalQty.ToString(numberFormat);

            txtGrandTotal.Text = totalAmount.ToString(numberFormat);

            txtTotalDiscount.Text = totalDiscount.ToString(numberFormat);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count < 1) return;

            var result = mainForm.ShowMessage("Are you sure yo want to remove?", true);

            if (result == DialogResult.No) return;

            if (deletedIdList == null) deletedIdList = new List<int>();

            foreach (DataGridViewRow row in dgvItems.SelectedRows)
            {
                if (row.Tag != null) deletedIdList.Add((int)row.Tag);

                dgvItems.Rows.Remove(row);
            }
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!await IsValid()) return;

            btnConfirm.Focus();

            var result = mainForm.ShowMessage("Are you sure yo want to save?", true);

            if (result == DialogResult.No) return;

            var msg = "";

            var success = false;

            try
            {
                var detailDtosList = new List<PurchaseOrderDetailDtos>();

                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    var detailId = 0;

                    if (row.Tag != null) int.TryParse(row.Tag.ToString(), out detailId);

                    var itemCell = row.Cells[0];

                    var qtyCell = row.Cells[1];

                    var discountCell = row.Cells[2];

                    var uomCell = row.Cells[3];

                    var unitPriceCell = row.Cells[4];

                    var sellingPriceCell = row.Cells[5];

                    var totalCell = row.Cells[6];

                    if (itemCell.Value == null || totalCell.Value == null) continue;

                    decimal qty = 0m, discount = 0m, unitPrice = 0m, sellingPrice = 0m, total = 0m;

                    UnitOfMeasure uom = 0;

                    if (qtyCell.Value != null) decimal.TryParse(qtyCell.Value.ToString(), out qty);

                    if (discountCell.Value != null) decimal.TryParse(discountCell.Value.ToString(), out discount);

                    if (uomCell.Value != null) Enum.TryParse(uomCell.Value.ToString(), out uom);

                    var itemId = 0;

                    if (itemCell.Tag != null) int.TryParse(itemCell.Tag.ToString(), out itemId);

                    if (itemId < 1) continue;

                    if (unitPriceCell.Value != null) decimal.TryParse(unitPriceCell.Value.ToString(), out unitPrice);

                    if (sellingPriceCell.Value != null) decimal.TryParse(sellingPriceCell.Value.ToString(), out sellingPrice);

                    if (totalCell.Value != null) decimal.TryParse(totalCell.Value.ToString(), out total);

                    detailDtosList.Add(new PurchaseOrderDetailDtos
                    {
                        ItemDtos = new ItemDtos { ItemId = itemId },
                        Quantity = qty,
                        TotalAmount = total,
                        UnitPrice = unitPrice,
                        SellingPrice = sellingPrice,
                        Discount = discount
                    });
                }

                var purchaseOrderDtos = new PurchaseOrderDtos
                {
                    Date = dtpDate.Value,
                    SupplierId = (int)cboSupplier.SelectedValue,
                    PONumber = txtPONumber.Text,
                    Remarks = txtRemarks.Text,
                    PurchaseOrderDetailDtosList = detailDtosList,
                    UserId = mainForm.UserDtos.UserId,
                    TotalDiscount = totalDiscount
                };

                success = await poController.Save(purchaseOrderDtos, deletedIdList, mainForm.NotifierEventMessenger);

                msg = success ? "Successfully saved." : "Failed to save. Please contact the administartor.";

                if (success)
                {
                    foreach (var item in detailDtosList)
                    {
                        await userController.SaveItemActivity(
                            new UserActivityDtos
                            {
                                Action = "Purchase Order (IN)",
                                CurrentStock = item.CurrentStock,
                                Date = DateTime.Now,
                                ItemId = item.ItemDtos.ItemId,
                                Quantity = item.Quantity,
                                Amount = item.TotalAmount,
                                ReferenceNumber = txtPONumber.Text,
                                Remarks = txtRemarks.Text,
                                UserId = mainForm.UserDtos.UserId
                            });
                    }

                    this.madeChanges = false;
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
                mainForm.ShowMessage(msg, false, !success);

                if (success) ClearControls();
            }
        }

        private void PurchaseOrderEntryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (madeChanges)
            {
                var result = mainForm.ShowMessage("Are you sure yo want to discard?", true);

                e.Cancel = result == System.Windows.Forms.DialogResult.No;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }

        private void btnSelectItem_Click(object sender, EventArgs e)
        {
            var searchItemForm = (SearchItemForm)Application.OpenForms["SearchItemForm"];

            if (searchItemForm == null)
            {
                searchItemForm = new SearchItemForm(new ConfirmSearchItemEventMessenger(ConfirmItems), true);
            }

            searchItemForm.Show();

            searchItemForm.BringToFront();
        }

        private async void ConfirmItems(List<int> idList)
        {
            var selectItemForm = (SearchItemForm)Application.OpenForms["SearchItemForm"];

            if (selectItemForm != null) selectItemForm.Close();

            foreach (var id in idList)
            {
                if (id < 1) continue;

                var item = await itemController.Find(id);

                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Cells[0].Value = string.Format("{0} | {1} | {2}{3}{4}{5}{6}{7}",
                    item.PartNo,
                    item.CategoryName,
                    item.BrandName,
                    string.IsNullOrWhiteSpace(item.Model) ? "" :
                        string.Format(" | {0}", item.Model),
                    string.IsNullOrWhiteSpace(item.Size) ? "" :
                        string.Format(" | {0}", item.Size),
                    string.IsNullOrWhiteSpace(item.Make) ? "" :
                        string.Format(" | {0}", item.Make),
                    string.IsNullOrWhiteSpace(item.Made) ? "" :
                        string.Format(" | {0}", item.Made),
                    string.IsNullOrWhiteSpace(item.OtherPartNo) ? "" :
                        string.Format(" | {0}", item.OtherPartNo));

                row.Cells[0].Tag = item.ItemId;

                row.Cells[0].ReadOnly = true;

                row.Cells[3].Value = item.UnitOfMeasure.ToString();

                row.Cells[4].Value = item.CurrentCost.ToString(numberFormat);

                row.Cells[5].Value = item.Price1.ToString(numberFormat);
                dgvItems.CurrentCell = row.Cells[1];
                dgvItems.Focus();

            }

            if (!madeChanges) madeChanges = true;

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (DataGridViewRow currow in dgvItems.Rows)
            {
                currow.DefaultCellStyle.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
        }

        private void cboSupplier_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar)) e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        private void txtRemarks_Enter(object sender, EventArgs e)
        {
            txtRemarks.SelectAll();
        }
    }
}
