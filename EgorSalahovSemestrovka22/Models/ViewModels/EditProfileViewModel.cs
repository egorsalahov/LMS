using System.ComponentModel.DataAnnotations;

namespace EgorSalahovSemestrovka22.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Выберите пол")]
        public string Gender { get; set; }

        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$", ErrorMessage = "Неверный формат. Пример: +7 (999) 999-99-99")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Введите дату рождения")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string? Bio { get; set; }
    }
}
