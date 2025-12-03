using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TrySystem.usercontrol
{
    public partial class UCreports : UserControl
    {
        public UCreports()
        {
            InitializeComponent();
            SetupDataGridView();
            SetupChart();

            guna2DataGridView1.CellPainting += Guna2DataGridView1_CellPainting;

            LoadReports();
        }

        private void SetupDataGridView()
        {
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Force fill width
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.RowHeadersVisible = false;
            guna2DataGridView1.BorderStyle = BorderStyle.None;
            guna2DataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // --- 2. Header Style (The Fix for Clicking) ---
            guna2DataGridView1.EnableHeadersVisualStyles = false;
            guna2DataGridView1.ColumnHeadersHeight = 45;
            guna2DataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // Normal State
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252); // Light Gray
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);   // Dark Gray Text
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Selection State (FIX: Make it same as Normal so it doesn't change color)
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(71, 85, 105);

            // --- 3. Row Style ---
            guna2DataGridView1.ThemeStyle.RowsStyle.BackColor = Color.White;
            guna2DataGridView1.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(254, 226, 226); // Light Red Highlight
            guna2DataGridView1.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            guna2DataGridView1.RowTemplate.Height = 30;
        }

        private void SetupChart()
        {
            // Make the chart look modern and clean
            chart1.BackColor = Color.White;

            // If the chart has an area, style it
            if (chart1.ChartAreas.Count > 0)
            {
                chart1.ChartAreas[0].BackColor = Color.White;
                chart1.ChartAreas[0].BorderColor = Color.White;

                // Remove ugly grid lines
                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Gray;
                chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Gray;
            }

            // Set Palette to specific nice colors
            chart1.Palette = ChartColorPalette.None;
            chart1.PaletteCustomColors = new Color[] {
                Color.FromArgb(59, 130, 246), // Blue
                Color.FromArgb(16, 185, 129), // Green
                Color.FromArgb(245, 158, 11), // Orange
                Color.FromArgb(239, 68, 68),  // Red
                Color.FromArgb(139, 92, 246)  // Purple
            };

            // Clear dummy data
            chart1.Series.Clear();
            Series s = chart1.Series.Add("LowStock");
            s.ChartType = SeriesChartType.Doughnut; // Donut chart looks professional
            s.IsValueShownAsLabel = true;
            s["PieStartAngle"] = "270";
        }

        private void LoadReports()
        {
            try
            {
                // 1. Get Data
                DataTable lowStockData = DatabaseHelper.GetLowStockProducts();

                // 2. Add a Manual "Status" Column to the DataTable if it doesn't exist
                if (!lowStockData.Columns.Contains("StatusText"))
                {
                    lowStockData.Columns.Add("StatusText", typeof(string));
                }

                // 3. Calculate Status (Critical vs Low)
                foreach (DataRow row in lowStockData.Rows)
                {
                    int qty = Convert.ToInt32(row["Quantity"]);
                    if (qty <= 5) // You can change this threshold
                    {
                        row["StatusText"] = "CRITICAL";
                    }
                    else
                    {
                        row["StatusText"] = "LOW STOCK";
                    }
                }

                // 4. Bind Data
                guna2DataGridView1.DataSource = lowStockData;

                // 5. Configure Columns & Distribution
                if (guna2DataGridView1.Columns.Count > 0)
                {
                    // Hide IDs
                    if (guna2DataGridView1.Columns.Contains("Id")) guna2DataGridView1.Columns["Id"].Visible = false;
                    if (guna2DataGridView1.Columns.Contains("ProductId")) guna2DataGridView1.Columns["ProductId"].Visible = false;
                    if (guna2DataGridView1.Columns.Contains("ProductCode")) guna2DataGridView1.Columns["ProductCode"].Visible = false;
                    if (guna2DataGridView1.Columns.Contains("Brand")) guna2DataGridView1.Columns["Brand"].Visible = false;
                    if (guna2DataGridView1.Columns.Contains("SupplierId")) guna2DataGridView1.Columns["SupplierId"].Visible = false;

                    // --- A. Product Name (40% Width) ---
                    if (guna2DataGridView1.Columns.Contains("ProductName"))
                    {
                        guna2DataGridView1.Columns["ProductName"].HeaderText = "Product Name";
                        guna2DataGridView1.Columns["ProductName"].FillWeight = 20;
                    }

                    // --- B. Category (20% Width) ---
                    if (guna2DataGridView1.Columns.Contains("Category"))
                    {
                        guna2DataGridView1.Columns["Category"].HeaderText = "Category";
                        guna2DataGridView1.Columns["Category"].FillWeight = 20;
                    }

                    // --- C. Stock (10% Width) ---
                    if (guna2DataGridView1.Columns.Contains("Quantity"))
                    {
                        guna2DataGridView1.Columns["Quantity"].HeaderText = "Stock";
                        guna2DataGridView1.Columns["Quantity"].FillWeight = 20;
                        //guna2DataGridView1.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    // --- D. Price (15% Width) ---
                    if (guna2DataGridView1.Columns.Contains("Price"))
                    {
                        guna2DataGridView1.Columns["Price"].HeaderText = "Price";
                        guna2DataGridView1.Columns["Price"].FillWeight = 20;
                        guna2DataGridView1.Columns["Price"].DefaultCellStyle.Format = "C2";
                        //guna2DataGridView1.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }

                    // --- E. Status (15% Width) ---
                    if (guna2DataGridView1.Columns.Contains("StatusText"))
                    {
                        guna2DataGridView1.Columns["StatusText"].HeaderText = "Status";
                        guna2DataGridView1.Columns["StatusText"].FillWeight = 20;
                        guna2DataGridView1.Columns["StatusText"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        guna2DataGridView1.Columns["StatusText"].DefaultCellStyle.Font = new Font("Segoe UI", 8, FontStyle.Bold);

                        // Optional: Color Code the text
                        // (Note: Advanced row coloring requires the CellFormatting event, 
                        // but this sets the basic column up)
                    }
                }

                // --- Charts & Labels Logic (Keep as is) ---
                int totalLowCount = DatabaseHelper.GetLowStockCount();
                if (totallowitems != null) totallowitems.Text = totalLowCount.ToString();
                int criticalCount = lowStockData.AsEnumerable().Count(r => Convert.ToInt32(r["Quantity"]) <= 5);
                if (criticalitems != null) criticalitems.Text = criticalCount.ToString();

                // Alert Banner
                if (lowalert != null)
                {
                    if (totalLowCount > 0)
                    {
                        lowalert.Text = $"{totalLowCount} items are below threshold.";
                        lowalert.ForeColor = Color.FromArgb(185, 28, 28);
                    }
                    else
                    {
                        lowalert.Text = "✅ Inventory levels are healthy.";
                        lowalert.ForeColor = Color.FromArgb(21, 128, 61);
                    }
                }

                // Chart Logic
                if (chart1.Series.IndexOf("LowStock") != -1)
                {
                    DataTable categoryData = DatabaseHelper.GetLowStockByCategory();
                    chart1.Series["LowStock"].Points.Clear();
                    foreach (DataRow row in categoryData.Rows)
                    {
                        chart1.Series["LowStock"].Points.AddXY(row["Category"].ToString(), row["Count"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            // Load categories for filtering
            LoadCategories();

            // Load category breakdown with details
            LoadCategoryAlerts();
        }


        private void Guna2DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // 1. Check if we are in the "StatusText" column and not the header
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                guna2DataGridView1.Columns[e.ColumnIndex].Name == "StatusText")
            {
                // 2. Setup smooth drawing
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // 3. Clear the cell with White first (so we don't draw over old text)
                // Note: Using the selection color if selected, otherwise white
                using (Brush backColorBrush = new SolidBrush(e.CellStyle.BackColor))
                {
                    // If selected, use the selection color, else use white
                    Color bgColor = (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected
                                    ? e.CellStyle.SelectionBackColor
                                    : Color.White;

                    using (Brush b = new SolidBrush(bgColor))
                    {
                        e.Graphics.FillRectangle(b, e.CellBounds);
                    }
                }

                // 4. Determine Status String & Color
                string statusValue = e.Value?.ToString() ?? "";
                Color badgeColor = Color.Gray; // Default
                Color textColor = Color.White;

                if (statusValue == "CRITICAL")
                {
                    badgeColor = Color.FromArgb(220, 38, 38); // Red (#DC2626)
                }
                else if (statusValue == "LOW STOCK")
                {
                    badgeColor = Color.FromArgb(245, 158, 11); // Orange (#F59E0B)
                }

                // 5. Create the Badge Rectangle (Smaller than the cell)
                // Adjust these numbers to change the badge size
                int margin = 5;
                Rectangle rect = new Rectangle(e.CellBounds.X + margin,
                                               e.CellBounds.Y + margin + 2,
                                               e.CellBounds.Width - (2 * margin),
                                               e.CellBounds.Height - (2 * margin) - 4);

                // 6. Draw the Rounded Rectangle (The "Pill")
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 12; // Roundness
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                    path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                    path.CloseFigure();

                    using (Brush brush = new SolidBrush(badgeColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }

                // 7. Draw the Text (Centered)
                TextRenderer.DrawText(e.Graphics, statusValue,
                    new Font("Segoe UI", 8, FontStyle.Bold),
                    rect, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                // 8. Tell the grid "I handled the painting, don't draw the default stuff"
                e.Handled = true;
            }
        }


        private void FormatGridColumns()
        {
            if (guna2DataGridView1.Columns.Count > 0)
            {
                // Hide IDs
                if (guna2DataGridView1.Columns.Contains("Id")) guna2DataGridView1.Columns["Id"].Visible = false;
                if (guna2DataGridView1.Columns.Contains("ProductId")) guna2DataGridView1.Columns["ProductId"].Visible = false;

                // Format Money
                if (guna2DataGridView1.Columns.Contains("Price"))
                {
                    guna2DataGridView1.Columns["Price"].DefaultCellStyle.Format = "C2";
                    guna2DataGridView1.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                // Center Quantity
                if (guna2DataGridView1.Columns.Contains("Quantity"))
                {
                    guna2DataGridView1.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
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
