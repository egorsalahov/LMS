namespace EgorSalahovSemestrovka22.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImagePath { get; set; } 
        public int CourseCount { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
