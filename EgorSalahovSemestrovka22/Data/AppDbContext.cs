using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;
using EgorSalahovSemestrovka22.Models.Entities.Orders;
using EgorSalahovSemestrovka22.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка точности для денег (decimal)
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            //Сид Категорий
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Frontend Development", ImagePath = "cat-1.png", CourseCount = 5 },
                new Category { Id = 2, Name = "Backend Development", ImagePath = "cat-2.png", CourseCount = 5 }
            );

            //Сид Преподавателей
            modelBuilder.Entity<Instructor>().HasData(
                new Instructor
                {
                    Id = 1,
                    FirstName = "Egor",
                    LastName = "Salahov",
                    UserName = "egor_dev",
                    Email = "egor@example.com",
                    PhoneNumber = "+1234567890",
                    Bio = "Senior Fullstack Developer",
                    AvatarPath = "instructor-1.png",
                    TotalEarnings = 1500.00m,
                    Gender = "Male",
                    DateOfBirth = new DateTime(1995, 5, 20),
                    RegistrationDate = new DateTime(2026, 01, 01)
                },
                new Instructor
                {
                    Id = 2,
                    FirstName = "Anna",
                    LastName = "Pro",
                    UserName = "anna_web",
                    Email = "anna@example.com",
                    PhoneNumber = "+9876543210",
                    Bio = "UI/UX Expert",
                    AvatarPath = "instructor-2.png",
                    TotalEarnings = 2300.50m,
                    Gender = "Female",
                    DateOfBirth = new DateTime(1998, 3, 10),
                    RegistrationDate = new DateTime(2026, 01, 01)
                }
            );

            //Сид 10 Курсов (по 5 в каждой категории)
            for (int i = 1; i <= 10; i++)
            {
                modelBuilder.Entity<Course>().HasData(new Course
                {
                    Id = i,
                    Title = i % 2 == 0 ? $"Advanced C# Patterns Vol. {i}" : $"Modern React Guide Vol. {i}",
                    ShortDescription = "Learn the best practices in this comprehensive course.",
                    FullDescription = "Detailed description of the course with modules and deep dives.",
                    Price = 49.99m + i,
                    OldPrice = 99.99m,
                    ImagePath = $"course-{i}.png",
                    LevelForStudent = i < 5 ? Level.Beginner : Level.Intermediate,
                    Duration = TimeSpan.FromHours(10 + i),
                    LessonsCount = 12 + i,
                    CategoryId = (i <= 5) ? 1 : 2,
                    InstructorId = (i % 2 == 0) ? 1 : 2,
                    HasLifetimeAccess = true,
                    HasMobileAccess = true,
                    HasAssignments = true,
                    HasCommunityAccess = true,
                    HasDownloadableResources = true,
                    HasSubtitles = true
                });
            }

            //Сиды Студента
            modelBuilder.Entity<Student>().HasData(
            new Student { Id = 1, FirstName = "Ivan", LastName = "Tester", UserName = "ivan_test", Email = "ivan@test.com", PhoneNumber = "+1234567890", Gender = "Male", DateOfBirth = new DateTime(2000, 1, 1), RegistrationDate = new DateTime(2026, 1, 1), Bio = "Learning C#", AvatarPath = "student-1.png" },
            new Student { Id = 2, FirstName = "Maria", LastName = "Sokolova", UserName = "maria_dev", Email = "maria@example.com", PhoneNumber = "+1234567891", Gender = "Female", DateOfBirth = new DateTime(1999, 3, 15), RegistrationDate = new DateTime(2026, 1, 5), Bio = "Frontend enthusiast", AvatarPath = "student-2.png" },
            new Student { Id = 3, FirstName = "Alexey", LastName = "Petrov", UserName = "alex_p", Email = "alex@example.com", PhoneNumber = "+1234567892", Gender = "Male", DateOfBirth = new DateTime(2001, 7, 22), RegistrationDate = new DateTime(2026, 1, 8), Bio = "Backend developer", AvatarPath = "student-3.png" },
            new Student { Id = 4, FirstName = "Olga", LastName = "Ivanova", UserName = "olga_i", Email = "olga@example.com", PhoneNumber = "+1234567893", Gender = "Female", DateOfBirth = new DateTime(1998, 11, 3), RegistrationDate = new DateTime(2026, 1, 10), Bio = "Fullstack learner", AvatarPath = "student-4.png" },
            new Student { Id = 5, FirstName = "Dmitry", LastName = "Kozlov", UserName = "dmitry_k", Email = "dmitry@example.com", PhoneNumber = "+1234567894", Gender = "Male", DateOfBirth = new DateTime(2002, 5, 18), RegistrationDate = new DateTime(2026, 1, 12), Bio = "JavaScript fan", AvatarPath = "student-5.png" },
            new Student { Id = 6, FirstName = "Elena", LastName = "Smirnova", UserName = "elena_s", Email = "elena@example.com", PhoneNumber = "+1234567895", Gender = "Female", DateOfBirth = new DateTime(1997, 9, 30), RegistrationDate = new DateTime(2026, 1, 15), Bio = "React developer", AvatarPath = "student-6.png" },
            new Student { Id = 7, FirstName = "Sergey", LastName = "Volkov", UserName = "sergey_v", Email = "sergey@example.com", PhoneNumber = "+1234567896", Gender = "Male", DateOfBirth = new DateTime(2000, 12, 7), RegistrationDate = new DateTime(2026, 2, 1), Bio = "Python & C#", AvatarPath = "student-7.png" },
            new Student { Id = 8, FirstName = "Anna", LastName = "Kuznetsova", UserName = "anna_k", Email = "anna2@example.com", PhoneNumber = "+1234567897", Gender = "Female", DateOfBirth = new DateTime(2001, 4, 25), RegistrationDate = new DateTime(2026, 2, 5), Bio = "UI/UX designer", AvatarPath = "student-8.png" },
            new Student { Id = 9, FirstName = "Pavel", LastName = "Morozov", UserName = "pavel_m", Email = "pavel@example.com", PhoneNumber = "+1234567898", Gender = "Male", DateOfBirth = new DateTime(1999, 8, 14), RegistrationDate = new DateTime(2026, 2, 10), Bio = "Game dev interested", AvatarPath = "student-9.png" },
            new Student { Id = 10, FirstName = "Tatiana", LastName = "Orlova", UserName = "tatiana_o", Email = "tatiana@example.com", PhoneNumber = "+1234567899", Gender = "Female", DateOfBirth = new DateTime(2002, 2, 28), RegistrationDate = new DateTime(2026, 2, 15), Bio = "Data Science student", AvatarPath = "student-10.png" },
            new Student { Id = 11, FirstName = "Nikolay", LastName = "Fedorov", UserName = "nikolay_f", Email = "nikolay@example.com", PhoneNumber = "+1234567810", Gender = "Male", DateOfBirth = new DateTime(1998, 6, 9), RegistrationDate = new DateTime(2026, 2, 20), Bio = "ASP.NET Core fan", AvatarPath = "student-11.png" },
            new Student { Id = 12, FirstName = "Ekaterina", LastName = "Popova", UserName = "ekaterina_p", Email = "ekaterina@example.com", PhoneNumber = "+1234567811", Gender = "Female", DateOfBirth = new DateTime(2000, 10, 16), RegistrationDate = new DateTime(2026, 3, 1), Bio = "Mobile developer", AvatarPath = "student-12.png" },
            new Student { Id = 13, FirstName = "Andrey", LastName = "Sidorov", UserName = "andrey_s", Email = "andrey@example.com", PhoneNumber = "+1234567812", Gender = "Male", DateOfBirth = new DateTime(2001, 1, 5), RegistrationDate = new DateTime(2026, 3, 5), Bio = "DevOps learner", AvatarPath = "student-13.png" },
            new Student { Id = 14, FirstName = "Yulia", LastName = "Vasilieva", UserName = "yulia_v", Email = "yulia@example.com", PhoneNumber = "+1234567813", Gender = "Female", DateOfBirth = new DateTime(1999, 7, 11), RegistrationDate = new DateTime(2026, 3, 10), Bio = "QA Automation", AvatarPath = "student-14.png" },
            new Student { Id = 15, FirstName = "Maxim", LastName = "Belov", UserName = "maxim_b", Email = "maxim@example.com", PhoneNumber = "+1234567814", Gender = "Male", DateOfBirth = new DateTime(2002, 4, 3), RegistrationDate = new DateTime(2026, 3, 15), Bio = "Cloud computing", AvatarPath = "student-15.png" }
        );


            // Сид Заказа
            modelBuilder.Entity<Order>().HasData(new Order
            {
                Id = 1,
                StudentId = 1,
                TotalAmount = 150.00m,
                Tax = 10.00m,
                OrderDate = new DateTime(2026, 06, 06),
                OrderStatus = "Completed",
                FirstName = "Ivan",
                LastName = "Tester",
                AddressLine1 = "Lenina st. 1",
                Country = "Russia",
                City = "Moscow",
                PaymentMethod = "Card",
                State = "MSK"
            });
        }
    }
}
