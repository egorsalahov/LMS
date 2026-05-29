using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Sem.Web.Services
{
    public class OrderService
    {
        private readonly ICartRepository _cartRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IInstructorRepository _instructorRepo;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            ICartRepository cartRepo,
            IOrderRepository orderRepo,
            IEnrollmentRepository enrollmentRepo,
            IInstructorRepository instructorRepo,
            ILogger<OrderService> logger)
        {
            _cartRepo = cartRepo;
            _orderRepo = orderRepo;
            _enrollmentRepo = enrollmentRepo;
            _instructorRepo = instructorRepo;
            _logger = logger;
        }

        public async Task<List<CartItem>> GetCartItemsAsync(string userId)
            => await _cartRepo.GetByStudentAsync(userId);

        public async Task<int> GetCartCountAsync(string userId)
            => await _cartRepo.GetCountByStudentAsync(userId);

        public (decimal subtotal, decimal tax, decimal total) CalculateTotals(List<CartItem> items)
        {
            var subtotal = items.Sum(c => c.Course.Price);
            var tax = Math.Round(subtotal * 0.13m, 2);
            var total = subtotal + tax;
            return (subtotal, tax, total);
        }

        public async Task<(bool success, string message, int cartCount)> AddToCartAsync(string userId, int courseId, bool isInstructor)
        {
            _logger.LogInformation("Добавление в корзину: пользователь={User}, курс={Course}", userId, courseId);
            if (isInstructor)
                return (false, "Instructors cannot purchase courses", 0);

            if (await _enrollmentRepo.AnyAsync(e => e.StudentId == userId && e.CourseId == courseId))
                return (false, "Already enrolled", 0);

            if (await _cartRepo.AnyAsync(c => c.StudentId == userId && c.CourseId == courseId))
                return (false, "Already in cart", 0);

            await _cartRepo.AddAsync(new CartItem { StudentId = userId, CourseId = courseId, AddedAt = DateTime.Now });
            await _cartRepo.SaveChangesAsync();

            var count = await _cartRepo.GetCountByStudentAsync(userId);
            return (true, "Added", count);
        }

        public async Task RemoveFromCartAsync(string userId, int cartItemId)
        {
            _logger.LogInformation("Удаление из корзины: элемент={CartItemId}", cartItemId);
            var item = await _cartRepo.GetByIdAndStudentAsync(cartItemId, userId);
            if (item != null)
            {
                _cartRepo.Delete(item);
                await _cartRepo.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            _logger.LogInformation("Очистка корзины пользователя {User}", userId);
            var items = await _cartRepo.GetAllByStudentAsync(userId);
            _cartRepo.DeleteRange(items);
            await _cartRepo.SaveChangesAsync();
        }

        public async Task<Order> CheckoutAsync(string userId, string firstName, string lastName,
            string addressLine1, string? addressLine2, string country, string state, string city,
            string paymentMethod)
        {
            _logger.LogInformation("Оформление заказа пользователем {User}", userId);
            var cartItems = await _cartRepo.GetAllByStudentAsync(userId);
            if (!cartItems.Any()) throw new InvalidOperationException("Cart is empty");

            if (cartItems.Any(c => c.Course == null))
                throw new InvalidOperationException("Cart contains items with missing course data");

            var (subtotal, tax, total) = CalculateTotals(cartItems);

            var order = new Order
            {
                StudentId = userId,
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

            await _orderRepo.AddAsync(order);

            foreach (var item in cartItems)
                await _enrollmentRepo.AddAsync(new Enrollment
                {
                    StudentId = userId,
                    CourseId = item.CourseId,
                    EnrollmentDate = DateTime.Now,
                    ProgressPercentage = 0
                });

            foreach (var item in cartItems)
            {
                var instructor = await _instructorRepo.GetByIdAsync(item.Course.InstructorId);
                if (instructor != null)
                {
                    instructor.TotalEarnings += item.Course.Price;
                    _instructorRepo.Update(instructor);
                }
            }

            _cartRepo.DeleteRange(cartItems);
            await _orderRepo.SaveChangesAsync();

            _logger.LogInformation("Заказ {OrderId} успешно создан", order.Id);
            return order;
        }

        public async Task<Order?> GetOrderAsync(string userId, int orderId)
            => await _orderRepo.GetByIdAndStudentAsync(orderId, userId);

        public bool ValidateCheckoutFields(string firstName, string lastName, string addressLine1,
            string country, string state, string city)
            => !string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName)
            && !string.IsNullOrWhiteSpace(addressLine1) && !string.IsNullOrWhiteSpace(country)
            && !string.IsNullOrWhiteSpace(state) && !string.IsNullOrWhiteSpace(city);
    }
}