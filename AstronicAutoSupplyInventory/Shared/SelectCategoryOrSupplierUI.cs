using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLibrary.Dtos;
using AstronicAutoSupplyInventory.EventMesseging;
using System.Diagnostics;

namespace AstronicAutoSupplyInventory.Shared
{
    public partial class SelectCategoryOrSupplierUI : UserControl
    {
        private IEnumerable<CategoryDtos> categories;
        private IEnumerable<SupplierDtos> suppliers;
        private bool isCategory;

        private readonly SelectCategoryOrSupplierEventMessenger selectCategoryOrSupplierEventMessenger;
        private bool started;

        public SelectCategoryOrSupplierUI(SelectCategoryOrSupplierEventMessenger selectCategoryOrSupplierEventMessenger,
            IEnumerable<object> objectList, bool isCategory = true)
        {
            this.selectCategoryOrSupplierEventMessenger = selectCategoryOrSupplierEventMessenger;

            this.isCategory = isCategory;

            if (isCategory) this.categories = (IEnumerable<CategoryDtos>)objectList;
            else this.suppliers = (IEnumerable<SupplierDtos>)objectList;

            InitializeComponent();
        }

        public bool ListViewFocusInvoked()
        {
            if (lstItems.Focused) return false;

            lstItems.Focus();

            if (lstItems.Items.Count > 0)
            {
                lstItems.FocusedItem = lstItems.Items[0];

                lstItems.FocusedItem.ForeColor = SystemColors.HighlightText;

                lstItems.FocusedItem.BackColor = SystemColors.Highlight;

                started = true;
            }

            return true;
        }

        public bool ConfirmSelectionInvoked()
        {
            if (lstItems.FocusedItem == null) return false;

            var item = lstItems.FocusedItem;

            var id = 0;

            int.TryParse(item.Tag.ToString(), out id);

            selectCategoryOrSupplierEventMessenger(id, item.Text, isCategory);

            return true;
        } 

        public void SearchKeyInvoked(string key)
        {
            InitializeList(key);
        }

        private void InitializeList(string key = "")
        {
            var myList = new List<Tuple<int, string>>();

            if (isCategory)
            {
                foreach (var category in this.categories)
                {
                    myList.Add(new Tuple<int, string>(category.CategoryId, category.Name));
                }
            }
            else
            {
                foreach (var supplier in this.suppliers)
                {
                    myList.Add(new Tuple<int, string>(supplier.SupplierId, supplier.Company));
                }
            }

            lstItems.Items.Clear();

            foreach (var item in myList.Where(myItem => myItem.Item2.Contains(key)))
            {
                var lstItem = new ListViewItem
                {
                    Tag = item.Item1,
                    Text = item.Item2
                };

                lstItems.Items.Add(lstItem);
            }

            if (lstItems.Items.Count > 0)
            {
                lstItems.FocusedItem = lstItems.Items[0];
            }

            var colors = new[] { SystemColors.Control, Color.LightBlue };

            var index = 0;

            foreach (ListViewItem item in lstItems.Items)
            {
                item.BackColor = colors[index];

                index++;

                if (index >= 2) index = 0;
            }
        }

        private void SelectCategoryOrSupplierUI_Load(object sender, EventArgs e)
        {
            InitializeList();
        }

        private void SelectCategoryOrSupplierUI_Resize(object sender, EventArgs e)
        {
            lstItems.Columns[0].Width = this.Width;
        }

        private void lstItems_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (!started) return;

            if (e.ItemIndex > 0)
            {
                lstItems.Items[0].ForeColor = Color.Black;

                lstItems.Items[0].BackColor = Color.White;

                started = false;
            }
        }

        private void lstItems_MouseClick(object sender, MouseEventArgs e)
        {
            ConfirmSelectionInvoked();
        }
    }
}
