using System;
using System.Data;
using System.Windows.Forms;

namespace TrySystem
{
    public partial class AddPurchaseOrderForm : Form
    {
        private ComboBox cmbProduct;
        private TextBox txtSupplierName;
        private TextBox txtQuantity;
        private TextBox txtPrice;
        private Label lblTotalAmount;
        private Button btnSave;
        private Button btnCancel;
        private int? purchaseOrderId = null;

        public AddPurchaseOrderForm()
        {
            InitializeComponent();
            LoadProducts();
        }

        public AddPurchaseOrderForm(int id) : this()
        {
            purchaseOrderId = id;
            this.Text = "Edit Purchase Order";
            LoadPurchaseOrderData();
        }

        private void LoadPurchaseOrderData()
        {
            try
            {
                DataRow orderData = DatabaseHelper.GetPurchaseOrderById(purchaseOrderId.Value);
                if (orderData != null)
                {
                    txtSupplierName.Text = orderData["SupplierName"]?.ToString() ?? "";
                    txtQuantity.Text = orderData["Quantity"]?.ToString() ?? "0";
                    txtPrice.Text = orderData["Price"]?.ToString() ?? "0";
                    
                    // Set product selection
                    if (orderData["ProductId"] != DBNull.Value)
                    {
                        int productId = Convert.ToInt32(orderData["ProductId"]);
                        foreach (DataRowView item in cmbProduct.Items)
                        {
                            if (Convert.ToInt32(item["Id"]) == productId)
                            {
                                cmbProduct.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading purchase order: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Add Purchase Order";
            this.Size = new System.Drawing.Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Supplier Name
            Label lblSupplier = new Label();
            lblSupplier.Text = "Supplier Name:";
            lblSupplier.Location = new System.Drawing.Point(20, 20);
            lblSupplier.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(lblSupplier);

            txtSupplierName = new TextBox();
            txtSupplierName.Location = new System.Drawing.Point(150, 18);
            txtSupplierName.Size = new System.Drawing.Size(300, 25);
            this.Controls.Add(txtSupplierName);

            // Product
            Label lblProduct = new Label();
            lblProduct.Text = "Product:";
            lblProduct.Location = new System.Drawing.Point(20, 55);
            lblProduct.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(lblProduct);

            cmbProduct = new ComboBox();
            cmbProduct.Location = new System.Drawing.Point(150, 53);
            cmbProduct.Size = new System.Drawing.Size(300, 25);
            cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(cmbProduct);

            // Quantity
            Label lblQuantity = new Label();
            lblQuantity.Text = "Quantity:";
            lblQuantity.Location = new System.Drawing.Point(20, 90);
            lblQuantity.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(lblQuantity);

            txtQuantity = new TextBox();
            txtQuantity.Location = new System.Drawing.Point(150, 88);
            txtQuantity.Size = new System.Drawing.Size(300, 25);
            txtQuantity.TextChanged += TxtQuantity_TextChanged;
            this.Controls.Add(txtQuantity);

            // Price
            Label lblPrice = new Label();
            lblPrice.Text = "Price per Unit:";
            lblPrice.Location = new System.Drawing.Point(20, 125);
            lblPrice.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(lblPrice);

            txtPrice = new TextBox();
            txtPrice.Location = new System.Drawing.Point(150, 123);
            txtPrice.Size = new System.Drawing.Size(300, 25);
            txtPrice.TextChanged += TxtPrice_TextChanged;
            this.Controls.Add(txtPrice);

            // Total Amount
            Label lblTotal = new Label();
            lblTotal.Text = "Total Amount:";
            lblTotal.Location = new System.Drawing.Point(20, 160);
            lblTotal.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(lblTotal);

            lblTotalAmount = new Label();
            lblTotalAmount.Text = "₱0.00";
            lblTotalAmount.Location = new System.Drawing.Point(150, 158);
            lblTotalAmount.Size = new System.Drawing.Size(300, 25);
            lblTotalAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.Controls.Add(lblTotalAmount);

            // Save Button
            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Location = new System.Drawing.Point(250, 210);
            btnSave.Size = new System.Drawing.Size(100, 35);
            btnSave.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            btnSave.ForeColor = System.Drawing.Color.White;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            // Cancel Button
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new System.Drawing.Point(360, 210);
            btnCancel.Size = new System.Drawing.Size(100, 35);
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadProducts()
        {
            try
            {
                DataTable products = DatabaseHelper.GetAllProducts();
                cmbProduct.DisplayMember = "ProductName";
                cmbProduct.ValueMember = "Id";
                cmbProduct.DataSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtQuantity_TextChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }

        private void TxtPrice_TextChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            try
            {
                if (int.TryParse(txtQuantity.Text, out int quantity) && 
                    decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    decimal total = quantity * price;
                    lblTotalAmount.Text = $"₱{total:N2}";
                }
                else
                {
                    lblTotalAmount.Text = "₱0.00";
                }
            }
            catch
            {
                lblTotalAmount.Text = "₱0.00";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // --- 1. VALIDATION ---
                if (string.IsNullOrWhiteSpace(txtSupplierName.Text))
                {
                    MessageBox.Show("Supplier name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbProduct.SelectedValue == null)
                {
                    MessageBox.Show("Please select a product.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Please enter a valid quantity (must be greater than 0).", "Quantity Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Please enter a valid price (must be greater than 0).", "Price Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // --- 2. PREPARE DATA ---
                int productId = Convert.ToInt32(cmbProduct.SelectedValue);
                string productName = cmbProduct.Text;
                string supplierName = txtSupplierName.Text.Trim();

                // Calculate the Total Amount (This fixes the NULL error)
                decimal totalAmount = quantity * price;

                bool success = false;

                // --- 3. SAVE TO DATABASE ---
                if (purchaseOrderId.HasValue)
                {
                    // UPDATE EXISTING ORDER
                    DataRow oldOrder = DatabaseHelper.GetPurchaseOrderById(purchaseOrderId.Value);
                    string currentStatus = oldOrder["Status"]?.ToString() ?? "Pending";

                    // Note: Ensure your DatabaseHelper.UpdatePurchaseOrder accepts 'totalAmount'
                    success = DatabaseHelper.UpdatePurchaseOrder(
                        purchaseOrderId.Value,
                        supplierName,
                        productId,
                        productName,
                        quantity,
                        price,
                        totalAmount,     // <--- Passing the calculated total
                        currentStatus
                    );

                    if (success)
                    {
                        MessageBox.Show("Purchase order updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    // ADD NEW ORDER
                    // Note: Ensure your DatabaseHelper.AddPurchaseOrder accepts 'totalAmount'
                    success = DatabaseHelper.AddPurchaseOrder(
                        supplierName,
                        productId,
                        productName,
                        quantity,
                        price,
                        totalAmount,     // <--- Passing the calculated total
                        "Pending"
                    );

                    if (success)
                    {
                        // --- 4. UPDATE INVENTORY STOCK ---
                        DataRow productData = DatabaseHelper.GetProductById(productId);
                        if (productData != null)
                        {
                            int currentQuantity = Convert.ToInt32(productData["Quantity"]);
                            int newQuantity = currentQuantity + quantity;
                            decimal productPrice = Convert.ToDecimal(productData["Price"]); // Keep original selling price

                            DatabaseHelper.UpdateProduct(
                                productId,
                                productData["ProductName"].ToString(),
                                productData["ProductCode"].ToString(),
                                productData["Category"].ToString(),
                                productData["Brand"].ToString(),
                                productPrice,
                                newQuantity
                            );
                        }

                        MessageBox.Show("Purchase order added successfully and inventory updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding purchase order: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}


