namespace EgorSalahovSemestrovka22.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; } // 1-5
        public DateTime CreatedAt { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int StudentId { get; set; } // Кто оставил отзыв
    }
}
