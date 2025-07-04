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
    public partial class PurchaseOrderViewDetailForm : Form
    {
        private PurchaseOrderController controller = new PurchaseOrderController();

        private readonly string poNumber;
        private readonly string numberFormat = "#,0.00;(#,0.00);''";
        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
        private bool started;

        public PurchaseOrderViewDetailForm(string poNumber)
        {
            this.poNumber = poNumber;

            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Escape:
                    this.Close();

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task InitializePurchaseOrder(string poNumber = "")
        {
            if (string.IsNullOrWhiteSpace(poNumber)) return;

            var purchaseOrderDtos = await controller.Find(poNumber);

            if (purchaseOrderDtos == null) return;

            txtPONumber.Text = purchaseOrderDtos.PONumber;

            txtSupplier.Text = purchaseOrderDtos.SupplierName;

            dtpDate.Value = purchaseOrderDtos.Date;

            dgvItems.Rows.Clear();

            foreach (var item in purchaseOrderDtos.PurchaseOrderDetailDtosList)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Cells[0].Value = item.ItemDtos.CategoryName;

                row.Cells[1].Value = item.ItemDtos.PartNo;

                row.Cells[2].Value = item.ItemDtos.BrandName;

                row.Cells[3].Value = item.ItemDtos.Model;

                row.Cells[4].Value = item.ItemDtos.Make;

                row.Cells[5].Value = item.ItemDtos.Made;

                row.Cells[6].Value = item.ItemDtos.Size;

                row.Cells[7].Value = item.Quantity.ToString(numberFormat);

                row.Cells[8].Value = item.UnitPrice.ToString(numberFormat);

                row.Cells[9].Value = item.Discount.ToString(numberFormat);

                row.Cells[10].Value = item.TotalAmount.ToString(numberFormat);
            }

            txtRemarks.Text = purchaseOrderDtos.Remarks;

            txtTotalQuantity.Text = purchaseOrderDtos.TotalQuantity.ToString(numberFormat);

            txtTotalAmount.Text = purchaseOrderDtos.GrandTotalAmount.ToString(numberFormat);

            txtTotalDiscount.Text = purchaseOrderDtos.TotalDiscount.ToString(numberFormat);

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (DataGridViewRow currow in dgvItems.Rows)
            {
                currow.DefaultCellStyle.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
        }

        private async void PurchaseOrderViewDetailForm_Load(object sender, EventArgs e)
        {
            mainForm.ShowProgressStatus();

            try
            {
                await InitializePurchaseOrder(poNumber);

                started = true;
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void txtPONumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && !string.IsNullOrWhiteSpace(txtPONumber.Text) && started)
            {
                mainForm.ShowProgressStatus();

                try
                {
                    await InitializePurchaseOrder(txtPONumber.Text);
                }
                catch (Exception ex)
                {
                    mainForm.HandleException(ex);
                }

                finally { mainForm.ShowProgressStatus(false); }
            }
        }
    }
}
