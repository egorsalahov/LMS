namespace EgorSalahovSemestrovka22.Models.Entities
{
    public class Wishlist
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } // Ссылка на объект студента

        public int CourseId { get; set; }
        public Course Course { get; set; }   // Ссылка на объект курса
    }
}
