using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JazzPizzazzApp.Models;

namespace JazzPizzazzApp.Services
{
    public class OrderService : IOrderService
    {
        private List<Order> _orders;

        public OrderService()
        {
            _orders = new List<Order>();
            _orders = GetAllOrders();
        }

        public List<Order> GetAllOrders()
        {
            if (!File.Exists("orders.txt"))
            {
                return new List<Order>();
            }

            _orders.Clear();

            using (StreamReader inFile = new StreamReader("orders.txt"))
            {
                string line;
                while ((line = inFile.ReadLine()) != null)
                {
                    string[] temp = line.Split('#');

                    var order = new Order(
                        int.Parse(temp[0]), // OrderID
                        temp[1],            // Email
                        temp[2],            // Pizza
                        temp[3],            // Date (as string)
                        int.Parse(temp[4]), // Size
                        temp[5],            // Crust
                        temp[6]             // Status
                    );

                    _orders.Add(order);
                }
            }

            return _orders;
        }

        public void SaveAllOrders(List<Order> orders)
        {
            using (StreamWriter outFile = new StreamWriter("orders.txt"))
            {
                foreach (var order in orders)
                {
                    outFile.WriteLine(order.ToFile());
                }
            }
        }

        public void AddOrder(Order order)
        {
            _orders.Add(order);
            SaveOrdersToFile();
        }

        public Order GetOrderByID(int id)
        {
            return _orders.FirstOrDefault(order => order.OrderID == id);
        }

        public void UpdateOrder(Order updatedOrder)
        {
            var order = _orders.FirstOrDefault(o => o.OrderID == updatedOrder.OrderID);
            if (order != null)
            {
                order.SetStatus(updatedOrder.GetStatus());
                SaveOrdersToFile();
            }
        }

        private void SaveOrdersToFile()
        {
            using (StreamWriter outFile = new StreamWriter("orders.txt"))
            {
                foreach (var order in _orders)
                {
                    outFile.WriteLine(order.ToFile());
                }
            }
        }

        public List<Order> GetOrdersByDate(string date)
        {
            return _orders.Where(order => order.GetDate() == date).ToList();
        }

        public void PrintAllOrders()
        {
            foreach (var order in _orders)
            {
                Console.WriteLine(order.ToString());
            }
        }

        public void DailyReport(string date)
        {
            var ordersForDate = GetOrdersByDate(date);
            Console.WriteLine($"Orders for {date}:");
            foreach (var order in ordersForDate)
            {
                Console.WriteLine(order.ToString());
            }
        }

        public void AverageSizeByCrust()
        {
            var averageSizes = _orders
                .GroupBy(o => o.GetCrust())
                .Select(group => new
                {
                    CrustType = group.Key,
                    AverageSize = group.Average(o => o.GetSize())
                });

            foreach (var item in averageSizes)
            {
                Console.WriteLine($"Crust: {item.CrustType}, Average Size: {item.AverageSize}");
            }
        }

        public void OrdersInProgress()
        {
            var inProgressOrders = _orders.Where(o => o.GetStatus() == "In Progress").ToList();
            Console.WriteLine("Orders in progress:");
            foreach (var order in inProgressOrders)
            {
                Console.WriteLine(order.ToString());
            }
        }

        public List<KeyValuePair<string, int>> GetTop5Pizzas()
        {
            var pizzaOrderCount = new Dictionary<string, int>();

            foreach (var order in _orders)
            {
                if (pizzaOrderCount.ContainsKey(order.Pizza))
                {
                    pizzaOrderCount[order.Pizza]++;
                }
                else
                {
                    pizzaOrderCount[order.Pizza] = 1;
                }
            }

            return pizzaOrderCount
                .OrderByDescending(p => p.Value)
                .Take(5)
                .ToList();
        }

