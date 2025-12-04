using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TrySystem
{
    public class DatabaseHelper
    {
        // Custom event for product operations
        public static event ProductEventHandler ProductAdded;
        public static event ProductEventHandler LowStockAlert;

        // Single, centralized connection string for the entire application.
        // To change the database, update ONLY this value.
        private static readonly string connectionString =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TrySystemDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";

        /// <summary>
        /// Creates and returns a new <see cref="SqlConnection"/> using the global connection string.
        /// This is the ONLY method that should be used to open database connections.
        /// </summary>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static void InitializeDatabase()
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();

                string createProductsTable = @"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Products' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Products (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProductName NVARCHAR(200) NOT NULL,
        ProductCode NVARCHAR(100) NOT NULL UNIQUE,
        Category NVARCHAR(100) NOT NULL,
        Brand NVARCHAR(100) NULL,
        Price DECIMAL(18,2) NOT NULL,
        Quantity INT NOT NULL DEFAULT 0,
        LowStockThreshold INT NOT NULL DEFAULT 10,
        DateAdded DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    );
END";

                string createHistoryTable = @"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'History' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.History (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProductId INT NULL,
        ProductName NVARCHAR(200),
        Action NVARCHAR(50) NOT NULL,
        Quantity INT NULL,
        Price DECIMAL(18,2) NULL,
        Category NVARCHAR(100),
        DateCreated DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        CONSTRAINT FK_History_Product FOREIGN KEY(ProductId) REFERENCES dbo.Products(Id)
    );
END";

                string createPurchaseTable = @"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PurchaseOrders' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.PurchaseOrders (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SupplierName NVARCHAR(200),
        ProductId INT NULL,
        ProductName NVARCHAR(200),
        Quantity INT NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        OrderDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        CONSTRAINT FK_PurchaseOrders_Product FOREIGN KEY(ProductId) REFERENCES dbo.Products(Id)
    );
