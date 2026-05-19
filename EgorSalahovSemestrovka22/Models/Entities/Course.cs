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
        public decimal? OldPrice { get; set; } //для скидки
        public string? ImagePath { get; set; }

        public Level LevelForStudent { get; set; }
        public TimeSpan Duration { get; set; } // Общая длительность (9h 30min)
        public int LessonsCount { get; set; } // Chapters/Lessons

        // Связи
        public int CategoryId { get; set; }
        public Category Category { get; set; } //Course Category Page 

        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; } 

        public ICollection<Section> Sections { get; set; } // Для Course Content (программа курса)
        public ICollection<Review> Reviews { get; set; } //звездочки (с возможностью расширения до комментов если добавить string Comment)

        //Includes (Course Detail Page)
        public bool HasLifetimeAccess { get; set; } = true;
        public bool HasMobileAccess { get; set; } = true;
        public bool HasAssignments { get; set; } //Наличие упражнений
        public bool HasCommunityAccess { get; set; } //Доступ к закрытому комьюнити (чату)
        public bool HasDownloadableResources { get; set; } //Дополнительные файлы для скачивания (исходники, PDF)
        public bool HasSubtitles { get; set; } //Субтитры
    }
}
