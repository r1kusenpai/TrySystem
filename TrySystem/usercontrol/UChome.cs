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
            label31.Text = inventoryValue.ToString("N2");

            // Load total products
            int totalProducts = DatabaseHelper.GetTotalProducts();
            label30.Text = totalProducts.ToString();

            // Load total units/wrong
            //int totalUnits = DatabaseHelper.GetTotalUnits();
            //label5.Text = totalUnits.ToString();

            // Load low stock count
            int lowStockCount = DatabaseHelper.GetLowStockCount();
            label24.Text = lowStockCount.ToString();

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
