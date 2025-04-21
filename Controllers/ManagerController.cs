using JazzPizzazzApp.Models;
using JazzPizzazzApp.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace JazzPizzazzApp.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMenuService _menuService;

        public ManagerController(IOrderService orderService, IMenuService menuService)
        {
            _orderService = orderService;
            _menuService = menuService;
        }

        [HttpGet]
        public ActionResult Login() => View();

        [HttpPost]
        public ActionResult Login(string username)
        {
            HttpContext.Session.SetString("ManagerUsername", username); // ✅ store login in session
            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard()
        {
            if (TempData["ManagerUsername"] == null)
                return RedirectToAction("Login");

            TempData.Keep("ManagerUsername");
            ViewBag.ManagerUsername = TempData["ManagerUsername"] as string;
            return View();
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Remove("ManagerUsername");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ViewMenu()
        {
            TempData.Keep("ManagerUsername");
            return View(_menuService.GetMenus());
        }

        [HttpGet]
        public ActionResult AddPizzaSelection()
        {
            TempData.Keep("ManagerUsername");
            return View();
        }

        [HttpGet]
        public ActionResult AddPizza()
        {
            TempData.Keep("ManagerUsername");
            return View();
        }

        [HttpPost]
        public ActionResult AddPizza(Menu newPizza)
        {
            if (ModelState.IsValid)
            {
                newPizza.Available = true;
                _menuService.AddPizza(newPizza);
                TempData.Keep("ManagerUsername");
                return RedirectToAction("ViewMenu");
            }

            TempData.Keep("ManagerUsername");
            return View(newPizza);
        }

        [HttpGet]
        public ActionResult RemovePizza()
        {
            TempData.Keep("ManagerUsername");
            var availablePizzas = _menuService.GetMenus().Where(p => p.Available).ToList();
            return View(availablePizzas);
        }

        [HttpPost]
        public ActionResult RemovePizza(int pizzaID)
        {
            var pizza = _menuService.GetMenus().FirstOrDefault(p => p.PizzaID == pizzaID);
            if (pizza != null)
            {
                pizza.Available = false;
                _menuService.EditPizza(pizza.PizzaID, pizza);
                TempData.Keep("ManagerUsername");
                return RedirectToAction("ViewMenu");
            }
            return NotFound("Pizza not found.");
        }

        [HttpGet]
        public ActionResult EditPizza()
        {
            TempData.Keep("ManagerUsername");
            ViewBag.EditFields = new List<string> { "PizzaName", "Toppings", "Crust", "Price" };
            return View(_menuService.GetMenus());
        }

        [HttpPost]
        public ActionResult EditPizza(int PizzaID, string fieldToEdit, string newValue)
        {
            var pizza = _menuService.GetMenus().FirstOrDefault(p => p.PizzaID == PizzaID);
            if (pizza != null)
            {
                switch (fieldToEdit)
                {
                    case "PizzaName":
                        pizza.PizzaName = newValue;
                        break;
                    case "Toppings":
                        pizza.Toppings = newValue;
                        break;
                    case "Crust":
                        pizza.Crust = newValue;
                        break;
                    case "Price":
                        if (double.TryParse(newValue, out double parsedPrice))
                        {
                            pizza.Price = parsedPrice;
                        }
                        else
                        {
                            TempData["Error"] = "Invalid price format.";
                            TempData.Keep("ManagerUsername");
                            return View(_menuService.GetMenus());
                        }
                        break;
                }

                _menuService.EditPizza(PizzaID, pizza);
                TempData.Keep("ManagerUsername");
                return RedirectToAction("ViewMenu");
            }

            return NotFound("Pizza not found.");
        }


        [HttpPost]
        public ActionResult EditOptions(int pizzaID, string newToppings, string newCrust)
        {
            if (_menuService.EditOptions(pizzaID, newToppings, newCrust))
            {
                TempData.Keep("ManagerUsername");
                return RedirectToAction("ViewMenu");
            }
            return NotFound("Pizza not found.");
        }

        public IActionResult ViewAllOrders(string sortBy)
        {
            // Get all orders
            var orders = _orderService.GetAllOrders();

            // Apply sorting based on the selected sortBy value
            switch (sortBy?.ToLower())
            {
                case "crust":
                    orders = orders.OrderBy(o => o.Crust).ToList();
                    break;
                case "email":
                    orders = orders.OrderBy(o => o.Email).ToList();
                    break;
                case "pizza":
                    orders = orders.OrderBy(o => o.Pizza).ToList();
                    break;
                case "date":
                    orders = orders.OrderBy(o => DateTime.Parse(o.Date)).ToList();  // Ensure Date is properly parsed
                    break;
                case "size":
                    orders = orders.OrderBy(o => o.Size).ToList();
                    break;
                case "orderid":
                    orders = orders.OrderBy(o => o.OrderID).ToList();
                    break;
                case "status":
                    orders = orders.OrderBy(o => o.Status).ToList();
                    break;
                default:
                    // Default sorting can be by Order ID
                    orders = orders.OrderBy(o => o.OrderID).ToList();
                    break;
            }

            // Pass sorted orders to the view
            return View(orders);
        }


        public ActionResult DailyReport(string date)
        {
            TempData.Keep("ManagerUsername");
            var dailyOrders = _orderService.GetOrdersByDate(date);
            return dailyOrders.Any()
                ? View("DailyReport", dailyOrders)
                : NotFound("No orders were placed on this day.");
        }

        public IActionResult Top5Pizzas()
        {
            var topPizzas = _orderService.GetTop5Pizzas();
            return View(topPizzas);
        }

        [HttpGet]
        public ActionResult AverageSizeByCrust()
        {
            TempData.Keep("ManagerUsername");
            _orderService.AverageSizeByCrust();
            return View();
        }

        // 🆕 AJAX-BASED ORDER STATUS PAGE
        [HttpGet]
        public ActionResult ChangeOrderStatus()
        {
            TempData.Keep("ManagerUsername");
            // Get all orders and pass them to the view for the dropdown
            var orders = _orderService.GetAllOrders();
            return View(orders);
        }

        [HttpPost]
        public ActionResult ChangeOrderStatus(int orderID)
        {
            TempData.Keep("ManagerUsername");

            // Call the service method to toggle the order status
            _orderService.ChangeOrderStatus(orderID);

            // Redirect to ViewOrders after the status is updated
            return RedirectToAction("ViewAllOrders");
        }

        // 🆕 AJAX handler for status update
        // [HttpPost]
        // public IActionResult UpdateOrderStatus(int orderId, string newStatus)
        // {
        //     bool success = _orderService.ChangeOrderStatus(orderId, newStatus);

        //     if (success)
        //     {
        //         return Json(new { success = true });
        //     }
        //     else
        //     {
        //         return Json(new { success = false, message = "Unable to update order status." });
        //     }
        // }

        [HttpPost]
        public JsonResult ToggleOrderStatus([FromBody] dynamic payload)
        {
            int orderID = (int)payload.orderID;
            var order = _orderService.GetOrderByID(orderID);

            if (order == null)
                return Json(new { success = false, message = "Order not found." });

            order.Status = (order.Status == "Completed") ? "In Progress" : "Completed";
            _orderService.UpdateOrder(order);

            return Json(new { success = true, newStatus = order.Status });
        }

        [HttpGet]
        public IActionResult ViewDailyOrders()
        {
            var defaultDate = DateTime.Now;
            ViewBag.SelectedDate = defaultDate.ToString("MM/dd/yy");
            return View();
        }

        [HttpPost]
        public IActionResult ViewDailyOrders(string selectedDate)
        {
            if (!DateTime.TryParseExact(selectedDate, "MM/dd/yy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                ViewBag.ErrorMessage = "Invalid date format. Please enter a valid date (MM/dd/yy).";
                return View();
            }

            var dailyOrders = _orderService.GetOrdersByDate(selectedDate);
            ViewBag.SelectedDate = selectedDate;
            return View(dailyOrders);
        }

        [HttpGet]
        public IActionResult Reports()
        {
            TempData.Keep("ManagerUsername");
            return View(); // expects Views/Manager/Reports.cshtml
        }

        [HttpGet]
        public IActionResult TotalAmountEarned(int? month, int? year)
        {
            Dictionary<string, decimal> revenueByPizza;

            if (month.HasValue && year.HasValue)
            {
                revenueByPizza = _orderService.GetRevenueByPizzaName(month.Value, year.Value); // Filtered by month/year
                ViewBag.FilterLabel = $"{new DateTime(year.Value, month.Value, 1):MMMM yyyy}";
            }
            else
            {
                revenueByPizza = _orderService.GetRevenueByPizzaName(); // All-time
                ViewBag.FilterLabel = "All Time";
            }

            decimal total = revenueByPizza.Values.Sum();

            var model = new RevenueBreakdownViewModel
            {
                TotalRevenue = total,
                RevenueByPizzaName = revenueByPizza
            };

            return View(model);
        }




        // 🆕 CRUST POPULARITY REPORT
        public IActionResult CrustPopularity()
        {
            TempData.Keep("ManagerUsername");

            var crustCounts = _orderService.GetCrustPopularity();

            if (crustCounts == null || crustCounts.Count == 0)
            {
                ViewData["Message"] = "No crust data available.";
                return View(new List<KeyValuePair<string, int>>());
            }

            var sortedCrusts = crustCounts
                .OrderByDescending(kvp => kvp.Value)
                .ToList();

            return View(sortedCrusts);
        }

        [HttpGet]
        public ActionResult ReAddUnavailablePizza()
        {
            TempData.Keep("ManagerUsername");
            // Get the list of unavailable pizzas (pizzas marked as unavailable)
            var unavailablePizzas = _menuService.GetMenus().Where(p => !p.Available).ToList();
            return View(unavailablePizzas);
        }

        // This action will be called when the manager selects to re-add a pizza from the list
        [HttpPost]
        public ActionResult ReAddUnavailablePizza(int pizzaID)
        {
            var pizza = _menuService.GetMenus().FirstOrDefault(p => p.PizzaID == pizzaID);
            if (pizza != null)
            {
                pizza.Available = true;
                _menuService.EditPizza(pizza.PizzaID, pizza);
                TempData.Keep("ManagerUsername");
                return RedirectToAction("ViewMenu");
            }
            return NotFound("Pizza not found.");
        }
    }
}
