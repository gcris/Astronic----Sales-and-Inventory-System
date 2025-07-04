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

namespace AstronicAutoSupplyInventory.Transaction.PurchaseOrder
{
    public partial class PoReturnViewDetailForm : Form
    {
        private PurchaseOrderReturnController poReturnController = new PurchaseOrderReturnController();
        private UserController userController = new UserController();

        private readonly string numberFormat = "#,0.00;(#,0.00);''";
        private readonly int id;

        private PurchaseOrderReturnDtos poReturnDtos;
        private bool started, madeChanges;
        private List<int> idList;

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public PoReturnViewDetailForm(int id = 0)
        {
            this.id = id;

            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.F1:
                    btnConfirm_Click(this, new EventArgs());

                    return true;
                case Keys.F2:
                    btnDelete_Click(this, new EventArgs());

                    return true;
                case Keys.Control | Keys.S:
                    txtReferenceNumber.Focus();

                    return true;
                case Keys.Escape:
                    btnCancel_Click(this, new EventArgs());

                    return true;
                case Keys.Down:
                    if (dgvItems.Focused) break;

                    dgvItems.Focus();

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task InitializePurchaseOrderReturn()
        {
            if (id < 1) return;

            dgvItems.Rows.Clear();

            var poReturnDtos = await poReturnController.Find(id);

            this.poReturnDtos = poReturnDtos;

            txtReferenceNumber.Text = poReturnDtos.ReferenceNumber;

            foreach (var detailDtos in poReturnDtos.PurchaseOrderReturnDetailDtosList)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = detailDtos.PurchaseOrderReturnDetailId;

                row.Cells[0].Value = detailDtos.ItemDtos.CategoryName;

                row.Cells[1].Value = detailDtos.ItemDtos.PartNo;

                row.Cells[2].Value = detailDtos.ItemDtos.BrandName;

                row.Cells[3].Value = detailDtos.ItemDtos.Model;

                row.Cells[4].Value = detailDtos.ItemDtos.Make;

                row.Cells[5].Value = detailDtos.ItemDtos.Made;

                row.Cells[6].Value = detailDtos.ItemDtos.Size;

                row.Cells[7].Value = detailDtos.Quantity.ToString(numberFormat);

                row.Cells[8].Value = detailDtos.Amount.ToString(numberFormat);

                row.Cells[9].Value = detailDtos.Remarks;
            }

            txtTotalQuantity.Text = poReturnDtos.TotalQuantity.ToString("#,0.00");

            txtTotalAmount.Text = poReturnDtos.TotalAmount.ToString("#,0.00");

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                row.DefaultCellStyle.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
        }

        private async void SalesReturnViewDetailForm_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                await InitializePurchaseOrderReturn();

                started = true;
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count < 1 || mainForm.IsLoading) return;

            var result = mainForm.ShowMessage("Are you sure yo want to delete the selected item(s)?", true);

            if (result == DialogResult.No) return;

            foreach (DataGridViewRow row in dgvItems.SelectedRows)
            {
                var id = 0;

                if (row.Tag != null)
                {
                    int.TryParse(row.Tag.ToString(), out id);

                    if (idList == null) idList = new List<int>();

                    idList.Add(id);

                    dgvItems.Rows.Remove(row);
                }
            }

            decimal totalQty = 0m, totalAmount = 0m;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                decimal qty = 0m, amount = 0m;

                if (row.Cells[1].Value != null) decimal.TryParse(row.Cells[1].Value.ToString(), out qty);

                if (row.Cells[2].Value != null) decimal.TryParse(row.Cells[2].Value.ToString(), out amount);

                totalQty += qty;

                totalAmount += amount;
            }

            txtTotalQuantity.Text = totalQty.ToString("#,0.00");

            txtTotalAmount.Text = totalAmount.ToString("#,0.00");
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (idList == null || mainForm.IsLoading) return;

            if (idList.Count < 1) return;

            var result = mainForm.ShowMessage("Are you sure yo want to save?", true);

            if (result == DialogResult.No) return;

            var msg = "";

            var success = false;

            try
            {
                mainForm.ShowProgressStatus();

                foreach (var id in idList)
                {
                    var exists = poReturnDtos.PurchaseOrderReturnDetailDtosList.Any(detail => detail.PurchaseOrderDetailId == id);

                    if (exists)
                    {
                        var detailDtos = poReturnDtos.PurchaseOrderReturnDetailDtosList.FirstOrDefault(detail => detail.PurchaseOrderDetailId == id);

                        success = await poReturnController.Delete(id, true);

                        if (!success) break;
                        else await userController.SaveActivity(
                            string.Format("Deletes Purchase Item '{0}' for reference # '{1}'",
                            detailDtos.ItemDtos.PartNo,
                            txtReferenceNumber.Text),
                            mainForm.UserDtos.UserId);
                    }
                }

                msg = success ? "Successfully saved." : "Failed to save. Please contact the administartor.";
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);

                msg = "Sorry for the inconvenience. Some category are not deleted because of internal issue. " +
                    "Contact the administrator for assistance. ";

                success = false;
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

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!started || mainForm.IsLoading) return;

            decimal totalQty = 0m;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                var qtyCell = row.Cells[2];

                if (qtyCell.Value != null)
                {
                    var value = 0m;

                    decimal.TryParse(qtyCell.Value.ToString(), out value);

                    totalQty += value;

                    if (!madeChanges && value > 0) madeChanges = true;
                }
            }

            txtTotalQuantity.Text = totalQty.ToString(numberFormat);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            this.Close();
        }

        private void SalesReturnViewDetailForm_FormClosing(object sender, FormClosingEventArgs e)
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
    }
}
