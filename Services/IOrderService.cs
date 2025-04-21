using JazzPizzazzApp.Models;
using System.Collections.Generic;

namespace JazzPizzazzApp.Services
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        void AddOrder(Order order); // Add a new order
        Order GetOrderByID(int id); // Get order by ID
        void UpdateOrder(Order updatedOrder); // Update an existing order
        List<Order> GetOrdersByDate(string date); // Get orders by date
        void PrintAllOrders(); // Print all orders
        void DailyReport(string date); // Print a daily report of orders
        void AverageSizeByCrust(); // Print average size by crust type
        void OrdersInProgress(); // Print orders that are still in progress
        List<KeyValuePair<string, int>> GetTop5Pizzas(); // Get the top 5 most ordered pizzas
        Dictionary<string, int> GetCrustPopularity(); // Get popularity of each crust type
        void ChangeOrderStatus(int orderID); // Change order status
        void SortOrders(ref List<Order> orders, string sortBy);

        decimal GetTotalAmountEarned();

        // ✅ Add this:
        void SaveAllOrders(List<Order> orders); // Save all orders to file
        
       Dictionary<string, decimal> GetRevenueByPizzaName(int? month = null, int? year = null);
       List<Order> GetOrdersByEmail(string email);
    }
}
