namespace EgorSalahovSemestrovka22.Models.Entities.Instructors
{
    public class Education
    {
        public int Id { get; set; }
        public string Degree { get; set; } 
        public string Institute { get; set; } 
        public string Years { get; set; } 

        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }
    }
}
