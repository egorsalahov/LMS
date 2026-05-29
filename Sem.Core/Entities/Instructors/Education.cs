namespace EgorSalahovSemestrovka22.Models.Entities.Instructors
{
    public class Education
    {
        public int Id { get; set; }
        public string Degree { get; set; } // Например, "BCA - Bachelor of Computer Applications"
        public string Institute { get; set; } // "International University"
        public string Years { get; set; } // "2004 - 2010"

        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }
    }
}
