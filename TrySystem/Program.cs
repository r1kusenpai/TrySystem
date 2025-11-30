
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrySystem
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                // Initialize database with exception handling
                try
                {
                    DatabaseHelper.InitializeDatabase();
                    
                    // Subscribe to custom events
                    DatabaseHelper.ProductAdded += OnProductAdded;
                    DatabaseHelper.LowStockAlert += OnLowStockAlert;
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"Database initialization failed: {sqlEx.Message}\nThe application may not function correctly.", 
                        "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Application initialization error: {ex.Message}", 
                        "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Show login form first
                Application.Run(new login());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fatal error: {ex.Message}\nApplication will now close.", 
                    "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event handler for custom ProductAdded event
        private static void OnProductAdded(object sender, ProductEventArgs e)
        {
            // Log or handle product addition event
            System.Diagnostics.Debug.WriteLine($"Product Added Event: {e.ProductName} ({e.ProductCode}) at {e.Timestamp}");
        }

        // Event handler for custom LowStockAlert event
        private static void OnLowStockAlert(object sender, ProductEventArgs e)
        {
            // Handle low stock alert event
            System.Diagnostics.Debug.WriteLine($"Low Stock Alert: {e.ProductName} has only {e.Quantity} units remaining!");
        }
    }
}
