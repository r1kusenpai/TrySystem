using System;
using System.Data;
using System.Windows.Forms;

namespace TrySystem
{
    public partial class EditProductForm : Form
    {
        private int productId;
        private TextBox txtProductName;
        private TextBox txtProductCode;
        private TextBox txtCategory;
        private TextBox txtBrand;
        private TextBox txtPrice;
        private TextBox txtQuantity;
        private Button btnSave;
        private Button btnCancel;

        public EditProductForm(int id, DataRow productData)
        {
            productId = id;
            InitializeComponent();
            LoadProductData(productData);
        }

        private void InitializeComponent()
        {
            this.Text = "Edit Product";
            this.Size = new System.Drawing.Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Product Name
            Label lblName = new Label();
            lblName.Text = "Product Name:";
            lblName.Location = new System.Drawing.Point(20, 20);
            lblName.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(lblName);

            txtProductName = new TextBox();
            txtProductName.Location = new System.Drawing.Point(150, 18);
            txtProductName.Size = new System.Drawing.Size(300, 25);
            this.Controls.Add(txtProductName);

            // Product Code
            Label lblCode = new Label();
            lblCode.Text = "Product Code:";
            lblCode.Location = new System.Drawing.Point(20, 55);
            lblCode.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(lblCode);

            txtProductCode = new TextBox();
            txtProductCode.Location = new System.Drawing.Point(150, 53);
            txtProductCode.Size = new System.Drawing.Size(300, 25);
            this.Controls.Add(txtProductCode);

            // Category
            Label lblCategory = new Label();
            lblCategory.Text = "Category:";
            lblCategory.Location = new System.Drawing.Point(20, 90);
            lblCategory.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(lblCategory);

            txtCategory = new TextBox();
            txtCategory.Location = new System.Drawing.Point(150, 88);
            txtCategory.Size = new System.Drawing.Size(300, 25);
            this.Controls.Add(txtCategory);

            // Brand
            Label lblBrand = new Label();
            lblBrand.Text = "Brand:";
            lblBrand.Location = new System.Drawing.Point(20, 125);
            lblBrand.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(lblBrand);

            txtBrand = new TextBox();
            txtBrand.Location = new System.Drawing.Point(150, 123);
            txtBrand.Size = new System.Drawing.Size(300, 25);
            this.Controls.Add(txtBrand);

            // Price
            Label lblPrice = new Label();
            lblPrice.Text = "Price:";
            lblPrice.Location = new System.Drawing.Point(20, 160);
            lblPrice.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(lblPrice);

            txtPrice = new TextBox();
            txtPrice.Location = new System.Drawing.Point(150, 158);
            txtPrice.Size = new System.Drawing.Size(300, 25);
            this.Controls.Add(txtPrice);

            // Quantity
            Label lblQuantity = new Label();
            lblQuantity.Text = "Quantity:";
            lblQuantity.Location = new System.Drawing.Point(20, 195);
            lblQuantity.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(lblQuantity);

            txtQuantity = new TextBox();
            txtQuantity.Location = new System.Drawing.Point(150, 193);
            txtQuantity.Size = new System.Drawing.Size(300, 25);
            this.Controls.Add(txtQuantity);

            // Save Button
            btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Location = new System.Drawing.Point(250, 240);
            btnSave.Size = new System.Drawing.Size(100, 35);
            btnSave.BackColor = System.Drawing.Color.FromArgb(70, 130, 180);
            btnSave.ForeColor = System.Drawing.Color.White;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            // Cancel Button
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new System.Drawing.Point(360, 240);
            btnCancel.Size = new System.Drawing.Size(100, 35);
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadProductData(DataRow productData)
        {
            txtProductName.Text = productData["ProductName"].ToString();
            txtProductCode.Text = productData["ProductCode"].ToString();
            txtCategory.Text = productData["Category"].ToString();
            txtBrand.Text = productData["Brand"].ToString();
            txtPrice.Text = productData["Price"].ToString();
            txtQuantity.Text = productData["Quantity"].ToString();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(txtProductName.Text))
                {
                    MessageBox.Show("Product name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtProductCode.Text))
                {
                    MessageBox.Show("Product code is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCategory.Text))
                {
                    MessageBox.Show("Category is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate and parse price
                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
                {
                    MessageBox.Show("Please enter a valid price (must be a positive number).", "Price Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate and parse quantity
                if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("Please enter a valid quantity (must be a positive number).", "Quantity Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update product
                bool success = DatabaseHelper.UpdateProduct(
                    productId,
                    txtProductName.Text.Trim(),
                    txtProductCode.Text.Trim(),
                    txtCategory.Text.Trim(),
                    txtBrand.Text.Trim(),
                    price,
                    quantity
                );

                if (success)
                {
                    MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}





