using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sem.Web.Services;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly UserManager<Student> _userManager;
        private readonly ILogger<OrderController> _logger;

        public OrderController(OrderService orderService, UserManager<Student> userManager, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _userManager = userManager;
            _logger = logger;
        }

        private async Task<Student?> GetCurrentUserAsync()
            => await _userManager.GetUserAsync(User);

        [Authorize, HttpGet]
        public async Task<IActionResult> Cart()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var items = await _orderService.GetCartItemsAsync(user.Id);
            var (subtotal, tax, total) = _orderService.CalculateTotals(items);

            ViewBag.Subtotal = subtotal;
            ViewBag.Tax = tax;
            ViewBag.Total = total;

            return View(items);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> AddToCart(int courseId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Json(new { success = false, message = "Not authorized" });

            var isInstructor = await _userManager.IsInRoleAsync(user, "Instructor");
            _logger.LogInformation("Пользователь {Email} добавляет курс {CourseId} в корзину", user.Email, courseId);
            var (success, message, count) = await _orderService.AddToCartAsync(user.Id, courseId, isInstructor);

            return Json(new { success, message, cartCount = count });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            if (!User.Identity!.IsAuthenticated) return Json(new { count = 0 });

            var user = await GetCurrentUserAsync();
            if (user == null) return Json(new { count = 0 });

            var count = await _orderService.GetCartCountAsync(user.Id);
            return Json(new { count });
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                _logger.LogInformation("Удаление элемента {ItemId} из корзины пользователя {Email}", cartItemId, user.Email);
                await _orderService.RemoveFromCartAsync(user.Id, cartItemId);
            }
            return RedirectToAction("Cart");
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> ClearCart()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                _logger.LogInformation("Очистка корзины пользователя {Email}", user.Email);
                await _orderService.ClearCartAsync(user.Id);
            }
            return RedirectToAction("Cart");
        }

        [Authorize, HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var items = await _orderService.GetCartItemsAsync(user.Id);
            if (!items.Any()) return RedirectToAction("Cart");

            var (subtotal, tax, total) = _orderService.CalculateTotals(items);
            ViewBag.Subtotal = subtotal;
            ViewBag.Tax = tax;
            ViewBag.Total = total;

            return View(items);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Checkout(string firstName, string lastName, string phone,
            string addressLine1, string? addressLine2, string country, string state, string city,
            string paymentMethod)
        {
            if (!_orderService.ValidateCheckoutFields(firstName, lastName, addressLine1, country, state, city))
            {
                TempData["ErrorMessage"] = "All required fields must be filled.";
                return RedirectToAction("Checkout");
            }

            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            _logger.LogInformation("Оформление заказа пользователем {Email}", user.Email);
            var order = await _orderService.CheckoutAsync(user.Id, firstName, lastName,
                addressLine1, addressLine2, country, state, city, paymentMethod);

            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }

        [Authorize, HttpGet]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction("SignIn", "Account");

            var order = await _orderService.GetOrderAsync(user.Id, orderId);
            if (order == null) return NotFound("Order not found");

            return View(order);
        }
    }
}