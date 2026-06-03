using EgorSalahovSemestrovka22.Models.Entities.Instructors;
using EgorSalahovSemestrovka22.Models.Enums;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace EgorSalahovSemestrovka22.Models.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? ImagePath { get; set; }

        public Level LevelForStudent { get; set; }
        public TimeSpan Duration { get; set; }
        public int LessonsCount { get; set; }


        public int CategoryId { get; set; }
        public Category Category { get; set; } 

        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; } 

        public ICollection<Section> Sections { get; set; } 
        public ICollection<Review> Reviews { get; set; } 
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();


        public bool HasLifetimeAccess { get; set; } = true;
        public bool HasMobileAccess { get; set; } = true;
        public bool HasAssignments { get; set; }
        public bool HasCommunityAccess { get; set; } 
        public bool HasDownloadableResources { get; set; } 
        public bool HasSubtitles { get; set; } 
    }
}
