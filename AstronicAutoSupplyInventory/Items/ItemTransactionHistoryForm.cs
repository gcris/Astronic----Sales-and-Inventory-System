using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Shared;
using AstronicAutoSupplyInventory.Transaction.PurchaseOrder;
using AstronicAutoSupplyInventory.Transaction.SalesInvoice;
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
    public partial class ItemTransactionHistoryForm : Form
    {
        private int itemId;

        private UserController userController = new UserController();
        private ItemController itemController = new ItemController();

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public ItemTransactionHistoryForm(int itemId = 0)
        {
            this.itemId = itemId;

            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.Enter:
                    btnDetails_Click(btnDetails, new EventArgs());

                    return true;
                case Keys.Escape:
                    btnClose_Click(this, new EventArgs());

                    return true;
                case Keys.Down:
                    if (dgvItems.Focused) break;

                    dgvItems.Focus();

                    return true;
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
                case Keys.Alt | Keys.S:
                    txtSearch.Focus();

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        public async Task InitializeItems(string key = "")
        {
            if (itemId < 1) return;

            var itemTransactions = await userController.GetActivities(itemId, DateTime.MinValue, DateTime.MinValue, key, true);

            if (!string.IsNullOrWhiteSpace(key)) itemTransactions = itemTransactions.Where(item => item.ReferenceNumber.Contains(key));

            dgvItems.Rows.Clear();

            foreach (var item in itemTransactions)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = item.ReferenceNumber;

                row.Cells[0].Value = item.ReferenceNumber;

                row.Cells[1].Value = item.Date;

                row.Cells[2].Value = item.Action;

                row.Cells[3].Value = item.Quantity;

                row.Cells[4].Value = item.Amount.ToString("#,0.00;(#,0.00)''");

                row.Cells[5].Value = item.CurrentStock;

                row.Cells[6].Value = item.Remarks;

                row.Cells[7].Value = item.UserName;
            }

            //if (itemTransactions.Count() > 0) dgvItems.Sort(dgvItems.Columns[1], ListSortDirection.Descending);

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                row.DefaultCellStyle.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
        }

        private async void ItemTransactionHistoryForm_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                var itemDtos = await itemController.Find(itemId);

                if (itemDtos != null)
                {
                    lblTitle.Text = string.Format("{0}{1}{2}",
                        itemDtos.CategoryName,
                        string.IsNullOrWhiteSpace(itemDtos.PartNo) ? "" :
                            string.Format(" | {0}", itemDtos.PartNo),
                        string.IsNullOrWhiteSpace(itemDtos.SupplierName) ? "" :
                            string.Format(" | {0}", itemDtos.SupplierName));
                }

                await InitializeItems();
            }
            catch (Exception ex) { mainForm.HandleException(ex); }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            if (row.Tag == null) return;

            var referenceNumber = row.Tag.ToString();

            if (string.IsNullOrWhiteSpace(referenceNumber)) return;

            var type = row.Cells[2].Value.ToString();

            Form currentForm = null;

            switch (type) {
                case "Sales Invoice (OUT)":
                    currentForm = new SalesInvoiceViewDetailForm(referenceNumber);
                    
                    break;
                case "Sales Return (IN-RETURNED)":
                    currentForm = new SalesReturnDetailForm(referenceNumber);

                    break;
                case "Purchase Order (IN)":
                    currentForm = new PurchaseOrderViewDetailForm(referenceNumber);

                    break;
                case "Purchase Order Return (OUT-RETURNED)":
                    currentForm = new POReturnDetailForm(referenceNumber);

                    break;
                default:
                    return;
            }

            mainForm.ChangeForm(currentForm);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            var searchItemForm = (SearchItemForm)Application.OpenForms["SearchItemForm"];

            if (searchItemForm != null)
            {
                this.Close();
                searchItemForm.Show();
                searchItemForm.BringToFront();
               
            }
            else this.Close();
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }

        private async void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (mainForm.IsLoading || string.IsNullOrWhiteSpace(txtSearch.Text)) return;

            if (e.KeyData == Keys.Enter)
            {
                await InitializeItems(txtSearch.Text);
            }
        }
    }
}
