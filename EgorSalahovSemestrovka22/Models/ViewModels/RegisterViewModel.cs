using System.ComponentModel.DataAnnotations;

namespace EgorSalahovSemestrovka22.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле Полное имя обязательно для заполнения")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Поле Email обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email адреса")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле Пароль обязательно для заполнения")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Пароль должен быть не менее {2} символов.", MinimumLength = 6)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Вы должны согласиться с условиями использования")]
        public bool IsAgreed { get; set; }
    }
}