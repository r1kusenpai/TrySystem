using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrySystem.usercontrol
{
    public partial class UCinventory : UserControl
    {
        public UCinventory()
        {
            InitializeComponent();
            LoadInventory();
            SetupDataGridView();

            // Subscribe to database events so the UI stays in sync with data changes.
            DatabaseHelper.ProductAdded += DatabaseHelper_ProductAdded;
            DatabaseHelper.LowStockAlert += DatabaseHelper_LowStockAlert;
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }

        public void LoadInventory()
        {
            dataGridView1.DataSource = DatabaseHelper.GetAllProducts();
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["Id"].Visible = false;
                if (dataGridView1.Columns["Price"] != null)
                {
                    dataGridView1.Columns["Price"].DefaultCellStyle.Format = "C2";
                }
            }
        }

        private void DatabaseHelper_ProductAdded(object sender, ProductEventArgs e)
        {
            // Refresh grid when a product is added or updated.
            LoadInventory();
        }

        private void DatabaseHelper_LowStockAlert(object sender, ProductEventArgs e)
        {
            // Simple notification example for low stock using delegates & events.
            MessageBox.Show(
                $"Low stock alert for '{e.ProductName}' (Qty: {e.Quantity}) in category '{e.Category}'.",
                "Low Stock Alert",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Navigate to Add Item tab
            Form1 parentForm = this.ParentForm as Form1;
            if (parentForm != null)
            {
                // Trigger the additemtab click event
                parentForm.TriggerAddItemTab();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                LoadInventory();
            }
            else
            {
                SearchProducts();
            }
        }

        private void SearchProducts()
        {
            dataGridView1.DataSource = DatabaseHelper.SearchProducts(textBox1.Text);
            if (dataGridView1.Columns.Count > 0 && dataGridView1.Columns["Price"] != null)
            {
                dataGridView1.Columns["Price"].DefaultCellStyle.Format = "C2";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SearchProducts();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EditProduct();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DeleteProduct();
        }

        private void EditProduct()
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            int productId = Convert.ToInt32(selectedRow.Cells["Id"].Value);

            // Get product details
            DataRow productData = DatabaseHelper.GetProductById(productId);
            if (productData == null)
            {
                MessageBox.Show("Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create and show edit form
            EditProductForm editForm = new EditProductForm(productId, productData);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadInventory(); // Refresh the inventory
            }
        }

        private void DeleteProduct()
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            string productName = selectedRow.Cells["ProductName"].Value.ToString();
            int productId = Convert.ToInt32(selectedRow.Cells["Id"].Value);

            // Confirm deletion
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete '{productName}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool success = DatabaseHelper.DeleteProduct(productId);
                if (success)
                {
                    MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadInventory(); // Refresh the inventory
                }
            }
        }

        private void UCinventory_Load(object sender, EventArgs e)
        {

        }
    }
}
