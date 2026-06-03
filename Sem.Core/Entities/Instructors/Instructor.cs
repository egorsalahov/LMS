namespace EgorSalahovSemestrovka22.Models.Entities.Instructors
{
    public class Instructor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public string Bio { get; set; }
        public string? AvatarPath { get; set; }
        public decimal TotalEarnings { get; set; }


        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Education> Educations { get; set; } = new List<Education>();
        public ICollection<Experience> Experiences { get; set; } = new List<Experience>();

    }
}
