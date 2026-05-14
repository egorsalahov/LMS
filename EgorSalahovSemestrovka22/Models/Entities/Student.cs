using EgorSalahovSemestrovka22.Models.Entities.Orders;
using Microsoft.AspNetCore.Identity;

namespace EgorSalahovSemestrovka22.Models.Entities
{
    public class Student : IdentityUser
    {
        //public int Id { get; set; }

        //Данные профиля
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string UserName { get; set; }
        //public string Email { get; set; }
        //public string PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public string? Bio { get; set; }
        public string? AvatarPath { get; set; }

        // Купленные курсы (скрин 2: Enrolled Courses)
        // Через эту коллекцию мы будем выводить карточки на странице обучения
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        // 2. Список желаемого
        public virtual ICollection<Wishlist> Wishlist { get; set; } = new List<Wishlist>();

        // История заказов
        // Связь с таблицей чеков/платежей
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        // 4. Корзина (Cart)
        // Чтобы товары не пропадали при перезагрузке страницы
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