END";

                using (SqlCommand cmd = new SqlCommand(createProductsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(createHistoryTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(createPurchaseTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool AddProduct(string name, string code, string category, string brand, decimal price, int quantity)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Products (ProductName, ProductCode, Category, Brand, Price, Quantity) 
                                   VALUES (@name, @code, @category, @brand, @price, @quantity)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(brand) ? (object)DBNull.Value : brand);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.ExecuteNonQuery();
                    }

                    int productId = GetLastInsertedId(conn);
                    AddHistory(productId, name, "Added", quantity, price, category);

                    ProductAdded?.Invoke(null, new ProductEventArgs(name, code, category, price, quantity));

                    if (quantity <= 10)
                    {
                        LowStockAlert?.Invoke(null, new ProductEventArgs(name, code, category, price, quantity));
                    }
                }
                return true;
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static DataTable GetAllProducts()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Id, ProductName, ProductCode, Category, Brand, Price, Quantity FROM Products ORDER BY ProductName";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error loading products: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public static DataTable SearchProducts(string searchTerm)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT Id, ProductName, ProductCode, Category, Brand, Price, Quantity 
                                   FROM Products 
                                   WHERE ProductName LIKE @search OR ProductCode LIKE @search OR Category LIKE @search OR Brand LIKE @search
                                   ORDER BY ProductName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@search", $"%{searchTerm}%");
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public static DataTable GetLowStockProducts()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT Id, ProductName, ProductCode, Category, Brand, Price, Quantity 
                                   FROM Products 
                                   WHERE Quantity <= LowStockThreshold
                                   ORDER BY Category, ProductName";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading low stock products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public static DataTable GetLowStockByCategory()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT Category, COUNT(*) as Count, SUM(Quantity) as TotalQuantity
                                   FROM Products 
                                   WHERE Quantity <= LowStockThreshold
                                   GROUP BY Category
                                   ORDER BY Category";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading low stock by category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public static DataTable GetHistory()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT TOP 100 * FROM History ORDER BY DateCreated DESC";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public static void AddHistory(int productId, string productName, string action, int quantity, decimal price, string category)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO History (ProductId, ProductName, Action, Quantity, Price, Category) 
                                   VALUES (@productId, @productName, @action, @quantity, @price, @category)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@productId", productId > 0 ? (object)productId : DBNull.Value);
                        cmd.Parameters.AddWithValue("@productName", (object)productName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@action", action);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@category", (object)category ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding history: {ex.Message}");
            }
        }

        public static int GetLowStockCount()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Products WHERE Quantity <= LowStockThreshold";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public static decimal GetInventoryValue()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT SUM(Price * Quantity) FROM Products";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public static int GetTotalProducts()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Products";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public static int GetTotalUnits()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT SUM(Quantity) FROM Products";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        return result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public static bool UpdateProduct(int id, string name, string code, string category, string brand, decimal price, int quantity)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Products 
                                   SET ProductName = @name, ProductCode = @code, Category = @category, 
                                       Brand = @brand, Price = @price, Quantity = @quantity
                                   WHERE Id = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.Parameters.AddWithValue("@brand", string.IsNullOrWhiteSpace(brand) ? (object)DBNull.Value : brand);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.ExecuteNonQuery();
                    }

                    AddHistory(id, name, "Updated", quantity, price, category);

                    ProductAdded?.Invoke(null, new ProductEventArgs(name, code, category, price, quantity));

                    if (quantity <= 10)
                    {
                        LowStockAlert?.Invoke(null, new ProductEventArgs(name, code, category, price, quantity));
                    }
                }
                return true;
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool DeleteProduct(int id)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();

                    string selectQuery = "SELECT ProductName, ProductCode, Category, Price, Quantity FROM Products WHERE Id = @id";
                    string productName = "";
                    string productCode = "";
                    string category = "";
                    decimal price = 0;
                    int quantity = 0;

                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                productName = reader["ProductName"].ToString();
                                productCode = reader["ProductCode"].ToString();
                                category = reader["Category"].ToString();
                                price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0;
                                quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0;
                            }
                        }
                    }

                    // Delete related history records first to avoid foreign key constraint
                    string deleteHistoryQuery = "DELETE FROM History WHERE ProductId = @id";
                    using (SqlCommand cmd = new SqlCommand(deleteHistoryQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // Set ProductId to NULL in PurchaseOrders to avoid foreign key constraint
                    string updatePurchaseQuery = "UPDATE PurchaseOrders SET ProductId = NULL WHERE ProductId = @id";
                    using (SqlCommand cmd = new SqlCommand(updatePurchaseQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // Now delete the product
                    string deleteQuery = "DELETE FROM Products WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    AddHistory(0, productName, "Deleted", quantity, price, category);
                }
                return true;
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static DataRow GetProductById(int id)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Products WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                return dt.Rows[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        public static DataTable GetPurchaseOrderHistory()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT Id, SupplierName, ProductName, Quantity, Price, TotalAmount, OrderDate, Status 
                                   FROM PurchaseOrders 
                                   ORDER BY OrderDate DESC";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error loading purchase orders: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading purchase orders: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public static int GetTotalPurchaseOrders()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM PurchaseOrders";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public static decimal GetTotalPurchaseValue()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT SUM(TotalAmount) FROM PurchaseOrders";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public static int GetActiveSuppliers()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(DISTINCT SupplierName) FROM PurchaseOrders WHERE SupplierName IS NOT NULL AND LTRIM(RTRIM(SupplierName)) <> ''";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        return result != DBNull.Value ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public static int GetOngoingOrders()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM PurchaseOrders WHERE Status IN ('Pending', 'Processing')";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public static bool AddPurchaseOrder(string supplierName, int productId, string productName, int quantity, decimal price, string status = "Pending")
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO PurchaseOrders (SupplierName, ProductId, ProductName, Quantity, Price, Status) 
                                   VALUES (@supplierName, @productId, @productName, @quantity, @price, @status)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@supplierName", string.IsNullOrWhiteSpace(supplierName) ? (object)DBNull.Value : supplierName);
                        cmd.Parameters.AddWithValue("@productId", productId);
                        cmd.Parameters.AddWithValue("@productName", productName);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding purchase order: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool UpdatePurchaseOrder(int id, string supplierName, int productId, string productName, int quantity, decimal price, string status)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE PurchaseOrders 
                                   SET SupplierName = @supplierName, ProductId = @productId, ProductName = @productName, 
                                       Quantity = @quantity, Price = @price, Status = @status
                                   WHERE Id = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@supplierName", string.IsNullOrWhiteSpace(supplierName) ? (object)DBNull.Value : supplierName);
                        cmd.Parameters.AddWithValue("@productId", productId);
                        cmd.Parameters.AddWithValue("@productName", productName);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating purchase order: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool DeletePurchaseOrder(int id)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM PurchaseOrders WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting purchase order: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static DataRow GetPurchaseOrderById(int id)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM PurchaseOrders WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                return dt.Rows[0];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading purchase order: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        private static int GetLastInsertedId(SqlConnection conn)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT CAST(SCOPE_IDENTITY() AS INT);", conn))
            {
                object result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }
    }
}

