using System;
using System.Collections.Generic;
using System.Linq;
using LegacyInventory.Core.Models;
using LegacyInventory.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LegacyInventory.Tests
{
    [TestClass]
    public class InventoryServiceTests
    {
        private InventoryService _inventory;

        [TestInitialize]
        public void Setup()
        {
            _inventory = new InventoryService();
            _inventory.AddProduct(new Product("A-1", "Widget", 10m, 5));
            _inventory.AddProduct(new Product("A-2", "Gadget", 20m, 2));
        }

        [TestMethod]
        public void AddProduct_IncreasesCount()
        {
            _inventory.AddProduct(new Product("A-3", "Gizmo", 5m, 100));

            Assert.AreEqual(3, _inventory.Count);
        }

        [TestMethod]
        public void AddProduct_DuplicateSku_Throws()
        {
            Assert.ThrowsExactly<InvalidOperationException>(
                () => _inventory.AddProduct(new Product("A-1", "Duplicate", 1m, 1)));
        }

        [TestMethod]
        public void GetProduct_ReturnsCorrectProduct()
        {
            Product product = _inventory.GetProduct("A-2");

            Assert.AreEqual("Gadget", product.Name);
        }

        [TestMethod]
        public void GetProduct_IsCaseInsensitive()
        {
            Product product = _inventory.GetProduct("a-1");

            Assert.AreEqual("Widget", product.Name);
        }

        [TestMethod]
        public void GetProduct_Unknown_Throws()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(
                () => _inventory.GetProduct("does-not-exist"));
        }

        [TestMethod]
        public void Sell_ReducesQuantity()
        {
            _inventory.Sell("A-1", 3);

            Assert.AreEqual(2, _inventory.GetProduct("A-1").Quantity);
        }

        [TestMethod]
        public void Sell_MoreThanStock_Throws()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => _inventory.Sell("A-2", 10));
        }

        [TestMethod]
        public void Sell_NonPositiveAmount_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _inventory.Sell("A-1", 0));
        }

        [TestMethod]
        public void Restock_IncreasesQuantity()
        {
            _inventory.Restock("A-2", 8);

            Assert.AreEqual(10, _inventory.GetProduct("A-2").Quantity);
        }

        [TestMethod]
        public void GetTotalInventoryValue_SumsLineValues()
        {
            // A-1: 10 * 5 = 50, A-2: 20 * 2 = 40 => 90
            Assert.AreEqual(90m, _inventory.GetTotalInventoryValue());
        }

        [TestMethod]
        public void GetLowStockProducts_ReturnsBelowThreshold()
        {
            List<Product> lowStock = _inventory.GetLowStockProducts(4).ToList();

            Assert.AreEqual(1, lowStock.Count);
            Assert.AreEqual("A-2", lowStock[0].Sku);
        }

        [TestMethod]
        public void TryGetProduct_Missing_ReturnsFalse()
        {
            Product product;
            bool found = _inventory.TryGetProduct("nope", out product);

            Assert.IsFalse(found);
            Assert.IsNull(product);
        }
    }
}
