namespace EgorSalahovSemestrovka22.Models.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public int ProgressPercentage { get; set; } = 0; // Для отображения в Dashboard студента
    }
}
