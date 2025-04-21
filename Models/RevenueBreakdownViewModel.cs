namespace JazzPizzazzApp.Models
{
    public class RevenueBreakdownViewModel
    {
        public decimal TotalRevenue { get; set; }
        public Dictionary<string, decimal> RevenueByPizzaName { get; set; }
    }
}
