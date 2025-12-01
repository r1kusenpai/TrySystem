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
    public partial class UCpurchase : UserControl
    {
        public UCpurchase()
        {
            InitializeComponent();

            // 1. Initial Setup
            SetupDataGridView();

            // 2. Load Data
            LoadPurchaseData();

            // 3. Subscribe to the Grid Click Event
            guna2DataGridView1.CellContentClick += DataGridView1_CellContentClick;

            // (Optional) Keep the main "Add" button event if needed
            if (purchasestock != null)
            {
                purchasestock.Click += guna2Button1_Click;
            }
        }

        private void SetupDataGridView()
        {
            // --- 1. General Grid Settings ---
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.RowHeadersVisible = false;
            guna2DataGridView1.BorderStyle = BorderStyle.None;
            guna2DataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; // Thin lines

            // --- 2. Header Style (The "Clean Gray" Look) ---
            guna2DataGridView1.EnableHeadersVisualStyles = false;
            guna2DataGridView1.ColumnHeadersHeight = 50; // Taller header looks more premium
            guna2DataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // Header Colors
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249); // Soft Gray Background
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 116, 139);   // Slate Gray Text
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold); // Smaller, crisp font
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(241, 245, 249); // Disable highlight on header

            // --- 3. Row Style ---
            guna2DataGridView1.ThemeStyle.RowsStyle.BackColor = Color.White;
            guna2DataGridView1.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(51, 65, 85); // Dark Slate (Not pure black)
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(239, 246, 255); // Pale Blue Highlight
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(51, 65, 85);
            guna2DataGridView1.RowTemplate.Height = 50; // Taller rows for breathing room

            // --- 4. ACTION BUTTONS (The "Ghost" Style) ---
            // Remove old columns first
            if (guna2DataGridView1.Columns.Contains("btnEdit")) guna2DataGridView1.Columns.Remove("btnEdit");
            if (guna2DataGridView1.Columns.Contains("btnDelete")) guna2DataGridView1.Columns.Remove("btnDelete");

            // EDIT Button (Clean Blue Text, No Box)
            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn();
            btnEdit.Name = "btnEdit";
            btnEdit.HeaderText = "";
            btnEdit.Text = "EDIT";
            btnEdit.UseColumnTextForButtonValue = true;
            btnEdit.FlatStyle = FlatStyle.Flat; // Removes 3D effect
            btnEdit.DefaultCellStyle.ForeColor = Color.FromArgb(37, 99, 235); // Royal Blue
            btnEdit.DefaultCellStyle.BackColor = Color.White; // Matches row background
            btnEdit.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255); // Matches row selection
            btnEdit.DefaultCellStyle.SelectionForeColor = Color.FromArgb(37, 99, 235);
            btnEdit.DefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);

            // DELETE Button (Clean Red Text, No Box)
            DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn();
            btnDelete.Name = "btnDelete";
            btnDelete.HeaderText = "";
            btnDelete.Text = "DELETE";
            btnDelete.UseColumnTextForButtonValue = true;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.DefaultCellStyle.ForeColor = Color.FromArgb(220, 38, 38); // Red
            btnDelete.DefaultCellStyle.BackColor = Color.White;
            btnDelete.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
            btnDelete.DefaultCellStyle.SelectionForeColor = Color.FromArgb(220, 38, 38);
            btnDelete.DefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);

            // Add buttons
            guna2DataGridView1.Columns.Add(btnEdit);
            guna2DataGridView1.Columns.Add(btnDelete);
        }

        private void LoadPurchaseData()
        {
            try
            {
                label1.Text = DatabaseHelper.GetTotalPurchaseOrders().ToString();
                label11.Text = DatabaseHelper.GetTotalPurchaseValue().ToString("N2");
                label15.Text = DatabaseHelper.GetActiveSuppliers().ToString();

                LoadPurchaseHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading purchase data: {ex.Message}");
            }
        }

        private void LoadPurchaseHistory()
        {
            DataTable dt = DatabaseHelper.GetPurchaseOrderHistory();
            guna2DataGridView1.DataSource = dt;

            if (guna2DataGridView1.Columns.Count > 0)
            {
                // --- 1. RESET AUTO-SIZE ---
                // We turn off global "Fill" so we can customize specific columns
                guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                // --- 2. HIDE TECHNICAL COLUMNS ---
                if (guna2DataGridView1.Columns.Contains("Id")) guna2DataGridView1.Columns["Id"].Visible = false;
                if (guna2DataGridView1.Columns.Contains("ProductId")) guna2DataGridView1.Columns["ProductId"].Visible = false;
                if (guna2DataGridView1.Columns.Contains("SupplierId")) guna2DataGridView1.Columns["SupplierId"].Visible = false;
                if (guna2DataGridView1.Columns.Contains("Status")) guna2DataGridView1.Columns["Status"].Visible = false;

                // --- 3. CONFIGURE TEXT COLUMNS (STRETCH) ---
                // These take up the available "White Space"

                if (guna2DataGridView1.Columns.Contains("SupplierName"))
                {
                    guna2DataGridView1.Columns["SupplierName"].HeaderText = "Supplier"; // Clean Name
                    guna2DataGridView1.Columns["SupplierName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    guna2DataGridView1.Columns["SupplierName"].FillWeight = 30; // 30% width priority
                }

                if (guna2DataGridView1.Columns.Contains("ProductName"))
                {
                    guna2DataGridView1.Columns["ProductName"].HeaderText = "Product"; // Clean Name
                    guna2DataGridView1.Columns["ProductName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    guna2DataGridView1.Columns["ProductName"].FillWeight = 30; // 30% width priority
                }

                // --- 4. CONFIGURE NUMERIC COLUMNS (SHRINK TO FIT) ---
                // These will snap to the size of the text/header so there are no gaps

                if (guna2DataGridView1.Columns.Contains("Quantity"))
                {
                    guna2DataGridView1.Columns["Quantity"].HeaderText = "Qty";
                    guna2DataGridView1.Columns["Quantity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    guna2DataGridView1.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    guna2DataGridView1.Columns["Quantity"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (guna2DataGridView1.Columns.Contains("Price"))
                {
                    guna2DataGridView1.Columns["Price"].DefaultCellStyle.Format = "C2";
                    guna2DataGridView1.Columns["Price"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    guna2DataGridView1.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    guna2DataGridView1.Columns["Price"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (guna2DataGridView1.Columns.Contains("TotalAmount"))
                {
                    guna2DataGridView1.Columns["TotalAmount"].HeaderText = "Total";
                    guna2DataGridView1.Columns["TotalAmount"].DefaultCellStyle.Format = "C2";
                    // MinimumWidth ensures headers don't get cut off if value is small
                    guna2DataGridView1.Columns["TotalAmount"].MinimumWidth = 100;
                    guna2DataGridView1.Columns["TotalAmount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    guna2DataGridView1.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    guna2DataGridView1.Columns["TotalAmount"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if (guna2DataGridView1.Columns.Contains("OrderDate"))
                {
                    guna2DataGridView1.Columns["OrderDate"].HeaderText = "Date";
                    guna2DataGridView1.Columns["OrderDate"].DefaultCellStyle.Format = "MM/dd/yyyy";
                    guna2DataGridView1.Columns["OrderDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    guna2DataGridView1.Columns["OrderDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    guna2DataGridView1.Columns["OrderDate"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // --- 5. BUTTONS (FIXED SIZE) ---
                if (guna2DataGridView1.Columns.Contains("btnEdit"))
                {
                    guna2DataGridView1.Columns["btnEdit"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    guna2DataGridView1.Columns["btnEdit"].DisplayIndex = guna2DataGridView1.Columns.Count - 2;
                }

                if (guna2DataGridView1.Columns.Contains("btnDelete"))
                {
                    guna2DataGridView1.Columns["btnDelete"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    guna2DataGridView1.Columns["btnDelete"].DisplayIndex = guna2DataGridView1.Columns.Count - 1;
                }
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = guna2DataGridView1.Columns[e.ColumnIndex].Name;

            // Get ID safely
            if (guna2DataGridView1.Rows[e.RowIndex].Cells["Id"].Value == DBNull.Value) return;
            int orderId = Convert.ToInt32(guna2DataGridView1.Rows[e.RowIndex].Cells["Id"].Value);

            // EDIT LOGIC
            if (colName == "btnEdit")
            {
                AddPurchaseOrderForm editForm = new AddPurchaseOrderForm(orderId);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadPurchaseData();
                }
            }

            // DELETE LOGIC
            if (colName == "btnDelete")
            {
                string productName = "this item";
                if (guna2DataGridView1.Columns.Contains("ProductName"))
                {
                    productName = guna2DataGridView1.Rows[e.RowIndex].Cells["ProductName"].Value.ToString();
                }

                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to delete the order for '{productName}'?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (DatabaseHelper.DeletePurchaseOrder(orderId))
                    {
                        MessageBox.Show("Deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPurchaseData();
                    }
                }
            }
        }

        public void RefreshData()
        {
            LoadPurchaseData();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            AddPurchaseOrderForm addOrderForm = new AddPurchaseOrderForm();
            if (addOrderForm.ShowDialog() == DialogResult.OK)
            {
                LoadPurchaseData();
            }
        }

        // Unused events
        private void UCpurchase_Load(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e) { }
    }
}