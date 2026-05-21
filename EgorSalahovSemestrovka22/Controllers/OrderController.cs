using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Student> _userManager;

        public OrderController(AppDbContext context, UserManager<Student> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Order/Cart
        [Authorize]
        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");

            var cartItems = await _context.CartItems
                .Include(c => c.Course)
                    .ThenInclude(c => c.Instructor)
                .Include(c => c.Course)
                    .ThenInclude(c => c.Reviews)
                .Where(c => c.StudentId == user.Id)
                .OrderByDescending(c => c.AddedAt)
                .ToListAsync();

            var subtotal = cartItems.Sum(c => c.Course.Price);
            var tax = Math.Round(subtotal * 0.13m, 2);
            var total = subtotal + tax;

            ViewBag.Subtotal = subtotal;
            ViewBag.Tax = tax;
            ViewBag.Total = total;

            return View(cartItems);
        }

        // POST: /Order/AddToCart — AJAX
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Not authorized" });

            if (await _userManager.IsInRoleAsync(user, "Instructor"))
                return Json(new { success = false, message = "Instructors cannot purchase courses" });

            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == user.Id && e.CourseId == courseId);
            if (alreadyEnrolled)
                return Json(new { success = false, message = "Already enrolled" });

            var alreadyInCart = await _context.CartItems
                .AnyAsync(c => c.StudentId == user.Id && c.CourseId == courseId);
            if (alreadyInCart)
                return Json(new { success = false, message = "Already in cart" });

            var cartItem = new CartItem
            {
                StudentId = user.Id,
                CourseId = courseId,
                AddedAt = DateTime.Now
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            var count = await _context.CartItems.CountAsync(c => c.StudentId == user.Id);

            return Json(new { success = true, cartCount = count });
        }

        // GET: /Order/GetCartCount — AJAX
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { count = 0 });

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { count = 0 });

            var count = await _context.CartItems.CountAsync(c => c.StudentId == user.Id);
            return Json(new { count = count });
        }

        // POST: /Order/RemoveFromCart
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var user = await _userManager.GetUserAsync(User);
            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.StudentId == user.Id);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Cart");
        }

        // POST: /Order/ClearCart
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ClearCart()
        {
            var user = await _userManager.GetUserAsync(User);
            var items = await _context.CartItems
                .Where(c => c.StudentId == user.Id)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return RedirectToAction("Cart");
        }

        // GET: /Order/Checkout
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");

            var cartItems = await _context.CartItems
                .Include(c => c.Course)
                .Where(c => c.StudentId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Cart");

            var subtotal = cartItems.Sum(c => c.Course.Price);
            var tax = Math.Round(subtotal * 0.13m, 2);
            var total = subtotal + tax;

            ViewBag.Subtotal = subtotal;
            ViewBag.Tax = tax;
            ViewBag.Total = total;

            // Обновляем TotalEarnings инструкторов
            foreach (var item in cartItems)
            {
                var course = item.Course;
                var instructor = await _context.Instructors.FindAsync(course.InstructorId);
                if (instructor != null)
                {
                    instructor.TotalEarnings += course.Price;
                }
            }
            await _context.SaveChangesAsync();

            return View(cartItems);
        }

        // POST: /Order/Checkout
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(string firstName, string lastName, string phone,
            string addressLine1, string addressLine2, string country, string state, string city,
            string paymentMethod)
        {
            // Серверная валидация
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(addressLine1) || string.IsNullOrWhiteSpace(country) ||
                string.IsNullOrWhiteSpace(state) || string.IsNullOrWhiteSpace(city))
            {
                TempData["ErrorMessage"] = "All required fields must be filled.";
                return RedirectToAction("Checkout");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");

            var cartItems = await _context.CartItems
                .Include(c => c.Course)
                .Where(c => c.StudentId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
                return RedirectToAction("Cart");

            var subtotal = cartItems.Sum(c => c.Course.Price);
            var tax = Math.Round(subtotal * 0.13m, 2);
            var total = subtotal + tax;

            var order = new Order
            {
                StudentId = user.Id,
                OrderDate = DateTime.Now,
                TotalAmount = total,
                Tax = tax,
                FirstName = firstName,
                LastName = lastName,
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                Country = country,
                State = state,
                City = city,
                PaymentMethod = total == 0 ? "Free" : paymentMethod,
                OrderStatus = "Completed",
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    CourseId = c.CourseId,
                    PriceAtPurchase = c.Course.Price
                }).ToList()
            };

            _context.Orders.Add(order);

            // Создаём Enrollment для каждого курса
            foreach (var item in cartItems)
            {
                var enrollment = new Enrollment
                {
                    StudentId = user.Id,
                    CourseId = item.CourseId,
                    EnrollmentDate = DateTime.Now,
                    ProgressPercentage = 0
                };
                _context.Enrollments.Add(enrollment);
            }

            // Очищаем корзину
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }


        // GET: /Order/Confirmation
        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Course)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.StudentId == user.Id);

            if (order == null)
                return NotFound("Order not found");

            return View(order);
        }
    }
}
