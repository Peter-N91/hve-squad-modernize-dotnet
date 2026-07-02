using System;
using System.Collections.Generic;
using LegacyInventory.Core.Models;
using LegacyInventory.Core.Services;

namespace LegacyInventory.ConsoleApp
{
    public static class Program
    {
        private static readonly PricingCalculator Pricing = new PricingCalculator(0.20m);

        public static void Main(string[] args)
        {
            InventoryService inventory = SeedInventory();

            Console.WriteLine("=====================================");
            Console.WriteLine("  Contoso Legacy Inventory Manager");
            Console.WriteLine("  (.NET 10)");
            Console.WriteLine("=====================================");
            Console.WriteLine();

            bool running = true;
            while (running)
            {
                PrintMenu();
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                if (choice == null)
                {
                    // No interactive input available (e.g. redirected/empty stdin) -> exit cleanly.
                    break;
                }

                Console.WriteLine();

                switch (choice.Trim())
                {
                    case "1":
                        ListProducts(inventory);
                        break;
                    case "2":
                        SellProduct(inventory);
                        break;
                    case "3":
                        RestockProduct(inventory);
                        break;
                    case "4":
                        ShowLowStock(inventory);
                        break;
                    case "5":
                        QuotePrice();
                        break;
                    case "0":
                    case "q":
                    case "Q":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Unknown option, please try again.");
                        break;
                }

                Console.WriteLine();
            }

            Console.WriteLine("Goodbye!");
        }

        private static InventoryService SeedInventory()
        {
            InventoryService inventory = new InventoryService();
            inventory.AddProduct(new Product("SKU-001", "Wireless Mouse", 24.99m, 120));
            inventory.AddProduct(new Product("SKU-002", "Mechanical Keyboard", 79.50m, 45));
            inventory.AddProduct(new Product("SKU-003", "USB-C Hub", 39.95m, 8));
            inventory.AddProduct(new Product("SKU-004", "1080p Webcam", 59.00m, 3));
            inventory.AddProduct(new Product("SKU-005", "Laptop Stand", 32.00m, 60));
            return inventory;
        }

        private static void PrintMenu()
        {
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("1) List products");
            Console.WriteLine("2) Sell stock");
            Console.WriteLine("3) Restock");
            Console.WriteLine("4) Low-stock report");
            Console.WriteLine("5) Price quote (with bulk discount + tax)");
            Console.WriteLine("0) Exit");
            Console.WriteLine("-------------------------------------");
        }

        private static void ListProducts(InventoryService inventory)
        {
            Console.WriteLine("Current inventory:");
            foreach (Product product in inventory.Products)
            {
                Console.WriteLine("  {0}  | line value: {1:C}", product, product.LineValue);
            }

            Console.WriteLine();
            Console.WriteLine("Total inventory value: {0:C}", inventory.GetTotalInventoryValue());
        }

        private static void SellProduct(InventoryService inventory)
        {
            string sku = Prompt("Enter SKU to sell");
            int amount = PromptInt("Quantity to sell");

            try
            {
                inventory.Sell(sku, amount);
                Product product = inventory.GetProduct(sku);
                Console.WriteLine("Sold {0} x {1}. Remaining stock: {2}.", amount, product.Name, product.Quantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }

        private static void RestockProduct(InventoryService inventory)
        {
            string sku = Prompt("Enter SKU to restock");
            int amount = PromptInt("Quantity to add");

            try
            {
                inventory.Restock(sku, amount);
                Product product = inventory.GetProduct(sku);
                Console.WriteLine("Restocked {0}. New stock level: {1}.", product.Name, product.Quantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }

        private static void ShowLowStock(InventoryService inventory)
        {
            const int threshold = 10;
            List<Product> lowStock = new List<Product>(inventory.GetLowStockProducts(threshold));

            if (lowStock.Count == 0)
            {
                Console.WriteLine("No products at or below {0} units.", threshold);
                return;
            }

            Console.WriteLine("Products at or below {0} units:", threshold);
            foreach (Product product in lowStock)
            {
                Console.WriteLine("  {0} - {1} units left", product.Sku, product.Quantity);
            }
        }

        private static void QuotePrice()
        {
            decimal unitPrice = PromptDecimal("Unit price");
            int quantity = PromptInt("Quantity");

            decimal discountRate = Pricing.GetDiscountRate(quantity);
            decimal total = Pricing.CalculateLineTotal(unitPrice, quantity);

            Console.WriteLine("Discount applied: {0:P0}", discountRate);
            Console.WriteLine("Total (incl. tax): {0:C}", total);
        }

        private static string Prompt(string label)
        {
            Console.Write(label + ": ");
            string value = Console.ReadLine();
            return value == null ? string.Empty : value.Trim();
        }

        private static int PromptInt(string label)
        {
            int value;
            while (!int.TryParse(Prompt(label), out value))
            {
                Console.WriteLine("Please enter a valid whole number.");
            }

            return value;
        }

        private static decimal PromptDecimal(string label)
        {
            decimal value;
            while (!decimal.TryParse(Prompt(label), out value))
            {
                Console.WriteLine("Please enter a valid amount.");
            }

            return value;
        }
    }
}
