using Microsoft.AspNetCore.Mvc;
using JazzPizzazzApp.Models;
using JazzPizzazzApp.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Globalization;

namespace JazzPizzazzApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMenuService _menuService;

        public CustomerController(IOrderService orderService, IMenuService menuService)
        {
            _orderService = orderService;
            _menuService = menuService;
        }

        private bool IsLoggedIn()
        {
            var email = HttpContext.Session.GetString("CustomerEmail");
            return !string.IsNullOrEmpty(email);
        }

        [HttpGet]
        public ActionResult Login() => View();

        [HttpPost]
        public ActionResult Login(string email)
        {
            HttpContext.Session.SetString("CustomerEmail", email); // Correct this line
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            var email = HttpContext.Session.GetString("CustomerEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var username = email.Split('@')[0];
            ViewData["Email"] = email;
            ViewData["Username"] = username;

            return View();
        }

        [HttpGet]
        public ActionResult ViewAvailablePizzas()
        {
            var email = HttpContext.Session.GetString("CustomerEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var menus = _menuService.GetMenus();
            return View("ViewAvailablePizzas", menus);
        }

        [HttpGet]
        public ActionResult PlaceOrder()
        {
            var email = HttpContext.Session.GetString("CustomerEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var menus = _menuService.GetMenus();
            return View("PlaceOrder", menus);
        }

        [HttpPost]
        public ActionResult PlaceOrder(int pizzaID, string size, string crust)
        {
            var email = HttpContext.Session.GetString("CustomerEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var menus = _menuService.GetMenus();
            var selectedPizza = menus.FirstOrDefault(m => m.PizzaID == pizzaID);

            if (selectedPizza != null)
            {
                var allOrders = _orderService.GetAllOrders();
                var nextOrderID = allOrders.Count > 0 ? allOrders.Max(o => o.OrderID) + 1 : 1;
                var pizzaSize = int.Parse(size);

                var basePrice = 10.0;
                var pricePerInch = 0.75;
                var totalCost = basePrice + (pizzaSize * pricePerInch);

                var newOrder = new Order(
                    orderID: nextOrderID,
                    email: email,
                    pizza: selectedPizza.PizzaName,
                    date: DateTime.Now.ToString("MM/dd/yy"),
                    size: pizzaSize,
                    crust: crust,
                    status: "In-Progress"
                );

                allOrders.Add(newOrder);
                _orderService.SaveAllOrders(allOrders);

                var receipt = new Receipt(
                    pizzaName: selectedPizza.PizzaName,
                    size: pizzaSize,
                    crust: crust,
                    totalCost: totalCost.ToString("0.00")
                );

                TempData["Receipt"] = JsonConvert.SerializeObject(receipt);
                return RedirectToAction("OrderReceipt");
            }
            else
            {
                ViewData["ErrorMessage"] = "Pizza not found.";
                return View("PlaceOrder", menus);
            }
        }

        [HttpGet]
        public ActionResult OrderReceipt()
        {
            var email = HttpContext.Session.GetString("CustomerEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var receiptJson = TempData["Receipt"] as string;

            if (string.IsNullOrEmpty(receiptJson))
            {
                return RedirectToAction("PlaceOrder");
            }

            var receipt = JsonConvert.DeserializeObject<Receipt>(receiptJson);
            TempData.Keep("Receipt");
            return View(receipt);
        }

        [HttpGet]
        public ActionResult ViewPastOrders()
        {
            var email = HttpContext.Session.GetString("CustomerEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var username = email.Split('@')[0];

            var allOrders = _orderService.GetAllOrders();
            var customerOrders = allOrders
                .Where(o => o.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            ViewData["Username"] = username;
            return View(customerOrders);
        }

        [HttpPost]
        public ActionResult FinalizeOrder()
        {
            var email = HttpContext.Session.GetString("CustomerEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var currentOrderJson = TempData["CurrentOrder"] as string;
            var currentOrder = string.IsNullOrEmpty(currentOrderJson)
                ? new List<Order>()
                : JsonConvert.DeserializeObject<List<Order>>(currentOrderJson);

            if (currentOrder == null || currentOrder.Count == 0)
                return RedirectToAction("PlaceOrder");

            foreach (var order in currentOrder)
            {
                var orderText = $"{order.OrderID},{order.Email},{order.Pizza},{order.Date},{order.Size},{order.Crust},{order.Status}";
                System.IO.File.AppendAllText("orders.txt", orderText + Environment.NewLine);
            }

            TempData.Remove("CurrentOrder");
            return RedirectToAction("ViewPastOrders");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CustomerEmail");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult TrackMyOrder()
        {
            return View("TrackMyOrder");
        }





    }
}
