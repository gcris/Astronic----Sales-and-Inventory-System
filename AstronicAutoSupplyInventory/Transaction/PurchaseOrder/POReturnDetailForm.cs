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
    public partial class POReturnDetailForm : Form
    {
        private readonly string referenceNumber;
        private readonly string numberFormat = "#,0.00;(#,0.00);''";
        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        private bool started;

        private PurchaseOrderReturnController purchaseOrderReturnController = new PurchaseOrderReturnController();

        public POReturnDetailForm(string referenceNumber)
        {
            this.referenceNumber = referenceNumber;

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

        private async Task InitializePOReturn(string referenceNumber = "")
        {
            if (string.IsNullOrWhiteSpace(referenceNumber)) return;

            var salesReturnDtos = await purchaseOrderReturnController.Find(referenceNumber);

            if (salesReturnDtos == null) return;

            txtReferenceNumber.Text = salesReturnDtos.ReferenceNumber;

            dtpDate.Value = salesReturnDtos.Date;

            dgvItems.Rows.Clear();

            foreach (var item in salesReturnDtos.PurchaseOrderReturnDetailDtosList)
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

                row.Cells[8].Value = item.Amount.ToString(numberFormat);
            }

            txtTotalQuantity.Text = salesReturnDtos.TotalQuantity.ToString(numberFormat);

            txtTotalAmount.Text = salesReturnDtos.TotalAmount.ToString(numberFormat);

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                row.DefaultCellStyle.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
        }

        private async void POReturnDetailForm_Load(object sender, EventArgs e)
        {
            mainForm.ShowProgressStatus();

            try
            {
                await InitializePOReturn(referenceNumber);

                started = true;
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void txtReferenceNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && !string.IsNullOrWhiteSpace(txtReferenceNumber.Text) && started)
            {
                mainForm.ShowProgressStatus();

                try
                {
                    await InitializePOReturn(txtReferenceNumber.Text);
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
