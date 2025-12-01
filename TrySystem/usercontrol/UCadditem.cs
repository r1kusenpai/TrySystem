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
    public partial class UCadditem : UserControl
    {
        public UCadditem()
        {
            InitializeComponent();
            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    throw new ArgumentException("Product name is required.", nameof(textBox2));
                }

                if (string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    throw new ArgumentException("Product code is required.", nameof(textBox3));
                }

                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    throw new ArgumentException("Category is required.", nameof(textBox1));
                }

                // Validate and parse price with specific exception handling
                decimal price = 0;
                if (!decimal.TryParse(textBox5.Text, out price))
                {
                    throw new FormatException("Price must be a valid decimal number.");
                }
                if (price < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(textBox5), "Price cannot be negative.");
                }

                // Validate and parse quantity with specific exception handling
                int quantity = 0;
                if (!int.TryParse(textBox6.Text, out quantity))
                {
                    throw new FormatException("Quantity must be a valid integer number.");
                }
                if (quantity < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(textBox6), "Quantity cannot be negative.");
                }

                // Add product to database
                bool success = DatabaseHelper.AddProduct(
                    textBox2.Text.Trim(),
                    textBox3.Text.Trim(),
                    textBox1.Text.Trim(),
                    textBox4.Text.Trim(),
                    price,
                    quantity
                );

                if (success)
                {
                    MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Clear form
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox1.Clear();
                    textBox4.Clear();
                    textBox5.Clear();
                    textBox6.Clear();

                    // Refresh inventory if it's open
                    RefreshInventory();
                }
            }
            catch (ArgumentOutOfRangeException rangeEx)
            {
                // Must catch before ArgumentException since it's a subclass
                MessageBox.Show(rangeEx.Message, "Range Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (ArgumentException argEx)
            {
                // Catches ArgumentException and other argument-related exceptions
                MessageBox.Show(argEx.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (FormatException formatEx)
            {
                MessageBox.Show(formatEx.Message, "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidOperationException opEx)
            {
                MessageBox.Show(opEx.Message, "Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshInventory()
        {
            // Find parent form and refresh inventory if it exists
            Form1 parentForm = this.ParentForm as Form1;
            if (parentForm != null)
            {
                // Check if inventory control is currently displayed
                foreach (Control control in parentForm.panelcontainer.Controls)
                {
                    if (control is UCinventory)
                    {
                        ((UCinventory)control).LoadInventory();
                        break;
                    }
                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
