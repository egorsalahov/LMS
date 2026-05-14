using System.ComponentModel.DataAnnotations;

namespace EgorSalahovSemestrovka22.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле Email обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email адреса")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}