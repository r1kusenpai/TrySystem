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
    public partial class UCreports : UserControl
    {
        public UCreports()
        {
            InitializeComponent();
            LoadReports();
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.ReadOnly = true;
            dataGridView2.AllowUserToAddRows = false;
        }

        private void LoadReports()
        {
            // Load low stock count
            int lowStockCount = DatabaseHelper.GetLowStockCount();
            label21.Text = lowStockCount.ToString();

            // Load low stock products categorized
            DataTable lowStockData = DatabaseHelper.GetLowStockProducts();
            dataGridView1.DataSource = lowStockData;

            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["Id"].Visible = false;
                if (dataGridView1.Columns["Price"] != null)
                {
                    dataGridView1.Columns["Price"].DefaultCellStyle.Format = "C2";
                }
            }

            // Load categories for filtering
            LoadCategories();

            // Load category breakdown with details
            LoadCategoryAlerts();
        }

        private void LoadCategories()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("All");
            
            DataTable lowStockData = DatabaseHelper.GetLowStockProducts();
            var categories = lowStockData.AsEnumerable()
                .Select(row => row.Field<string>("Category"))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            foreach (var category in categories)
            {
                if (!string.IsNullOrEmpty(category))
                {
                    comboBox1.Items.Add(category);
                }
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCategory = comboBox1.SelectedItem?.ToString() ?? "All";
            FilterByCategory(selectedCategory);
        }

        private void LoadCategoryAlerts()
        {
            DataTable categoryData = DatabaseHelper.GetLowStockByCategory();
            
            // Display categorized low stock in DataGridView
            dataGridView2.DataSource = categoryData;
            
            if (dataGridView2.Columns.Count > 0)
            {
                // Rename columns for better display
                if (dataGridView2.Columns["Category"] != null)
                {
                    dataGridView2.Columns["Category"].HeaderText = "Category";
                    dataGridView2.Columns["Category"].Width = 200;
                }
                if (dataGridView2.Columns["Count"] != null)
                {
                    dataGridView2.Columns["Count"].HeaderText = "Number of Products";
                    dataGridView2.Columns["Count"].Width = 150;
                }
                if (dataGridView2.Columns["TotalQuantity"] != null)
                {
                    dataGridView2.Columns["TotalQuantity"].HeaderText = "Total Units";
                    dataGridView2.Columns["TotalQuantity"].Width = 150;
                }
            }
            
            // Update the alert panel to show categorized information
            if (categoryData.Rows.Count > 0)
            {
                string categoryDetails = "Low Stock by Category:\n";
                foreach (DataRow row in categoryData.Rows)
                {
                    string category = row["Category"].ToString();
                    int count = Convert.ToInt32(row["Count"]);
                    int totalQty = Convert.ToInt32(row["TotalQuantity"]);
                    categoryDetails += $"{category}: {count} products ({totalQty} units)\n";
                }
                label20.Text = categoryDetails.Trim();
            }
            else
            {
                label20.Text = "No low stock products by category";
            }
        }

        private void FilterByCategory(string category)
        {
            DataTable allData = DatabaseHelper.GetLowStockProducts();
            if (string.IsNullOrEmpty(category) || category == "All")
            {
                dataGridView1.DataSource = allData;
            }
            else
            {
                DataView dv = new DataView(allData);
                dv.RowFilter = $"Category = '{category.Replace("'", "''")}'";
                dataGridView1.DataSource = dv.ToTable();
            }

            if (dataGridView1.Columns.Count > 0 && dataGridView1.Columns["Price"] != null)
            {
                dataGridView1.Columns["Price"].DefaultCellStyle.Format = "C2";
            }
        }
    }
}
