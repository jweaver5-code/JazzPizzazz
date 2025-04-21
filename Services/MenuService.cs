using JazzPizzazzApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JazzPizzazzApp.Services
{
    public class MenuService : IMenuService
    {
        private List<Menu> _menus;

        // Constructor
        public MenuService()
        {
            _menus = new List<Menu>(); // Initialize with an empty list
            _menus = GetMenus(); // Load menus from file
        }

        // Method to get all menus from the file
        public List<Menu> GetMenus()
{
    // Ensure the file exists
    if (!File.Exists("pizza-menu.txt"))
    {
        // If the file doesn't exist, return an empty list
        return new List<Menu>();
    }

    _menus.Clear(); // Clear the list before loading

    // Read the menu data from the file
    using (StreamReader inFile = new StreamReader("pizza-menu.txt"))
    {
        string line;
        while ((line = inFile.ReadLine()) != null)
        {
            string[] temp = line.Split('#');
            var menu = new Menu(
                int.Parse(temp[0]), // PizzaID remains the same
                temp[1],             // PizzaName
                temp[2],             // Toppings
                temp[3],             // Size
                double.Parse(temp[4]), // Price
                bool.Parse(temp[5]),  // Available (true or false)
                bool.Parse(temp[6])   // HasSpecialOptions (if applicable)
            );
            _menus.Add(menu);
        }
    }

    // Ensure PizzaID remains intact and sequential
    int id = 1;
    foreach (var menu in _menus)
    {
        menu.PizzaID = id++; // Reset PizzaID to start from 1 for the current session
    }

    return _menus;
}

        // Method to save the menu back to the file
        private void SaveMenus()
        {
            using (StreamWriter outFile = new StreamWriter("pizza-menu.txt"))
            {
                foreach (var menu in _menus)
                {
                    outFile.WriteLine(menu.ToFile());
                }
            }
        }

        // Method to add a new pizza to the menu
        public void AddPizza(Menu newPizza)
        {
            _menus.Add(newPizza); // Add the new pizza to the list
            Menu.IncMax(); // Increment the Max static field
            SaveMenus(); // Save the updated menu to the file
        }

        // Method to remove a pizza from the menu by pizza ID
        public bool RemovePizza(int pizzaID)
        {
            var pizzaToRemove = _menus.FirstOrDefault(m => m.PizzaID == pizzaID);
            if (pizzaToRemove != null)
            {
                _menus.Remove(pizzaToRemove);
                Menu.DecrMax(); // Decrement the Max static field
                SaveMenus(); // Save the updated menu to the file
                return true;
            }
            return false; // Pizza not found
        }

        // Method to edit a pizza's details
        public bool EditPizza(int pizzaID, Menu updatedPizza)
        {
            var pizzaToEdit = _menus.FirstOrDefault(m => m.PizzaID == pizzaID);
            if (pizzaToEdit != null)
            {
                pizzaToEdit.PizzaName = updatedPizza.PizzaName;
                pizzaToEdit.Toppings = updatedPizza.Toppings;
                pizzaToEdit.Crust = updatedPizza.Crust;
                pizzaToEdit.Price = updatedPizza.Price;
                pizzaToEdit.InStock = updatedPizza.InStock;
                pizzaToEdit.Available = updatedPizza.Available;

                SaveMenus(); // Save the updated menu to the file
                return true;
            }
            return false; // Pizza not found
        }

        // Method to edit options of a pizza (e.g., toppings, crust)
        public bool EditOptions(int pizzaID, string newToppings, string newCrust)
        {
            var pizzaToEdit = _menus.FirstOrDefault(m => m.PizzaID == pizzaID);
            if (pizzaToEdit != null)
            {
                pizzaToEdit.SetOptions(newToppings);
                pizzaToEdit.Crust = newCrust; // Assuming crust can be edited as well
                SaveMenus(); // Save the updated menu to the file
                return true;
            }
            return false; // Pizza not found
        }

        // Method to find a pizza by pizza ID
        public Menu FindPizza(int pizzaID)
        {
            return _menus.FirstOrDefault(m => m.PizzaID == pizzaID);
        }

        // Method to find pizzas by name
        public List<Menu> FindPizzasByName(string pizzaName)
        {
            return _menus.Where(m => m.PizzaName.Contains(pizzaName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Method to check if a pizza is in stock
        public bool IsPizzaInStock(int pizzaID)
        {
            var pizza = _menus.FirstOrDefault(m => m.PizzaID == pizzaID);
            return pizza?.InStock ?? false;
        }

        // Additional methods can be added for filtering and other operations as needed
    }
}
