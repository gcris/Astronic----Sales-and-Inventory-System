using AstronicAutoSupplyInventory.EventMesseging;
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
    public partial class PoDetailListForm : Form
    {
        private readonly ConfirmPurchaseItemsToReturnEventMessenger confirmItemsToReturnEventMessenger;
        //private readonly string poNumber;
        private readonly string numberFormat = "#,0.00;(#,0.00);''";

        private PurchaseOrderController purchaseOrderController = new PurchaseOrderController();

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private readonly IEnumerable<PurchaseOrderDetailDtos> purchaseOrderDetailDtosList;

        public PoDetailListForm(ConfirmPurchaseItemsToReturnEventMessenger confirmItemsToReturnEventMessenger,
            IEnumerable<PurchaseOrderDetailDtos> purchaseOrderDetailDtosList)
        {
            this.confirmItemsToReturnEventMessenger = confirmItemsToReturnEventMessenger;

            this.purchaseOrderDetailDtosList = purchaseOrderDetailDtosList;

            InitializeComponent();

            var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            Width = screen.Width - 100;

            Height = screen.Height - 100;
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Enter:
                    btnConfirm_Click(btnConfirm, new EventArgs());

                    return true;
                case Keys.Escape:
                    btnCancel_Click(btnCancel, new EventArgs());

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

        private void InitializePurchaseOrder()
        {
            dgvItems.Rows.Clear();

            foreach (var detailDtos in purchaseOrderDetailDtosList)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = detailDtos.PurchaseOrderDetailId;

                row.Cells[0].Tag = detailDtos.ItemDtos.ItemId;

                row.Cells[0].Value = detailDtos.ItemDtos.CategoryName;

                row.Cells[1].Value = detailDtos.ItemDtos.PartNo;

                row.Cells[2].Value = detailDtos.ItemDtos.BrandName;

                row.Cells[3].Value = detailDtos.ItemDtos.Model;

                row.Cells[4].Value = detailDtos.ItemDtos.Make;

                row.Cells[5].Value = detailDtos.ItemDtos.Made;

                row.Cells[6].Value = detailDtos.ItemDtos.Size;

                row.Cells[7].Value = detailDtos.Quantity.ToString(numberFormat);

                row.Cells[8].Value = detailDtos.UnitPrice.ToString(numberFormat);

                row.Cells[9].Value = detailDtos.TotalAmount.ToString(numberFormat);
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

        private void SalesInvoiceDetailListForm_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                InitializePurchaseOrder();

                dgvItems.Focus();
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count < 1 || mainForm.IsLoading) return;

            var list = new List<PurchaseOrderDetailDtos>();

            foreach (DataGridViewRow row in dgvItems.SelectedRows)
            {
                var detailId = 0;

                if (row.Tag != null) int.TryParse(row.Tag.ToString(), out detailId);

                var exists = purchaseOrderDetailDtosList.Any(detail => detail.PurchaseOrderDetailId == detailId);

                if (exists) list.Add(purchaseOrderDetailDtosList.FirstOrDefault(detail => detail.PurchaseOrderDetailId == detailId));
            }

            confirmItemsToReturnEventMessenger(list);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading) return;

            confirmItemsToReturnEventMessenger(null);
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }
    }
}
