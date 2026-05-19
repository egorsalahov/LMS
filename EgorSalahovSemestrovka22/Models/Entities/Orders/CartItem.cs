namespace EgorSalahovSemestrovka22.Models.Entities.Orders
{
    public class CartItem
    {
        public int Id { get; set; }

        public string StudentId { get; set; } // Чья корзина
        public Student Student { get; set; }

        public int CourseId { get; set; } // Какой курс
        public Course Course { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
