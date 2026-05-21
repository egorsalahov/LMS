namespace EgorSalahovSemestrovka22.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    namespace EgorSalahovSemestrovka22.Models.ViewModels
    {
        public class ForgotPasswordViewModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string Email { get; set; }
        }
    }
}
