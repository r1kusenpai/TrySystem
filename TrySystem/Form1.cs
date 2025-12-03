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
using TrySystem.usercontrol;

namespace TrySystem
{
    
    public partial class Form1 : Form
    {
        private List<Guna.UI2.WinForms.Guna2Button> navButtons;
        
        private readonly string _currentUsername;

        public Form1() : this("User")
        {
        }

        public Form1(string username)
        {
            _currentUsername = string.IsNullOrWhiteSpace(username) ? "User" : username.Trim();
            InitializeComponent();
            EnhanceUI();
            SetupDataGridView();
            LoadDashboardData();
            UpdateUserGreeting();

            //new
            navButtons = new List<Guna.UI2.WinForms.Guna2Button>()
                {
                homebutton,
                inventorybutton,
                additembutton,
                purchasebutton,
                reportsbutton
                };

            SetActiveButton(homebutton);
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


        private void SetActiveButton(object sender)
        {
            // Cast the sender to a Guna2Button
            var clickedButton = sender as Guna.UI2.WinForms.Guna2Button;

            if (clickedButton != null)
            {
                // Iterate through all buttons and uncheck them
                foreach (var btn in navButtons)
                {
                    btn.Checked = false;
                }

                // Check only the button that was just clicked
                clickedButton.Checked = true;
            }
        }


        private void EnhanceUI()
        {
             
            
           

            // Enhance panel colors
            panelmenu.BackColor = Color.FromArgb(30, 41, 59);
            panel4.BackColor = Color.FromArgb(30, 41, 65);
            
            // Enhance logo
            logo.ForeColor = Color.White;
            logo.Font = new Font("Century Gothic", 14F, FontStyle.Bold);
        }

        private void LoadDashboardData()
        {
            //Load inventory value
            //decimal inventoryValue = DatabaseHelper.GetInventoryValue();
            //label4.Text = inventoryValue.ToString("N2");

            // Load total products
            int totalProducts = DatabaseHelper.GetTotalProducts();
            label11.Text = totalProducts.ToString();

            // Load total units
            decimal inventoryValue1 = DatabaseHelper.GetInventoryValue();
            label5.Text = inventoryValue1.ToString();

            // Load low stock count
            int lowStockCount = DatabaseHelper.GetLowStockCount();
            //label21.Text = lowStockCount.ToString(); $"{totalLowCount} items are below threshold.";
            label21.Text = $"{lowStockCount} items are below threshold.";

            // Load recent history
            LoadRecentHistory();
        }

        private void UpdateUserGreeting()
        {
            string displayName = string.IsNullOrWhiteSpace(_currentUsername) ? "User" : _currentUsername;
            if (label13 != null)
            {
                label13.Text = $"Welcome back, {displayName}! Here's what's happening with your warehouse.";
            }

            if (lblCurrentUser != null)
            {
                lblCurrentUser.Text = $"Logged in as: {displayName}";
            }
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
        private void addusercontrol(UserControl usercontrol)
        {
            usercontrol.Dock = DockStyle.Fill;
            panelcontainer.Controls.Clear();
            panelcontainer.Controls.Add(usercontrol);
            usercontrol.BringToFront();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            CollapseMenu();
        }

        private void CollapseMenu()
        {
           

        }


        public void TriggerAddItemTab()
        {
            additembutton_Click(null, null);
        }

        private void panel3_Paint(object sender, EventArgs e)
        {

        }

        private void lblCurrentUser_Click(object sender, EventArgs e)
        {

        }
        // Start of the change tab color upon clicking
        

        private void homebutton_Click(object sender, EventArgs e)
        {
            SetActiveButton(sender);
            UChome panel1 = new UChome();
            addusercontrol(panel1);
        }

        private void inventorybutton_Click(object sender, EventArgs e)
        {
            SetActiveButton(sender);
            UCinventory uCinventory = new UCinventory();
            addusercontrol(uCinventory);
        }

        private void additembutton_Click(object sender, EventArgs e)
        {
            SetActiveButton(sender);
            UCadditem uAdditem = new UCadditem();
            addusercontrol(uAdditem);
        }

        private void purchasebutton_Click(object sender, EventArgs e)
        {
            SetActiveButton(sender);
            UCpurchase uPurchase = new UCpurchase();
            addusercontrol(uPurchase);
        }

        private void reportsbutton_Click(object sender, EventArgs e)
        {
            SetActiveButton(sender);
            UCreports uReports = new UCreports();
            addusercontrol(uReports);
        }

        private void logout_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label21_Click(object sender, EventArgs e)
        {

        }
    }


}
