namespace EgorSalahovSemestrovka22.Models.Entities.Orders
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public decimal PriceAtPurchase { get; set; } 
    }
}
