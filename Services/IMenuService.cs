using JazzPizzazzApp.Models;
using System.Collections.Generic;

namespace JazzPizzazzApp.Services
{
    public interface IMenuService
    {
        List<Menu> GetMenus();
        void AddPizza(Menu newPizza);
        bool RemovePizza(int pizzaID);
        bool EditPizza(int pizzaID, Menu updatedPizza);
        bool EditOptions(int pizzaID, string newToppings, string newCrust);
        Menu FindPizza(int pizzaID);
        List<Menu> FindPizzasByName(string pizzaName);
        bool IsPizzaInStock(int pizzaID);
        
    }
}
