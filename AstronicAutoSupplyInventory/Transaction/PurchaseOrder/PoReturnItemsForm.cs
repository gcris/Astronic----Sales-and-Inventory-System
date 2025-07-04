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

namespace AstronicAutoSupplyInventory.Transaction.PurchaseOrder
{
    public partial class PoReturnItemsForm : Form
    {
        private PurchaseOrderController purchaseOrderController = new PurchaseOrderController();
        private PurchaseOrderReturnController poReturnController = new PurchaseOrderReturnController();
        private UserController userController = new UserController();

        private readonly string numberFormat = "#,0.00;(#,0.00);''";

        //private IEnumerable<PurchaseOrderDtos> poDtosList;
        private PurchaseOrderDtos purchaseOrderDtos;
        private bool started, madeChanges;
        private string poNumber;

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public PoReturnItemsForm(string orNumber = "")
        {
            this.poNumber = orNumber;

            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Alt | Keys.S:
                    txtSearch.Focus();

                    return true;
                case Keys.Alt | Keys.D:
                    dtpDate.Focus();

                    return true;
                case Keys.F1:
                    btnConfirm_Click(this, new EventArgs());

                    return true;
                case Keys.Escape:
                    btnCancel_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
                case Keys.Down:
                    if (dgvItems.Focused) break;

                    dgvItems.Focus();

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        //private async Task InitializeSearchTextbox()
        //{
        //    this.poDtosList = await purchaseOrderController.GetAll(this.poDtosList, DateTime.MinValue, DateTime.MinValue);

        //    var autoSourceList = new AutoCompleteStringCollection();

        //    foreach (var salesInvoiceDtos in this.poDtosList)
        //        autoSourceList.Add(cboSearchType.SelectedIndex == 0 ? salesInvoiceDtos.PONumber : salesInvoiceDtos.SupplierName);

        //    txtSearch.AutoCompleteMode = AutoCompleteMode.Suggest;

        //    txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;

        //    txtSearch.AutoCompleteCustomSource = autoSourceList;

        //    if (!txtSearch.Focused) 
        //        txtSearch.Text = cboSearchType.SelectedIndex == 0 ? 
        //            "ENTER P.O. NUMBER!" :
        //            "ENTER SUPPLIER'S NAME";

        //    txtSearch.Focus();
        //}

        private async Task InitializePurchaseOrder(string key = "")
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            var purchaseOrder = await purchaseOrderController.Find(key);

            if (purchaseOrder == null)
            {
                MessageBox.Show(this, string.Format("No P.O. # '{0}' found", key), 
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            this.purchaseOrderDtos = purchaseOrder;

            dgvItems.Rows.Clear();

            var salesInvoiceListForm = new PoDetailListForm(
                new EventMesseging.ConfirmPurchaseItemsToReturnEventMessenger(ConfirmItemsToReturnInvoked), 
                purchaseOrderDtos.PurchaseOrderDetailDtosList);

            salesInvoiceListForm.ShowDialog();
        }

        private void ConfirmItemsToReturnInvoked(IEnumerable<PurchaseOrderDetailDtos> poDetailDtosList)
        {
            var salesInvoiceListForm = (PoDetailListForm)Application.OpenForms["PoDetailListForm"];

            salesInvoiceListForm.Close();

            if (poDetailDtosList == null) return;

            foreach (var item in poDetailDtosList)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = item.PurchaseOrderDetailId;

                row.Cells[0].Tag = item.ItemDtos.ItemId;

                row.Cells[0].Value = item.ItemDtos.CategoryName;

                row.Cells[1].Value = item.ItemDtos.PartNo;

                row.Cells[2].Value = item.ItemDtos.BrandName;

                row.Cells[3].Value = item.ItemDtos.Model;

                row.Cells[4].Value = item.ItemDtos.Make;

                row.Cells[5].Value = item.ItemDtos.Made;

                row.Cells[6].Value = item.ItemDtos.Size;

                row.Cells[7].Value = item.Quantity.ToString(numberFormat);

                dgvItems.CurrentCell = dgvItems.Rows[0].Cells[8];
                dgvItems.Focus();

                for (int i = 0; i < 8; i++) row.Cells[0].ReadOnly = true;
            }

            dtpDate.Focus();

            ActiveControl = dgvItems;
         
            madeChanges = true;

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                row.DefaultCellStyle.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
        }

        private async void SearchPONumber()
        {
            if (madeChanges || (poNumber != txtSearch.Text.Trim() &&
                !string.IsNullOrWhiteSpace(txtSearch.Text) &&
                !string.IsNullOrWhiteSpace(poNumber)))
            {
                var result = mainForm.ShowMessage("Are you sure you want to discard?", true);

                if (result == System.Windows.Forms.DialogResult.No) return;
            }

            try
            {
                poNumber = txtSearch.Text.Trim();

                await InitializePurchaseOrder(txtSearch.Text.Trim());
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }
        }

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!started || mainForm.IsLoading) return;

