using AstronicAutoSupplyInventory.EventMesseging;
using AstronicAutoSupplyInventory.Transaction.PurchaseOrder;
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

namespace AstronicAutoSupplyInventory.Supplier
{
    public partial class SupplierListForm : Form
    {
        private SupplierController controller = new SupplierController();
        private UserController userController = new UserController();

        private readonly MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        private bool started;

        public SupplierListForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keys)
        {
            switch (keys)
            {
                case Keys.F1:
                    btnAdd_Click(this, new EventArgs());

                    return true;
                case Keys.F2:
                    btnEdit_Click(this, new EventArgs());

                    return true;
                case Keys.F3:
                    btnDelete_Click(this, new EventArgs());

                    return true;
                case Keys.F5:
                    btnRefresh_Click(this, new EventArgs());

                    return true;
                case Keys.Enter:
                    if (!dgvItems.Focused) break;

                    btnViewItems_Click(this, new EventArgs());

                    return true;
                case Keys.Escape:
                    btnClose_Click(this, new EventArgs());

                    return true;
                case Keys.Alt | Keys.S:
                    txtSearch.Focus();

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

        private void InitializePrivileges()
        {
            var privileges = mainForm.UserDtos.UserPrivilegeDtosList;

            if (privileges.Count() > 0)
            {
                btnAdd.Enabled = privileges.Any(privilege => privilege.Action == "Add Supplier" && privilege.IsEnable);

                btnEdit.Enabled = privileges.Any(privilege => privilege.Action == "Edit Supplier" && privilege.IsEnable);

                btnDelete.Enabled = privileges.Any(privilege => privilege.Action == "Delete Supplier" && privilege.IsEnable);

                btnViewItems.Enabled = privileges.Any(privilege => privilege.Action == "View Purchase Order History Per Supplier" && privilege.IsEnable);
            }
        }

        private async Task InitializeSupplier(string key = "")
        {
            var listDtos = await controller.GetAll(key);

            dgvItems.Rows.Clear();

            foreach (var item in listDtos)
            {
                var row = dgvItems.Rows[dgvItems.Rows.Add()];

                row.Tag = item.SupplierId;

                row.Cells[0].Value = item.Company;

                row.Cells[1].Value = item.ContactPerson;

                row.Cells[2].Value = item.Address;

                row.Cells[3].Value = item.ContactNo;
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

        private async void SupplierListForm_Load(object sender, EventArgs e)
        {
            try
            {
                mainForm.ShowProgressStatus();

                InitializePrivileges();

                await InitializeSupplier();

                txtSearch.Focus();

                started = true;
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }

            finally { mainForm.ShowProgressStatus(false); }
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!started || txtSearch.Focused) return;

            try
            {
                await InitializeSupplier(txtSearch.Text);
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnAdd.Enabled) return;

            var supplierForm = new AddEditSupplierForm(new AddNewEventMessenger(AddNewSupplierInvoked));

            supplierForm.ShowDialog();
        }

        private async void AddNewSupplierInvoked(int id)
        {
            if (id > 0) await InitializeSupplier();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnEdit.Enabled) return;

            var id = 0;

            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            int.TryParse(row.Tag.ToString(), out id);

            if (id < 1) return;

            var supplierForm = new AddEditSupplierForm(new AddNewEventMessenger(AddNewSupplierInvoked), id);

            supplierForm.ShowDialog();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnDelete.Enabled) return;

            var msg = "Are you sure you want to delete?";

            var result = mainForm.ShowMessage(msg, true); //MessageBox.Show(this, msg, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == System.Windows.Forms.DialogResult.No) return;

            var success = false;

            try
            {
                mainForm.ShowProgressStatus();

                foreach (DataGridViewRow row in dgvItems.SelectedRows)
                {
                    var itemId = 0;

                    int.TryParse(row.Tag.ToString(), out itemId);

                    if (itemId < 1) continue;

                    success = await controller.Delete(itemId);

                    if (!success) break;
                    else await userController.SaveActivity(
                        string.Format("Deleted Supplier '{0}'", row.Cells[0].Value),
                        mainForm.UserDtos.UserId);

                    dgvItems.Rows.Remove(row);
                }

                msg = success ? "Successfully deleted." : "Cannot be deleted because the selected supplier has referenced.";
            }
            catch (Exception ex)
            {
                mainForm.HandleException(ex);

                msg = "Sorry for the inconvenience. Some supplier are not deleted because of internal issue. " +
                    "Contact the administrator for assistance. ";
            }

            finally
            {
                mainForm.ShowProgressStatus(false);

                mainForm.ShowMessage(msg, false, !success);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            SupplierListForm_Load(sender, e);
        }

        private void btnViewItems_Click(object sender, EventArgs e)
        {
            if (mainForm.IsLoading || !btnViewItems.Enabled) return;

            var id = 0;

            if (dgvItems.SelectedRows.Count < 1) return;

            var row = dgvItems.SelectedRows[0];

            int.TryParse(row.Tag.ToString(), out id);

            if (id < 1) return;

            var poPerCustomerForm = new PurchaseOrderPerSupplierForm(id);

            poPerCustomerForm.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pnlHelp.Visible = !pnlHelp.Visible;
        }
    }
}
