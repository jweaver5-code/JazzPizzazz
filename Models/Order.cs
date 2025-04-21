using System;

namespace JazzPizzazzApp.Models
{
    public class Order
    {
        // Fields
        public int OrderID { get; set; }
        public string Email { get; set; }
        public string Pizza { get; set; }
        public string Date { get; set; }
        public int Size { get; set; }
        public string Crust { get; set; }
        public string Status { get; set; }

        // Constructor
        public Order(int orderID, string email, string pizza, string date, int size, string crust, string status)
        {
            OrderID = orderID;
            Email = email;
            Pizza = pizza;
            Date = date;
            Size = size;
            Crust = crust;
            Status = status;
        }

        // Getters (to access private fields)
        public int GetOrderID() => OrderID;
        public string GetEmail() => Email;
        public string GetPizza() => Pizza;
        public string GetDate() => Date;
        public int GetSize() => Size;
        public string GetCrust() => Crust;
        public string GetStatus() => Status;

        // Setters (to set fields)
        public void SetOrderID(int orderID) => OrderID = orderID;
        public void SetEmail(string email) => Email = email;
        public void SetPizza(string pizza) => Pizza = pizza;
        public void SetDate(string date) => Date = date;
        public void SetSize(int size) => Size = size;
        public void SetCrust(string crust) => Crust = crust;
        public void SetStatus(string status) => Status = status;

        // Additional methods as needed
        
        public override string ToString()
        {
            return $"OrderID: {OrderID}, Email: {Email}, Pizza: {Pizza}, Date: {Date}, Size: {Size}, Crust: {Crust}, Status: {Status}";
        }

        public string ToFile()
        {
            return $"{OrderID}#{Email}#{Pizza}#{Date}#{Size}#{Crust}#{Status}";
        }
    }
}