        public Dictionary<string, int> GetCrustPopularity()
        {
            Dictionary<string, int> crustCounts = new Dictionary<string, int>();

            foreach (Order order in _orders)
            {
                string crust = order.GetCrust();

                if (!string.IsNullOrWhiteSpace(crust))
                {
                    if (!crustCounts.ContainsKey(crust))
                    {
                        crustCounts[crust] = 0;
                    }

                    crustCounts[crust]++;
                }
            }

            return crustCounts;
        }

        public void ChangeOrderStatus(int orderID)
        {
            var order = _orders.FirstOrDefault(o => o.OrderID == orderID);
            if (order != null)
            {
                order.Status = order.Status == "In Progress" ? "Completed" : "In Progress";
                UpdateOrder(order);
            }
            else
            {
                Console.WriteLine("Order not found.");
            }
        }

        public void SortOrders(ref List<Order> orders, string sortBy)
        {
            for (int i = 0; i < orders.Count - 1; i++)
            {
                int min = i;
                for (int j = i + 1; j < orders.Count; j++)
                {
                    if (CompareOrders(orders[min], orders[j], sortBy) > 0)
                    {
                        min = j;
                    }
                }

                if (min != i)
                {
                    Swap(ref orders, min, i);
                }
            }
        }

        private int CompareOrders(Order order1, Order order2, string sortBy)
        {
            switch (sortBy.ToLower())
            {
                case "orderid":
                    return order1.OrderID.CompareTo(order2.OrderID);
                case "email":
                    return string.Compare(order1.Email, order2.Email, StringComparison.OrdinalIgnoreCase);
                case "pizza":
                    return string.Compare(order1.Pizza, order2.Pizza, StringComparison.OrdinalIgnoreCase);
                case "date":
                    return string.Compare(order1.Date, order2.Date, StringComparison.OrdinalIgnoreCase); // Compare as string
                case "size":
                    return order1.Size.CompareTo(order2.Size);
                case "crust":
                    return string.Compare(order1.Crust, order2.Crust, StringComparison.OrdinalIgnoreCase);
                case "status":
                    return string.Compare(order1.Status, order2.Status, StringComparison.OrdinalIgnoreCase);
                default:
                    return 0;
            }
        }

        private void Swap(ref List<Order> orders, int i, int j)
        {
            var temp = orders[i];
            orders[i] = orders[j];
            orders[j] = temp;
        }

        public decimal GetTotalAmountEarned()
        {
            var orders = GetAllOrders();

            // Assuming each order has a property `Price` or you calculate based on Size
            decimal total = 0;

            foreach (var order in orders)
            {
                // You can also use a pricing function if no Price property
                decimal price = CalculatePrice(order.Size);
                total += price;
            }

            return total;
        }

        // Example helper method if prices depend on size
        private decimal CalculatePrice(int size)
        {
            return size switch
            {
                8 => 8.99m,  // Small
                12 => 10.99m, // Medium
                16 => 12.99m, // Large
                _ => 0m  // Default if size doesn't match
            };
        }

      public Dictionary<string, decimal> GetRevenueByPizzaName(int? month = null, int? year = null)
{
    return _orders
        .Where(o =>
        {
            if (DateTime.TryParseExact(o.Date, "MM/dd/yy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                bool monthMatches = !month.HasValue || parsedDate.Month == month.Value;
                bool yearMatches = !year.HasValue || parsedDate.Year == year.Value;
                return monthMatches && yearMatches;
            }
            return false; // Skip orders with invalid date formats
        })
        .GroupBy(o => o.Pizza)
        .Select(g =>
        {
            string pizzaName = g.Key;
            decimal revenue = g.Sum(o =>
            {
                int size = o.Size;
                return CalculatePrice(size);
            });

            return new { pizzaName, revenue };
        })
        .ToDictionary(x => x.pizzaName, x => x.revenue);
}

public List<Order> GetOrdersByEmail(string email)
{
    return _orders
        .Where(o => o.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
        .ToList();
}




    }
}
