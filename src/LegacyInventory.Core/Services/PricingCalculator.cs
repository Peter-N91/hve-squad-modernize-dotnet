using System;

namespace LegacyInventory.Core.Services
{
    /// <summary>
    /// Calculates order pricing including quantity-based bulk discounts and tax.
    /// </summary>
    public class PricingCalculator
    {
        private readonly decimal _taxRate;

        public PricingCalculator(decimal taxRate)
        {
            if (taxRate < 0m || taxRate > 1m)
            {
                throw new ArgumentOutOfRangeException("taxRate", "Tax rate must be between 0 and 1.");
            }

            _taxRate = taxRate;
        }

        /// <summary>
        /// Returns the discount rate (0 - 0.15) that applies for the given quantity.
        /// </summary>
        public decimal GetDiscountRate(int quantity)
        {
            if (quantity < 0)
            {
                throw new ArgumentOutOfRangeException("quantity", "Quantity cannot be negative.");
            }

            if (quantity >= 100)
            {
                return 0.15m;
            }

            if (quantity >= 50)
            {
                return 0.10m;
            }

            if (quantity >= 10)
            {
                return 0.05m;
            }

            return 0m;
        }

        /// <summary>
        /// Calculates the total payable amount for a line, applying the bulk discount
        /// and then adding tax.
        /// </summary>
        public decimal CalculateLineTotal(decimal unitPrice, int quantity)
        {
            if (unitPrice < 0m)
            {
                throw new ArgumentOutOfRangeException("unitPrice", "Unit price cannot be negative.");
            }

            if (quantity < 0)
            {
                throw new ArgumentOutOfRangeException("quantity", "Quantity cannot be negative.");
            }

            decimal gross = unitPrice * quantity;
            decimal discount = gross * GetDiscountRate(quantity);
            decimal netAfterDiscount = gross - discount;
            decimal withTax = netAfterDiscount * (1m + _taxRate);

            return Math.Round(withTax, 2, MidpointRounding.AwayFromZero);
        }
    }
}