            if (e.ColumnIndex == 8)
            {
                var row = dgvItems.Rows[e.RowIndex];

                var qtyCell = row.Cells[7];

                var qtyToReturnCell = row.Cells[8];

                decimal qty = 0m, qtyToReturn = 0m;

                if (qtyCell.Value != null) decimal.TryParse(qtyCell.Value.ToString(), out qty);

                if (qtyToReturnCell.Value != null) decimal.TryParse(qtyToReturnCell.Value.ToString(), out qtyToReturn);

                if (qtyToReturn > qty)
                {
                    mainForm.ShowMessage("Insufficient quantity.");

                    row.Cells[8].Value = null;

                    dgvItems.CurrentCell = row.Cells[8];

                    return;
                }
            }

            decimal totalQty = 0m;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                var qtyCell = row.Cells[8];

                if (qtyCell.Value != null)
                {
                    var value = 0m;

                    decimal.TryParse(qtyCell.Value.ToString(), out value);

                    totalQty += value;

                    if (!madeChanges && value > 0) madeChanges = true;
                }
            }

            txtTotalQuantityToReturn.Text = totalQty.ToString(numberFormat);
        }

        private async void SalesReturnItemsForm_Load(object sender, EventArgs e)
        {
            try
            {
                //cboSearchType.SelectedIndex = 0;

                //await InitializeSearchTextbox();

                if (!string.IsNullOrWhiteSpace(poNumber))
                {
                   
                    await InitializePurchaseOrder(poNumber);

                    txtSearch.Text = poNumber;
                    if (dgvItems.Rows.Count != 0)
                    {
                        dgvItems.CurrentCell = dgvItems.Rows[0].Cells[8];
                        dgvItems.Focus();//asd
                    }
                    else this.Close();
                    //orNumber = string.Empty;
                }
                else txtSearch.Focus();

                started = true;
            }
            catch (Exception ex) { mainForm.HandleException(ex); }
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!madeChanges || mainForm.IsLoading) return;

            btnConfirm.Focus();

            var result = mainForm.ShowMessage("Are you sure yo want to save?", true);

            if (result == DialogResult.No) return;

            var msg = "";

            var success = false;

            try
            {
                mainForm.ShowProgressStatus();

                var detailDtosList = new List<PurchaseOrderReturnDetailDtos>();

                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    int detailId = 0, itemId = 0;

                    if (row.Tag != null) int.TryParse(row.Tag.ToString(), out detailId);

                    if (detailId < 1) continue;

                    if (row.Cells[0].Tag != null) int.TryParse(row.Cells[0].Tag.ToString(), out itemId);

                    if (itemId < 1) continue;

                    var qtyCell = row.Cells[8];

                    var remarksCell = row.Cells[9];

                    if (qtyCell.Value == null) qtyCell.Value = row.Cells[7].Value;

                    var qty = 0m;

                    var remarks = "";

                    if (qtyCell.Value != null) decimal.TryParse(qtyCell.Value.ToString(), out qty);

                    if (remarksCell.Value != null) remarks = remarksCell.Value.ToString();

                    detailDtosList.Add(new PurchaseOrderReturnDetailDtos
                    {
                        Quantity = qty,
                        Remarks = remarks,
                        PurchaseOrderDetailId = detailId,
                        ItemDtos = new ItemDtos { ItemId = itemId }
                    });
                }

                var poReturnDtos = new PurchaseOrderReturnDtos
                {
                    Date = dtpDate.Value,
                    ReferenceNumber = this.purchaseOrderDtos.PONumber,
                    PurchaseOrderReturnDetailDtosList = detailDtosList,
                    TotalQuantity = detailDtosList.Sum(detail => detail.Quantity),
                    TotalAmount = detailDtosList.Sum(detail => detail.Amount),
                    UserId = mainForm.UserDtos.UserId
                };

                success = await poReturnController.Save(poReturnDtos, DateTime.MinValue);

                msg = success ? "Successfully saved." : "Failed to save. Please contact the administartor.";

                if (success)
                {
                    foreach (var item in detailDtosList)
                    {
                        await userController.SaveItemActivity(
                            new UserActivityDtos
                            {
                                Action = "Purchase Order Return (OUT-RETURNED)",
                                CurrentStock = item.CurrentStock,
                                Date = DateTime.Now,
                                ItemId = item.ItemDtos.ItemId,
                                Quantity = item.Quantity,
                                Amount = item.Amount,
                                ReferenceNumber = poNumber,
                                Remarks = item.Remarks,
                                UserId = mainForm.UserDtos.UserId
                            });
                    }
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

                mainForm.ShowProgressStatus(false);

                if (success)
                {
                    this.madeChanges = false;

                    this.Close();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            this.Close();
        }

        private void SalesReturnItemsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (madeChanges)
            {
                var result = mainForm.ShowMessage("Are you sure yo want to discard?", true);

                e.Cancel = result == System.Windows.Forms.DialogResult.No;
            }
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SearchPONumber();
            }
        }
    }
}
