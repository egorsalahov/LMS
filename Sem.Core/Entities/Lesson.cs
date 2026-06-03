namespace EgorSalahovSemestrovka22.Models.Entities
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public bool IsPreview { get; set; }
        public string? VideoLink { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; }
    }
}
