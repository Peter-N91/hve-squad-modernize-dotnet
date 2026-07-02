using System;

namespace LegacyInventory.Core.Models
{
    /// <summary>
    /// Represents a single product held in inventory.
    /// </summary>
    public class Product
    {
        public Product(string sku, string name, decimal unitPrice, int quantity)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                throw new ArgumentException("SKU is required.", "sku");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is required.", "name");
            }

            if (unitPrice < 0m)
            {
                throw new ArgumentOutOfRangeException("unitPrice", "Unit price cannot be negative.");
            }

            if (quantity < 0)
            {
                throw new ArgumentOutOfRangeException("quantity", "Quantity cannot be negative.");
            }

            Sku = sku;
            Name = name;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        public string Sku { get; private set; }

        public string Name { get; private set; }

        public decimal UnitPrice { get; private set; }

        public int Quantity { get; set; }

        public decimal LineValue
        {
            get { return UnitPrice * Quantity; }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} (x{2}) @ {3:C}", Sku, Name, Quantity, UnitPrice);
        }
    }
}
