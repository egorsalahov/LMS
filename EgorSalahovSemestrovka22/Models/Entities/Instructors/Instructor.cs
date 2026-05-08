namespace EgorSalahovSemestrovka22.Models.Entities.Instructors
{
    public class Instructor
    {
        public int Id { get; set; }

        // Данные из профиля (image_34418d.jpg)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public string Bio { get; set; }
        public string AvatarPath { get; set; }

        // Статистика (image_34414e.jpg)
        // Эти поля можно вычислять через связи, но иногда их хранят для быстроты
        public decimal TotalEarnings { get; set; }

        // Связи
        public ICollection<Course> Courses { get; set; } = new List<Course>();

        // Образование и Опыт 
        public ICollection<Education> Educations { get; set; } = new List<Education>();
        public ICollection<Experience> Experiences { get; set; } = new List<Experience>();

        // Для вывода "Total Students"
        // Можно получить через: Courses.SelectMany(c => c.Enrollments).Count()
    }
}
