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

namespace AstronicAutoSupplyInventory.Shared
{
    public partial class PriceInquiryForm : Form
    {
        private ItemController itemController = new ItemController();

        public PriceInquiryForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.F1:
                    btnConfirm_Click(this, new EventArgs());

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

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            var searchItemForm = (SearchItemForm)Application.OpenForms["SearchItemForm"];

            if (searchItemForm == null)
            {
                searchItemForm = new SearchItemForm(new ConfirmSearchItemEventMessenger(SearchItemInvoked));
            }

            searchItemForm.ShowDialog();
        }

        private async void SearchItemInvoked(List<int> idList)
        {
            var searchItemForm = (SearchItemForm)Application.OpenForms["SearchItemForm"];

            searchItemForm.Close();

            var id = idList.FirstOrDefault();

            var itemDtos = await itemController.Find(id);

            if (itemDtos == null)
            {
                MessageBox.Show(this, "Price Inquiry", "No item found.", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            lblItemName.Text = itemDtos.CategoryName;

            lblItemNo.Text = itemDtos.PartNo;

            lblMade.Text = itemDtos.Made;

            lblMake.Text = itemDtos.Make;

            lblModel.Text = itemDtos.Model;

            lblQOH.Text = itemDtos.QuantityOnHand.ToString("#,0.00");

            lblSellingPrice.Text = string.Format("Php {0}", itemDtos.Price1.ToString("#,0.00"));

            lblSize.Text = itemDtos.Size;

            lblBrand.Text = itemDtos.BrandName;

            if (itemDtos.MinimumStock > 0)
            {
                var percentage = itemDtos.QuantityOnHand / itemDtos.MinimumStock;

                lblStockStatus.Text = percentage <= 0 ? "Out of Stock" : 
                    percentage <= 0.4m ? "At Minimum Stock" : "On Stock";

                lblStockStatus.ForeColor = percentage <= 0.4m ? Color.Red :
                    percentage <= 0.7m ? Color.Orange : Color.Green;
            }

            lblQOH.ForeColor = lblStockStatus.ForeColor;
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
