using System;

namespace TrySystem
{
    // Custom EventArgs for product-related events
    public class ProductEventArgs : EventArgs
    {
        public string ProductName { get; }
        public string ProductCode { get; }
        public string Category { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public DateTime Timestamp { get; }

        public ProductEventArgs(string productName, string productCode, string category, decimal price, int quantity)
        {
            ProductName = productName;
            ProductCode = productCode;
            Category = category;
            Price = price;
            Quantity = quantity;
            Timestamp = DateTime.Now;
        }
    }

    // Custom delegate for product events
    public delegate void ProductEventHandler(object sender, ProductEventArgs e);
}










