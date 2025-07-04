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

namespace AstronicAutoSupplyInventory.Categories
{
    public partial class AddEditCategoryForm : Form
    {
        private int categoryId;
        private string categoryName;

        private readonly AddNewEventMessenger addCategoryEventMessenger;
        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        private CategoryController categoryController = new CategoryController();
        private UserController userController = new UserController();

        public AddEditCategoryForm(int categoryId = 0, AddNewEventMessenger addCategoryEventMessenger = null)
        {
            this.categoryId = categoryId;

            this.addCategoryEventMessenger = addCategoryEventMessenger;

            InitializeComponent();
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
                case Keys.Alt | Keys.H:
                    lnkHelp_LinkClicked(lnkHelp, new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()));

                    return true;
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        private async Task InitializeCategory()
        {
            if (categoryId < 1) return;

            var queryCategoryDtos = await categoryController.Find(categoryId);

            if (queryCategoryDtos == null) return;

            txtCategoryName.Text = queryCategoryDtos.Name;

            txtMinimumStock.Text = queryCategoryDtos.MinimumStock.ToString("#,0.00;(#,0.00);''");

            categoryName = queryCategoryDtos.Name;
        }

        private async Task<bool> IsValid()
        {
            var msg = "";

            var name = txtCategoryName.Text.Trim();

            var stock = 0m;

            var validStock = decimal.TryParse(txtMinimumStock.Text, out stock);

            if (string.IsNullOrWhiteSpace(name))
            {
                msg = "Category Name is requried.";

                txtCategoryName.Focus();
            }
            else if (!await categoryController.IsValidName(categoryName, name))
            {
                msg = "Category Name already in used.";

                txtCategoryName.Focus();
            }
            if (!validStock && !string.IsNullOrWhiteSpace(txtMinimumStock.Text))
            {
                msg = "Invalid Minimum Stock.";

                txtMinimumStock.Focus();
            }

            if (msg.Length > 0) mainForm.ShowMessage(msg);

            return msg.Length == 0;
        }

        private async void AddEditCategory_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                await InitializeCategory();
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (!await IsValid() || mainForm.IsLoading) return;

            var result = mainForm.ShowMessage("Are you sure you want to save changes?", true);

            if (result == System.Windows.Forms.DialogResult.No) return;

            try
            {
                mainForm.ShowProgressStatus();

                var stock = 0m;

                decimal.TryParse(txtMinimumStock.Text, out stock);

                var id = await categoryController.Save(
                    new CategoryDtos 
                    { 
                        CategoryId = categoryId, 
                        Name = txtCategoryName.Text.Trim(),
                        LastUpdate = DateTime.Now,
                        MinimumStock = stock
                    });

                if (id > 0)
                {
                    await userController.SaveActivity(
                        string.Format(id < 1 ? "Creates new Category '{0}'" : "Updates Category '{0}'", txtCategoryName.Text), 
                        mainForm.UserDtos.UserId);

                    mainForm.ShowMessage("Successfully saved");

                    if (addCategoryEventMessenger != null) addCategoryEventMessenger(id);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddEditCategoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = mainForm.IsLoading;

            if (e.Cancel) return;

            var result = mainForm.ShowMessage("Are you sure you want to close?", true);

            e.Cancel = result == System.Windows.Forms.DialogResult.No;
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }
    }
}
