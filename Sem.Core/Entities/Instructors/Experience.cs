namespace EgorSalahovSemestrovka22.Models.Entities.Instructors
{
    public class Experience
    {
        public int Id { get; set; }
        public string Position { get; set; } 
        public string Company { get; set; }
        public string Years { get; set; }

        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }
    }
}
