namespace JazzPizzazzApp.Models
{
    public class Receipt
    {
        public string PizzaName { get; set; }    // Capital P and N
        public int Size { get; set; }            // Capital S
        public string Crust { get; set; }        // Capital C
        public string TotalCost { get; set; }    // Capital T and C

        public Receipt(string pizzaName, int size, string crust, string totalCost)
        {
            PizzaName = pizzaName;
            Size = size;
            Crust = crust;
            TotalCost = totalCost;
        }
    }
}
