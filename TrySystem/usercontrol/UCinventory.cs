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
            //guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //guna2DataGridView1.ReadOnly = true;
            //guna2DataGridView1.AllowUserToAddRows = false;

            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.RowHeadersVisible = false; // Hides the left gray strip
            guna2DataGridView1.BorderStyle = BorderStyle.None;
            guna2DataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            guna2DataGridView1.AllowUserToAddRows = false;

            // --- 2. Header Style (Fixing the Black Box) ---
            guna2DataGridView1.EnableHeadersVisualStyles = false; // CRITICAL
            guna2DataGridView1.ColumnHeadersHeight = 45;
            guna2DataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252); // Light Gray
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);   // Dark Gray Text
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252); // Stop header highlight

            // --- 3. Row Style ---
            guna2DataGridView1.ThemeStyle.RowsStyle.BackColor = Color.White;
            guna2DataGridView1.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(239, 246, 255); // Soft Blue Highlight
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            guna2DataGridView1.RowTemplate.Height = 45; // More breathing room

            // --- 4. ADD MODERN BUTTONS (Clean & Flat) ---

            // Clear existing button columns to prevent duplicates
            if (guna2DataGridView1.Columns.Contains("btnEdit")) guna2DataGridView1.Columns.Remove("btnEdit");
            if (guna2DataGridView1.Columns.Contains("btnDelete")) guna2DataGridView1.Columns.Remove("btnDelete");

            // Edit Button (Blue Text, No Box)
            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn();
            btnEdit.Name = "btnEdit";
            btnEdit.HeaderText = ""; // Empty Header
            btnEdit.Text = "EDIT";
            btnEdit.UseColumnTextForButtonValue = true;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.DefaultCellStyle.ForeColor = Color.FromArgb(37, 99, 235); // Royal Blue
            btnEdit.DefaultCellStyle.BackColor = Color.White;
            btnEdit.DefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);

            // Delete Button (Red Text, No Box)
            DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
            btnDelete.Name = "btnDelete";
            btnDelete.HeaderText = ""; // Empty Header
            btnDelete.Text = "DELETE";
            btnDelete.UseColumnTextForButtonValue = true;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.DefaultCellStyle.ForeColor = Color.FromArgb(220, 38, 38); // Red
            btnDelete.DefaultCellStyle.BackColor = Color.White;
            btnDelete.DefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);

            // Add them to the grid
            guna2DataGridView1.Columns.Add(btnEdit);
            guna2DataGridView1.Columns.Add(btnDelete);
        }

        public void LoadInventory()
        {
            //guna2DataGridView10.DataSource = DatabaseHelper.GetAllProducts();
            //if (guna2DataGridView10.Columns.Count > 0)
            //{
            //    guna2DataGridView10.Columns["Id"].Visible = false;
            //    if (guna2DataGridView10.Columns["Price"] != null)
            //    {
            //        guna2DataGridView10.Columns["Price"].DefaultCellStyle.Format = "C2";
            //    }
            //}

            DataTable dt = DatabaseHelper.GetAllProducts();
            guna2DataGridView1.DataSource = dt;

            // 2. Hide Technical Columns ("Id")
            if (guna2DataGridView1.Columns.Contains("Id"))
            {
                guna2DataGridView1.Columns["Id"].Visible = false;
            }

            // 3. HIDE THE DUPLICATE "ACTION" COLUMN
            // If your database has a column named "Action" or "Edit/Delete", hide it here.
            // Based on your screenshot, it looks like column index 0.
            if (guna2DataGridView1.Columns.Count > 0)
            {
                // Try to hide the first column if it's the ugly one
                // You can check the name by debugging, but usually it's index 0
                if (guna2DataGridView1.Columns[0].HeaderText == "Action" || guna2DataGridView1.Columns[0].HeaderText == "Edit/Delete")
                {
                    guna2DataGridView1.Columns[0].Visible = false;
                }
            }

            // 4. Move Buttons to the End
            if (guna2DataGridView1.Columns.Contains("btnEdit"))
                guna2DataGridView1.Columns["btnEdit"].DisplayIndex = guna2DataGridView1.Columns.Count - 2;

            if (guna2DataGridView1.Columns.Contains("btnDelete"))
                guna2DataGridView1.Columns["btnDelete"].DisplayIndex = guna2DataGridView1.Columns.Count - 1;
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
            guna2DataGridView10.DataSource = DatabaseHelper.SearchProducts(textBox1.Text);
            if (guna2DataGridView10.Columns.Count > 0 && guna2DataGridView10.Columns["Price"] != null)
            {
                guna2DataGridView10.Columns["Price"].DefaultCellStyle.Format = "C2";
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
            if (guna2DataGridView10.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = guna2DataGridView10.SelectedRows[0];
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
            if (guna2DataGridView10.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = guna2DataGridView10.SelectedRows[0];
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

        private void itemsBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex >= 0)
            //{
            //    if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "btnEdit")
            //    {
            //        EditProduct();
            //    }
            //    else if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "btnDelete")
            //    {
            //        DeleteProduct();
            //    }
            //}
            if (e.RowIndex < 0) return;

            // 1. Identify which column was clicked by its Name
            // (We named them "btnEdit" and "btnDelete" in the Setup function earlier)
            string colName = guna2DataGridView1.Columns[e.ColumnIndex].Name;

            // 2. Capture the Product ID and Name from the current row
            // Note: ensure "Id" and "ProductName" match your Database column names EXACTLY.
            int id = Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["Id"].Value);
            string prodName = guna2DataGridView1.Rows[e.RowIndex].Cells["ProductName"].Value.ToString();

            // ----------------------------
            // DELETE BUTTON LOGIC
            // ----------------------------
            if (colName == "btnDelete")
            {
                // A. Show the Warning
                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to delete '{prodName}'?\nThis cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                // B. If User clicks "Yes", delete it
                if (result == DialogResult.Yes)
                {
                    bool success = DatabaseHelper.DeleteProduct(id); // Call your DB method

                    if (success)
                    {
                        MessageBox.Show("Product deleted successfully.");
                        LoadInventory(); // REFRESH the grid immediately
                    }
                }
            }

            // ----------------------------
            // EDIT BUTTON LOGIC
            // ----------------------------
            if (colName == "btnEdit")
                {
                    // 1. Get the ID (You already have this)
                    // int id = ... (keep your existing line getting the id)

                    // 2. Fetch the missing "productData" (The DataRow)
                    // We use your DatabaseHelper to get the row for this specific ID
                    DataRow productData = DatabaseHelper.GetProductById(id);

                    // 3. Create the form passing BOTH arguments
                    EditProductForm editForm = new EditProductForm(id, productData);

                    // 4. Show the form
                    editForm.ShowDialog();

                    // 5. Refresh grid after closing
                    LoadInventory();
                }

            }

        private void searchbar_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = guna2DataGridView1.DataSource as DataTable;

            if (dt != null)
            {
                // 2. Clean the search text (remove extra spaces)
                string keyword = searchbar.Text.Trim();

                // Escape the single quote character to prevent crashes (e.g. if user types "O'Neil")
                keyword = keyword.Replace("'", "''");

                // 3. Apply the Filter
                if (string.IsNullOrEmpty(keyword))
                {
                    // If search is empty, show everything
                    dt.DefaultView.RowFilter = "";
                }
                else
                {
                    // Filter Logic: Show row IF (Name matches) OR (Category matches) OR (Brand matches)
                    // The % symbols mean "contains" (e.g., "ppl" finds "Apple")
                    dt.DefaultView.RowFilter = string.Format(
                        "ProductName LIKE '%{0}%' OR Category LIKE '%{0}%' OR Brand LIKE '%{0}%'",
                        keyword
                    );
                }
            }
        }
    }
}
