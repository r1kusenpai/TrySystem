using System;
using System.Collections.Generic;
using System.Linq;

namespace TrySystem
{
    // Class demonstrating Generics and Collections
    public class ProductManager
    {
        // Generic List collection
        private List<Product> _products = new List<Product>();

        // Generic Dictionary for category grouping
        private Dictionary<string, List<Product>> _productsByCategory = new Dictionary<string, List<Product>>();

        // Generic method with constraints
        public void AddProduct<T>(T product) where T : Product
        {
            _products.Add(product);
            
            // Group by category using Dictionary
            if (!_productsByCategory.ContainsKey(product.Category))
            {
                _productsByCategory[product.Category] = new List<Product>();
            }
            _productsByCategory[product.Category].Add(product);
        }

        // Generic method returning IEnumerable
        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            if (_productsByCategory.ContainsKey(category))
            {
                return _productsByCategory[category].AsEnumerable();
            }
            return Enumerable.Empty<Product>();
        }

        // Using LINQ with generics
        public List<Product> GetLowStockProducts(int threshold = 10)
        {
            return _products.Where(p => p.Quantity <= threshold)
                          .OrderBy(p => p.Category)
                          .ThenBy(p => p.ProductName)
                          .ToList();
        }

        // Generic method with multiple type parameters
        public Dictionary<TKey, List<TValue>> GroupBy<TKey, TValue>(
            IEnumerable<TValue> items, 
            Func<TValue, TKey> keySelector) where TKey : class
        {
            return items.GroupBy(keySelector)
                       .ToDictionary(g => g.Key, g => g.ToList());
        }

        public int GetTotalProducts()
        {
            return _products.Count;
        }

        public Dictionary<string, int> GetCategoryCounts()
        {
            return _productsByCategory.ToDictionary(
                kvp => kvp.Key, 
                kvp => kvp.Value.Count
            );
        }
    }

    // Simple Product class for demonstration
    public class Product
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}





