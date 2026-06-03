using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.Entities.Instructors;
using EgorSalahovSemestrovka22.Models.Entities.Orders;
using EgorSalahovSemestrovka22.Models.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EgorSalahovSemestrovka22.Data
{
    public class AppDbContext : IdentityDbContext<Student>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            //мок-данные (gpt)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Frontend Development", ImagePath = "cat-1.png", CourseCount = 5 },
                new Category { Id = 2, Name = "Backend Development", ImagePath = "cat-2.png", CourseCount = 5 }
            );

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

           
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(m => m.Sender)
                      .WithMany()
                      .HasForeignKey(m => m.SenderId)
                      .OnDelete(DeleteBehavior.Restrict); 

                entity.HasOne(m => m.Receiver)
                      .WithMany()
                      .HasForeignKey(m => m.ReceiverId)
                      .OnDelete(DeleteBehavior.Restrict); 
            });
        }
    }
}
