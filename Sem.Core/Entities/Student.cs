using EgorSalahovSemestrovka22.Models.Entities.Orders;
using Microsoft.AspNetCore.Identity;

namespace EgorSalahovSemestrovka22.Models.Entities
{
    public class Student : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public string? Bio { get; set; }
        public string? AvatarPath { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public virtual ICollection<Wishlist> Wishlist { get; set; } = new List<Wishlist>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
