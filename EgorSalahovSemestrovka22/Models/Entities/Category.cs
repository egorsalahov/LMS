namespace EgorSalahovSemestrovka22.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImagePath { get; set; } // Для плиток на странице категорий
        public int CourseCount { get; set; } // Можно сделать расчетным полем
        public ICollection<Course> Courses { get; set; }
    }
}
