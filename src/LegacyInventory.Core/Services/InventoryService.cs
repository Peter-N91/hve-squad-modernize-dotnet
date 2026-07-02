using System;
using System.Collections.Generic;
using System.Linq;
using LegacyInventory.Core.Models;

namespace LegacyInventory.Core.Services
{
    /// <summary>
    /// In-memory inventory store providing basic stock management operations.
    /// </summary>
    public class InventoryService
    {
        private readonly Dictionary<string, Product> _products =
            new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<Product> Products
        {
            get { return _products.Values.OrderBy(p => p.Sku); }
        }

        public int Count
        {
            get { return _products.Count; }
        }

        public void AddProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException("product");
            }

            if (_products.ContainsKey(product.Sku))
            {
                throw new InvalidOperationException(
                    string.Format("A product with SKU '{0}' already exists.", product.Sku));
            }

            _products.Add(product.Sku, product);
        }

        public Product GetProduct(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                throw new ArgumentException("SKU is required.", "sku");
            }

            Product product;
            if (!_products.TryGetValue(sku, out product))
            {
                throw new KeyNotFoundException(
                    string.Format("No product found with SKU '{0}'.", sku));
            }

            return product;
        }

        public bool TryGetProduct(string sku, out Product product)
        {
            product = null;
            if (string.IsNullOrWhiteSpace(sku))
            {
                return false;
            }

            return _products.TryGetValue(sku, out product);
        }

        public void Restock(string sku, int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Restock amount must be positive.");
            }

            Product product = GetProduct(sku);
            product.Quantity += amount;
        }

        public void Sell(string sku, int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Sell amount must be positive.");
            }

            Product product = GetProduct(sku);
            if (product.Quantity < amount)
            {
                throw new InvalidOperationException(
                    string.Format("Not enough stock for '{0}'. In stock: {1}, requested: {2}.",
                        sku, product.Quantity, amount));
            }

            product.Quantity -= amount;
        }

        public decimal GetTotalInventoryValue()
        {
            return _products.Values.Sum(p => p.LineValue);
        }

        public IEnumerable<Product> GetLowStockProducts(int threshold)
        {
            if (threshold < 0)
            {
                throw new ArgumentOutOfRangeException("threshold", "Threshold cannot be negative.");
            }

            return _products.Values
                .Where(p => p.Quantity <= threshold)
                .OrderBy(p => p.Quantity)
                .ToList();
        }
    }
}
