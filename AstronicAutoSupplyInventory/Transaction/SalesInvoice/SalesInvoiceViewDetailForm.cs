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

namespace AstronicAutoSupplyInventory.Transaction.SalesInvoice
{
    public partial class SalesInvoiceViewDetailForm : Form
    {
        private SalesInvoiceController controller = new SalesInvoiceController();

        private readonly string orNumber;
        private readonly string numberFormat = "#,0.00;(#,0.00);''";
        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        private bool started;

        public SalesInvoiceViewDetailForm(string orNumber)
        {
            this.orNumber = orNumber;

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

        private async Task InitializeSalesInvoice(string orNumber = "")
        {
            if (string.IsNullOrWhiteSpace(orNumber)) return;

            var salesInvoiceDtos = await controller.Find(orNumber);

            if (salesInvoiceDtos == null) return;

            txtORNumber.Text = salesInvoiceDtos.ORNumber;

            txtCustomer.Text = salesInvoiceDtos.CustomerName;

            dtpDate.Value = salesInvoiceDtos.Date;

            dgvItems.Rows.Clear();

            foreach (var item in salesInvoiceDtos.SalesInvoiceDetailDtosList)
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

                row.Cells[8].Value = item.CurrentCost.ToString(numberFormat);
               
                row.Cells[9].Value = item.Discount.ToString(numberFormat);

                row.Cells[10].Value = item.UnitPrice.ToString(numberFormat);

                row.Cells[11].Value = item.TotalAmount.ToString(numberFormat);
            }

            txtRemarks.Text = salesInvoiceDtos.Remarks;

            txtTotalQuantity.Text = salesInvoiceDtos.TotalQuantity.ToString(numberFormat);

            txtTotalAmount.Text = salesInvoiceDtos.TotalAmount.ToString(numberFormat);

            txtTotalDiscount.Text = salesInvoiceDtos.TotalDiscount.ToString(numberFormat);

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (DataGridViewRow currow in dgvItems.Rows)
            {
                currow.DefaultCellStyle.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
        }

        private async void SalesInvoiceViewDetailForm_Load(object sender, EventArgs e)
        {
            mainForm.ShowProgressStatus();

            try
            {
                await InitializeSalesInvoice(orNumber);

                started = true;
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void txtORNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && !string.IsNullOrWhiteSpace(txtORNumber.Text) && started)
            {
                mainForm.ShowProgressStatus();

                try
                {
                    await InitializeSalesInvoice(txtORNumber.Text);
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
