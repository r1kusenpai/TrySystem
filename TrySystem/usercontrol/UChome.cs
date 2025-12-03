using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace TrySystem.usercontrol
{
    public partial class UChome : UserControl
    {
       
        public UChome()
        {

           
            InitializeComponent();
            LoadDashboardData();
            SetupDataGridView();
           


        }
        private void SetupDataGridView()
        {
            recentorder.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Force fill width
            recentorder.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            recentorder.ReadOnly = true;
            recentorder.AllowUserToAddRows = false;
            recentorder.RowHeadersVisible = false;
            recentorder.BorderStyle = BorderStyle.None;
            recentorder.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // --- 2. Header Style (The Fix for Clicking) ---
            recentorder.EnableHeadersVisualStyles = false;
            recentorder.ColumnHeadersHeight = 45;
            recentorder.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // Normal State
            recentorder.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252); // Light Gray
            recentorder.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);   // Dark Gray Text
            recentorder.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Selection State (FIX: Make it same as Normal so it doesn't change color)
            recentorder.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);
            recentorder.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(71, 85, 105);

            // --- 3. Row Style ---
            recentorder.ThemeStyle.RowsStyle.BackColor = Color.White;
            recentorder.ThemeStyle.RowsStyle.ForeColor = Color.Black;
            recentorder.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(254, 226, 226); // Light Red Highlight
            recentorder.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
            recentorder.RowTemplate.Height = 30;
        }

        private void LoadDashboardData()
        {
            // Load inventory value
            decimal inventoryValue = DatabaseHelper.GetInventoryValue();
            label31.Text = inventoryValue.ToString("N2");

            // Load total products
            int totalProducts = DatabaseHelper.GetTotalProducts();
            label30.Text = totalProducts.ToString();

            // Load total units/wrong
            //int totalUnits = DatabaseHelper.GetTotalUnits();
            //label5.Text = totalUnits.ToString();

            // Load low stock count
            int lowStockCount = DatabaseHelper.GetLowStockCount();
            label24.Text = $"{lowStockCount} items are below threshold.";

            // Load recent history
            LoadRecentHistory();
        }

        private void LoadRecentHistory()
        {
            recentorder.DataSource = DatabaseHelper.GetHistory();
            if (recentorder.Columns.Count > 0)
            {
                recentorder.Columns["Id"].Visible = false;
                recentorder.Columns["ProductId"].Visible = false;
                if (recentorder.Columns["Price"] != null)
                {
                    recentorder.Columns["Price"].DefaultCellStyle.Format = "C2";
                }
            }
        }

        private void home_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelcontainer_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
