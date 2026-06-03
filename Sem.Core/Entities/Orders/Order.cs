using EgorSalahovSemestrovka22.Models.Entities.Orders;

namespace EgorSalahovSemestrovka22.Models.Entities.Orders
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string StudentId { get; set; }
        public Student Student { get; set; }

        public decimal TotalAmount { get; set; } 
        public decimal Tax { get; set; }     

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }


        public string PaymentMethod { get; set; } 
        public string OrderStatus { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
