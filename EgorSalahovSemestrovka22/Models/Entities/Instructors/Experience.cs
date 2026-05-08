namespace EgorSalahovSemestrovka22.Models.Entities.Instructors
{
    public class Experience
    {
        public int Id { get; set; }
        public string Position { get; set; } // "Web Design & Development Team Leader"
        public string Company { get; set; } // "Creative Agency"
        public string Years { get; set; } // "2013 - 2016"

        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }
    }
}
