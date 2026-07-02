using System;
using LegacyInventory.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LegacyInventory.Tests
{
    [TestClass]
    public class PricingCalculatorTests
    {
        private PricingCalculator _calculator;

        [TestInitialize]
        public void Setup()
        {
            _calculator = new PricingCalculator(0.20m);
        }

        [TestMethod]
        public void GetDiscountRate_SmallQuantity_NoDiscount()
        {
            Assert.AreEqual(0m, _calculator.GetDiscountRate(9));
        }

        [TestMethod]
        public void GetDiscountRate_TenUnits_FivePercent()
        {
            Assert.AreEqual(0.05m, _calculator.GetDiscountRate(10));
        }

        [TestMethod]
        public void GetDiscountRate_FiftyUnits_TenPercent()
        {
            Assert.AreEqual(0.10m, _calculator.GetDiscountRate(50));
        }

        [TestMethod]
        public void GetDiscountRate_HundredUnits_FifteenPercent()
        {
            Assert.AreEqual(0.15m, _calculator.GetDiscountRate(100));
        }

        [TestMethod]
        public void CalculateLineTotal_NoDiscount_AddsTax()
        {
            // 5 units * 10 = 50, no discount, +20% tax = 60
            decimal total = _calculator.CalculateLineTotal(10m, 5);

            Assert.AreEqual(60.00m, total);
        }

        [TestMethod]
        public void CalculateLineTotal_WithBulkDiscount_AppliesDiscountThenTax()
        {
            // 10 units * 10 = 100, -5% = 95, +20% tax = 114
            decimal total = _calculator.CalculateLineTotal(10m, 10);

            Assert.AreEqual(114.00m, total);
        }

        [TestMethod]
        public void Constructor_InvalidTaxRate_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PricingCalculator(1.5m));
        }

        [TestMethod]
        public void CalculateLineTotal_NegativePrice_Throws()
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => _calculator.CalculateLineTotal(-1m, 5));
        }
    }
}
