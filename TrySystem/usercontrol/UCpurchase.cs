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
            LoadPurchaseData();
            SetupDataGridView();
            button1.Click += Button1_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void LoadPurchaseData()
        {
            try
            {
                // Load total purchase orders
                int totalOrders = DatabaseHelper.GetTotalPurchaseOrders();
                label1.Text = totalOrders.ToString();

                // Load total purchase value
                decimal totalValue = DatabaseHelper.GetTotalPurchaseValue();
                label11.Text = totalValue.ToString("N2");

                // Load active suppliers
                int activeSuppliers = DatabaseHelper.GetActiveSuppliers();
                label15.Text = activeSuppliers.ToString();

                // Load ongoing orders
                //int ongoingOrders = DatabaseHelper.GetOngoingOrders();
                //label2.Text = ongoingOrders.ToString();

                // Load purchase order history
                LoadPurchaseHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading purchase data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPurchaseHistory()
        {
            dataGridView1.DataSource = DatabaseHelper.GetPurchaseOrderHistory();
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["Id"].Visible = false;
                if (dataGridView1.Columns["Price"] != null)
                {
                    dataGridView1.Columns["Price"].DefaultCellStyle.Format = "C2";
                }
                if (dataGridView1.Columns["TotalAmount"] != null)
                {
                    dataGridView1.Columns["TotalAmount"].DefaultCellStyle.Format = "C2";
                }
                if (dataGridView1.Columns["OrderDate"] != null)
                {
                    dataGridView1.Columns["OrderDate"].DefaultCellStyle.Format = "MM/dd/yyyy HH:mm";
                }
            }
        }

        public void RefreshData()
        {
            LoadPurchaseData();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            AddPurchaseOrderForm addOrderForm = new AddPurchaseOrderForm();
            if (addOrderForm.ShowDialog() == DialogResult.OK)
            {
                LoadPurchaseData(); // Refresh the purchase data
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a purchase order to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            DataRow row = null;
            
            if (selectedRow.DataBoundItem is DataRowView rowView)
            {
                row = rowView.Row;
            }
            else if (dataGridView1.DataSource is DataTable dt && selectedRow.Index < dt.Rows.Count)
            {
                row = dt.Rows[selectedRow.Index];
            }
            
            if (row != null && row["Id"] != DBNull.Value)
            {
                int orderId = Convert.ToInt32(row["Id"]);
                
                AddPurchaseOrderForm editForm = new AddPurchaseOrderForm(orderId);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadPurchaseData(); // Refresh the purchase data
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a purchase order to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            DataRow row = null;
            
            if (selectedRow.DataBoundItem is DataRowView rowView)
            {
                row = rowView.Row;
            }
            else if (dataGridView1.DataSource is DataTable dt && selectedRow.Index < dt.Rows.Count)
            {
                row = dt.Rows[selectedRow.Index];
            }
            
            if (row != null && row["Id"] != DBNull.Value)
            {
                int orderId = Convert.ToInt32(row["Id"]);
                string productName = row["ProductName"]?.ToString() ?? "";

                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to delete the purchase order for '{productName}'?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (DatabaseHelper.DeletePurchaseOrder(orderId))
                    {
                        MessageBox.Show("Purchase order deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPurchaseData(); // Refresh the purchase data
                    }
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
