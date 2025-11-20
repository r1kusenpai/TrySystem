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
        public Form1()
        {
            InitializeComponent();
            EnhanceUI();
        }
        
        private void EnhanceUI()
        {
            // Enhance menu button colors
            hometab.BackColor = Color.FromArgb(70, 130, 180);
            hometab.ForeColor = Color.White;
            hometab.FlatStyle = FlatStyle.Flat;
            hometab.FlatAppearance.BorderSize = 0;
            
            inventorytab.BackColor = Color.FromArgb(70, 130, 180);
            inventorytab.ForeColor = Color.White;
            inventorytab.FlatStyle = FlatStyle.Flat;
            inventorytab.FlatAppearance.BorderSize = 0;
            
            additemtab.BackColor = Color.FromArgb(70, 130, 180);
            additemtab.ForeColor = Color.White;
            additemtab.FlatStyle = FlatStyle.Flat;
            additemtab.FlatAppearance.BorderSize = 0;
            
            purchasetab.BackColor = Color.FromArgb(70, 130, 180);
            purchasetab.ForeColor = Color.White;
            purchasetab.FlatStyle = FlatStyle.Flat;
            purchasetab.FlatAppearance.BorderSize = 0;
            
            reporttab.BackColor = Color.FromArgb(70, 130, 180);
            reporttab.ForeColor = Color.White;
            reporttab.FlatStyle = FlatStyle.Flat;
            reporttab.FlatAppearance.BorderSize = 0;
            
            // Enhance panel colors
            panelmenu.BackColor = Color.FromArgb(41, 53, 65);
            panel4.BackColor = Color.FromArgb(31, 43, 55);
            
            // Enhance logo
            logo.ForeColor = Color.White;
            logo.Font = new Font("Century Gothic", 14F, FontStyle.Bold);
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

        private void hometab_Click(object sender, EventArgs e)
        {
            UChome panel1 = new UChome();
            addusercontrol(panel1);
        }

        private void inventorytab_Click(object sender, EventArgs e)
        {
            UCinventory uCinventory = new UCinventory();
            addusercontrol(uCinventory);
        }

        private void additemtab_Click(object sender, EventArgs e)
        {
            UCadditem uAdditem = new UCadditem();
            addusercontrol(uAdditem);
        }

        private void purchasetab_Click(object sender, EventArgs e)
        {
            UCpurchase uPurchase = new UCpurchase();    
            addusercontrol(uPurchase);
        }

        private void reporttab_Click(object sender, EventArgs e)
        {
            UCreports uReports = new UCreports();
            addusercontrol(uReports);
        }

        public void TriggerAddItemTab()
        {
            additemtab_Click(null, null);
        }

        private void panel3_Paint(object sender, EventArgs e)
        {

        }
    }
}
