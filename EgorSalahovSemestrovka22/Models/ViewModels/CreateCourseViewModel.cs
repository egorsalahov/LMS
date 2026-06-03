using EgorSalahovSemestrovka22.Models.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace EgorSalahovSemestrovka22.Models.ViewModels
{
    public class CreateCourseViewModel
    {
        [Required(ErrorMessage = "Введите название курса")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Выберите уровень")]
        [Display(Name = "Level")]
        public Level? LevelForStudent { get; set; }

        [Required(ErrorMessage = "Введите краткое описание")]
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }

        [Required(ErrorMessage = "Введите полное описание")]
        [Display(Name = "Full Description")]
        public string FullDescription { get; set; }

        [Display(Name = "Course Image")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Free Course")]
        public bool IsFree { get; set; }

        [Display(Name = "Price ($)")]
        [Range(0, double.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
        public decimal? Price { get; set; }

        [Display(Name = "Discount Price ($)")]
        [Range(0, double.MaxValue, ErrorMessage = "Скидка не может быть отрицательной")]
        public decimal? OldPrice { get; set; }


        [Display(Name = "Lifetime Access")]
        public bool HasLifetimeAccess { get; set; } = true;

        [Display(Name = "Mobile Access")]
        public bool HasMobileAccess { get; set; } = true;

        [Display(Name = "Assignments")]
        public bool HasAssignments { get; set; }

        [Display(Name = "Community Access")]
        public bool HasCommunityAccess { get; set; }

        [Display(Name = "Downloadable Resources")]
        public bool HasDownloadableResources { get; set; }

        [Display(Name = "Subtitles")]
        public bool HasSubtitles { get; set; }

        [ValidateNever]
        public List<SectionViewModel> Sections { get; set; } = new();

        public class SectionViewModel
        {
            public string Title { get; set; }
            public List<LessonViewModel> Lessons { get; set; } = new();
        }

        public class LessonViewModel
        {
            public string Title { get; set; }
            public string? VideoLink { get; set; }
            public IFormFile? VideoFile { get; set; }
        }
    }
}
