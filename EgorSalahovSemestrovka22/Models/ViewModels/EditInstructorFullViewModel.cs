using System.ComponentModel.DataAnnotations;

namespace EgorSalahovSemestrovka22.Models.ViewModels
{
    public class EditInstructorFullViewModel
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Gender { get; set; }
        [Phone] public string? PhoneNumber { get; set; }
        [Required, DataType(DataType.Date)] public DateTime DateOfBirth { get; set; }
        public string? Bio { get; set; }
        public List<EducationItem>? Educations { get; set; }
        public List<ExperienceItem>? Experiences { get; set; }
    }

    public class EducationItem
    {
        public string? Degree { get; set; }
        public string? Institute { get; set; }
        public string? Years { get; set; }
    }

    public class ExperienceItem
    {
        public string? Position { get; set; }
        public string? Company { get; set; }
        public string? Years { get; set; }
    }
}
