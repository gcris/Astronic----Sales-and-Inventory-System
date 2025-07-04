using AstronicAutoSupplyInventory.Customer;
using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Shared;
using AstronicAutoSupplyInventory.Transaction.PurchaseOrder;
using CommonLibrary.Dtos;
using InventoryServices.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AstronicAutoSupplyInventory.Transaction.SalesInvoice
{
    public partial class SalesInvoiceForm : Form
    {
        private CustomerController customerController = new CustomerController();
        private SupplierController supplierController = new SupplierController();
        private ItemController itemController = new ItemController();
        private SalesInvoiceController salesInvoiceController = new SalesInvoiceController();
        private PurchaseOrderController purchaseOrderController = new PurchaseOrderController();
        private UserController userController = new UserController();

        //private IEnumerable<ItemDtos> itemDtosList;
        private List<int> deletedIdList;
        private bool started, madeChanges;

        private readonly int salesInvoiceId;
        private readonly string numberFormat = "#,0.00;(#,0.00);''";
        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private decimal totalDiscount;

        public SalesInvoiceForm(int salesInvoiceId = 0)
        {
            this.salesInvoiceId = salesInvoiceId;

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
                case Keys.F4:
                    btnPriceInquiry_Click(this, new EventArgs());

                    return true;
                case Keys.F5:
                    btnReturnItem_Click(this, new EventArgs());

                    return true;
                case Keys.F6:
                    btnReturnInvoice_Click(this, new EventArgs());

                    return true;
                case Keys.F7:
                    btnPurchaseOrderEntry_Click(this, new EventArgs());

                    return true;
                case Keys.F8:
                    btnReturnPurchaseItem_Click(this, new EventArgs());

                    return true;
                case Keys.F9:
                    btnReturnPurchaseOrder_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.C:
                    cboCustomer.Focus();

                    return true;
                case Keys.Alt | Keys.R:
                    txtOrNumber.Focus();

                    return true;
                case Keys.Alt | Keys.T:
                    btnAddCustomer_Click(this, new EventArgs());

                    return true;
                case Keys.Escape:
                    this.Close();

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
                btnReturnItem.Enabled = privileges.Any(privilege => privilege.Action == "Return Sales Invoice");

                btnReturnInvoice.Enabled = btnReturnItem.Enabled;

                btnPurchaseOrderEntry.Enabled = privileges.Any(privilege => privilege.Action == "Encode Purchase Order" && privilege.IsEnable);

                btnReturnPurchaseItem.Enabled = privileges.Any(privilege => privilege.Action == "Return Purchase Order" && privilege.IsEnable);

                btnReturnPurchaseOrder.Enabled = btnReturnPurchaseItem.Enabled;
            }
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            var addEditCustomer = new AddEditCustomerForm(NewCustomerInvoked);

            addEditCustomer.ShowDialog();

            addEditCustomer.BringToFront();
        }

        private async void NewCustomerInvoked(int id)
        {
            try
            {
                if (id > 0)
                {
                    await InitializeCustomer();

                    cboCustomer.SelectedValue = id;
                }

                var addEditCustomer = (AddEditCustomerForm)Application.OpenForms["AddEditCustomerForm"];

                if (addEditCustomer != null) addEditCustomer.Close();
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }

        private async Task InitializeCustomer()
        {
            var customerDtosList = await customerController.GetAll();

            cboCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            cboCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;

            var list = customerDtosList.ToList();

            list.Add(new CustomerDtos { CustomerId = 0, CustomerName = "WALK-IN" });

            cboCustomer.DataSource = list;

            cboCustomer.DisplayMember = "CustomerName";

            cboCustomer.ValueMember = "CustomerId";

            cboCustomer.SelectedValue = 0;
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
                            row.Cells[4].Value = itemDtos.Price1.ToString(numberFormat);

                            row.Cells[3].Value = itemDtos.CurrentCost.ToString(numberFormat);
                        }
                    }
                }
            }
        }

        private async Task<bool> IsValid()
        {
            var msg = "";

            if (string.IsNullOrWhiteSpace(txtOrNumber.Text)) 
            {
                msg = "O.R. Number is required.";

                txtOrNumber.Focus();
            }
            else if (!await salesInvoiceController.IsValidOrNumber(txtOrNumber.Text.Trim()))
            {
                msg = "O.R. Number already used.";

                txtOrNumber.Focus();
            }
            else if (cboCustomer.SelectedValue == null)
            {
                msg = "Customer is required.";

                cboCustomer.Focus();
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
            decimal qty = 0m, unitPrice = 0m, discount = 0m;

            if (qtyCell.Value != null) decimal.TryParse(qtyCell.Value.ToString(), out qty);

            if (discountCell.Value != null) decimal.TryParse(discountCell.Value.ToString(), out discount);

            if (unitPriceCell.Value != null) decimal.TryParse(unitPriceCell.Value.ToString(), out unitPrice);

            return new Tuple<decimal, decimal, decimal>(qty, discount, (qty * unitPrice) - discount);
        }

        private void ClearControls()
        {
            deletedIdList = null;

            txtOrNumber.Text = string.Empty;

            dgvItems.Rows.Clear();

            txtTotalQuantity.Text = string.Empty;

            txtGrandTotal.Text = string.Empty;

            txtTotalDiscount.Text = string.Empty;
        }

        private async void SalesInvoiceForm_Load(object sender, EventArgs e)
        {
            mainForm.ShowProgressStatus();

            try
            {
                InitializePrivileges();

                await InitializeCustomer();

                //await InitializeItemList();

                //await InitializeSalesInvoice();

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
            //var currentColIndex = dgvItems.CurrentCell.ColumnIndex;

            //var itemCell = (TextBox)e.Control;

            //itemCell.CharacterCasing = CharacterCasing.Upper;

            //// Get Item Column Index
            //if (currentColIndex == 0)
            //{
            //    if (itemCell != null && itemDtosList.Count() > 0)
            //    {
            //        itemCell.AutoCompleteMode = AutoCompleteMode.Suggest;

            //        itemCell.AutoCompleteSource = AutoCompleteSource.CustomSource;

            //        var source = new AutoCompleteStringCollection();

            //        var queryItemList = itemDtosList
            //            .Select(item => new
            //            {
            //                Name = string.Format("{0} | {1} | {2}{3}{4}{5}{6}{7}",
            //                    item.PartNo,
            //                    item.CategoryName,
            //                    item.BrandName,
            //                    string.IsNullOrWhiteSpace(item.Model) ? "" :
            //                        string.Format(" | {0}", item.Model),
            //                    string.IsNullOrWhiteSpace(item.Size) ? "" :
            //                        string.Format(" | {0}", item.Size),
            //                    string.IsNullOrWhiteSpace(item.Make) ? "" :
            //                        string.Format(" | {0}", item.Make),
            //                    string.IsNullOrWhiteSpace(item.Made) ? "" :
            //                        string.Format(" | {0}", item.Made),
            //                    string.IsNullOrWhiteSpace(item.OtherPartNo) ? "" :
            //                        string.Format(" | {0}", item.OtherPartNo))
            //            }).OrderBy(item => item.Name).ToList();

            //        foreach (var item in queryItemList) source.Add(item.Name);

            //        itemCell.AutoCompleteCustomSource = source;
            //    }
            //}
            //else itemCell.AutoCompleteCustomSource = null;
        }

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!started) return;

            var currentCell = dgvItems.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (currentCell.Value == null) return;

            if (e.ColumnIndex == 0)
            {
                //var queryItemList = itemDtosList
                //    .Select(item => new
                //    {
                //        Name = string.Format("{0} | {1} | {2}{3}{4}{5}{6}{7}",
                //            item.PartNo,
                //            item.CategoryName,
                //            item.BrandName,
                //            string.IsNullOrWhiteSpace(item.Model) ? "" :
                //                string.Format(" | {0}", item.Model),
                //            string.IsNullOrWhiteSpace(item.Size) ? "" :
                //                string.Format(" | {0}", item.Size),
                //            string.IsNullOrWhiteSpace(item.Make) ? "" :
                //                string.Format(" | {0}", item.Make),
                //            string.IsNullOrWhiteSpace(item.Made) ? "" :
                //                string.Format(" | {0}", item.Made),
                //            string.IsNullOrWhiteSpace(item.OtherPartNo) ? "" :
                //                string.Format(" | {0}", item.OtherPartNo)),
                //        item.ItemId,
                //        item.Price1
                //    }).OrderBy(item => item.Name).ToList();

                ////var itemId = 0;

                //if (currentCell.Value != null)
                //{
                //    var itemDtos = queryItemList.FirstOrDefault(item => item.Name == currentCell.Value.ToString());

                //    if (itemDtos != null)
                //    {
                //        //var itemDtos = queryItemList.FirstOrDefault(item => item.ItemId == itemId);

                //        //dgvItems.Rows[e.RowIndex].Cells[0].Value = e.RowIndex + 1;

                //        dgvItems.Rows[e.RowIndex].Cells[0].Tag = itemDtos.ItemId;

                //        dgvItems.Rows[e.RowIndex].Cells[3].Value = itemDtos.Price1.ToString(numberFormat);
                //    }
                //}

                //if (!madeChanges) madeChanges = true;
            }

            if (e.ColumnIndex >= 1 && e.ColumnIndex <= 4)
            {
                var qtyCell = dgvItems.Rows[e.RowIndex].Cells[1];

                var discountCell = dgvItems.Rows[e.RowIndex].Cells[2];

                var unitPriceCell = dgvItems.Rows[e.RowIndex].Cells[3];

                dgvItems.Rows[e.RowIndex].Cells[5].Value = CalculateRowTotal(qtyCell, discountCell, unitPriceCell).Item3.ToString(numberFormat);
            }

            decimal totalQty = 0m, totalAmount = 0m;

            totalDiscount = 0m;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                var qtyCell = row.Cells[1];

                var discountCell = row.Cells[2];

                var unitPriceCell = row.Cells[3];

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
                var detailDtosList = new List<SalesInvoiceDetailDtos>();

                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    var detailId = 0;

                    if (row.Tag != null) int.TryParse(row.Tag.ToString(), out detailId);

                    var itemCell = row.Cells[0];

                    var qtyCell = row.Cells[1];

                    var discountCell = row.Cells[2];

                    var netPriceCell = row.Cells[4]; //Current Cost

                    var unitPriceCell = row.Cells[3]; //Selling Price
                   
                    var totalCell = row.Cells[5];

                    if (itemCell.Value == null || totalCell.Value == null) continue;

                    decimal qty = 0m, unitPrice = 0m, total = 0m, discount = 0m;

                    if (qtyCell.Value != null) decimal.TryParse(qtyCell.Value.ToString(), out qty);

                    if (discountCell.Value != null) decimal.TryParse(discountCell.Value.ToString(), out discount);

                    var itemId = 0;

                    if (itemCell.Tag != null) int.TryParse(itemCell.Tag.ToString(), out itemId);

                    if (itemId < 1) continue;

                    if (unitPriceCell.Value != null) decimal.TryParse(unitPriceCell.Value.ToString(), out unitPrice);

                    if (totalCell.Value != null) decimal.TryParse(totalCell.Value.ToString(), out total);

                    detailDtosList.Add(new SalesInvoiceDetailDtos
                    {
                        ItemDtos = new ItemDtos { ItemId = itemId },
                        Quantity = qty,
                        TotalAmount = total,
                        UnitPrice = unitPrice,
                        SalesInvoiceDetailId = detailId,
                        SalesInvoiceId = salesInvoiceId,
                        Discount = discount
                    });
                }

                var type = cboCustomer.Text;

                var salesInvoiceDtos = new SalesInvoiceDtos
                {
                    Date = dtpDate.Value,
                    CustomerId = (int)cboCustomer.SelectedValue,
                    ORNumber = txtOrNumber.Text,
                    Remarks = txtRemarks.Text,
                    SalesInvoiceId = salesInvoiceId,
                    SalesInvoiceDetailDtosList = detailDtosList,
                    UserId = mainForm.UserDtos.UserId,
                    TotalDiscount = totalDiscount
                };

                success = await salesInvoiceController.Save(salesInvoiceDtos, deletedIdList);

                msg = success ? "Successfully saved." : "Failed to save. Please contact the administartor.";

                if (success)
                {
                    foreach (var item in detailDtosList)
                    {
                        await userController.SaveItemActivity(
                            new UserActivityDtos
                            {
                                Action = "Sales Invoice (OUT)",
                                CurrentStock = item.CurrentStock,
                                Date = DateTime.Now,
                                ItemId = item.ItemDtos.ItemId,
                                Quantity = item.Quantity,
                                Amount = item.TotalAmount,
                                ReferenceNumber = txtOrNumber.Text,
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

        private void SalesInvoiceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (madeChanges)
            {
                var result = mainForm.ShowMessage("Are you sure yo want to discard?", true);

                e.Cancel = result == System.Windows.Forms.DialogResult.No;
            }
        }

        private void btnReturnItem_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnReturnItem.Enabled) return;

            var salesReturnForm = new SalesReturnItemsForm();

            salesReturnForm.ShowDialog();
        }

        private void btnPurchaseOrderEntry_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnPurchaseOrderEntry.Enabled) return;

            var purchaseOrderForm = new PurchaseOrderEntryForm();

            purchaseOrderForm.ShowDialog();
        }

        private void btnPriceInquiry_Click(object sender, EventArgs e)
        {
            var priceInquiryForm = new PriceInquiryForm();

            priceInquiryForm.ShowDialog();
        }

        private void btnReturnInvoice_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnReturnInvoice.Enabled) return;

            var enterOrNumber = new ReturnTransactionForm();

            enterOrNumber.ShowDialog();
        }


        private void btnReturnPurchaseItem_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnReturnPurchaseItem.Enabled) return;

            var poReturnItemsForm = new PoReturnItemsForm();

            poReturnItemsForm.ShowDialog();
        }

        private void btnReturnPurchaseOrder_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnReturnPurchaseOrder.Enabled) return;

            var enterOrNumber = new ReturnTransactionForm(false);

            enterOrNumber.ShowDialog();
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
                searchItemForm = new SearchItemForm(new ConfirmSearchItemEventMessenger(ConfirmItems), isSalesInvoice: true);
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
                row.Cells[4].Value = item.CurrentCost.ToString(numberFormat);
                row.Cells[3].Value = item.Price1.ToString(numberFormat);
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

        private void cboCustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar)) e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        private void txtRemarks_Enter(object sender, EventArgs e)
        {
            txtRemarks.SelectAll();
        }
    }
}
