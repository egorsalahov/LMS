using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult Checkout()
        {
            return View();
        }
    }
}