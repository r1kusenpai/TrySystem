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
    public partial class UChome : UserControl
    {
        public UChome()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            // Load inventory value
            decimal inventoryValue = DatabaseHelper.GetInventoryValue();
            label4.Text = inventoryValue.ToString("N2");

            // Load total products
            int totalProducts = DatabaseHelper.GetTotalProducts();
            label11.Text = totalProducts.ToString();

            // Load total units
            int totalUnits = DatabaseHelper.GetTotalUnits();
            label5.Text = totalUnits.ToString();

            // Load low stock count
            int lowStockCount = DatabaseHelper.GetLowStockCount();
            label21.Text = lowStockCount.ToString();

            // Load recent history
            LoadRecentHistory();
        }

        private void LoadRecentHistory()
        {
            dataGridView1.DataSource = DatabaseHelper.GetHistory();
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["ProductId"].Visible = false;
                if (dataGridView1.Columns["Price"] != null)
                {
                    dataGridView1.Columns["Price"].DefaultCellStyle.Format = "C2";
                }
            }
        }
    }
}
