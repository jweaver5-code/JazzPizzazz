namespace JazzPizzazzApp.Models
{
    public class Menu
    {
        public int PizzaID { get; set; }
        public string PizzaName { get; set; } = string.Empty;
        public string Toppings { get; set; } = string.Empty;
        public string Crust { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool InStock { get; set; }
        public bool Available { get; set; }

        static public int Max { get; set; }
        static public int Count { get; set; }

        // ✅ Required for model binding
        public Menu() { }

        // Optional constructor for manual creation
        public Menu(int pizzaID, string pizzaName, string toppings, string crust, double price, bool inStock, bool available)
        {
            PizzaID = pizzaID;
            PizzaName = pizzaName;
            Toppings = toppings;
            Crust = crust;
            Price = price;
            InStock = inStock;
            Available = available;
        }

        // Static methods
        static public void IncCount() => Count++;
        static public void SetMax(int max) => Max = max;
        static public void IncMax() => Max++;
        static public void DecrMax() => Max--;
        static public void SetCount(int count) => Count = count;
        static public int GetCount() => Count;

        public int GetPizzaID() => PizzaID;

        public override string ToString() =>
            $"{PizzaID}#{PizzaName}#{Toppings}#{Crust}#{Price}#{InStock}#{Available}";

        public string ToFile() =>
            $"{PizzaID}#{PizzaName}#{Toppings}#{Crust}#{Price}#{InStock}#{Available}";

        public void SetOptions(string options)
        {
            this.Toppings = options;
        }
    }
}
