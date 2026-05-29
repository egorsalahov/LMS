using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.ViewModels;
using EgorSalahovSemestrovka22.Services;
using Microsoft.AspNetCore.Identity;

namespace Sem.Web.Services
{
    public class AccountService
    {
        private readonly UserManager<Student> _userManager;
        private readonly SignInManager<Student> _signInManager;
        private readonly EmailService _emailService;

        public AccountService(
            UserManager<Student> userManager,
            SignInManager<Student> signInManager,
            EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        // Регистрация
        public async Task<(IdentityResult result, Student student)> RegisterAsync(RegisterViewModel model)
        {
            var student = new Student
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FullName,
                LastName = "",
                Gender = "",
                Bio = "",
                AvatarPath = "default-avatar.png",
                RegistrationDate = DateTime.Now,
                DateOfBirth = DateTime.Now
            };

            var result = await _userManager.CreateAsync(student, model.Password);
            return (result, student);
        }

        //роль студента
        public async Task AddStudentRoleAsync(Student student)
            => await _userManager.AddToRoleAsync(student, "Student");

        public async Task<string> GenerateEmailConfirmationTokenAsync(Student student)
            => await _userManager.GenerateEmailConfirmationTokenAsync(student);

        public async Task<Student?> FindByIdAsync(string userId)
    => await _userManager.FindByIdAsync(userId);

        public async Task SendConfirmationEmailAsync(Student student, string confirmationLink)
        {
            await _emailService.SendEmailAsync(
                student.Email,
                "Подтверждение регистрации на Dreams LMS",
                $@"<h2>Добро пожаловать, {student.FirstName}!</h2>
                   <p>Для подтверждения Email перейдите по ссылке:</p>
                   <p><a href='{confirmationLink}'>Подтвердить Email</a></p>");
        }

        // Подтверждение Email
        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
            => await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(userId), token);

        // Вход
        public async Task<Student?> FindByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe)
            => await _signInManager.PasswordSignInAsync(userName, password, rememberMe, lockoutOnFailure: false);

        public async Task<bool> IsInRoleAsync(Student user, string role)
            => await _userManager.IsInRoleAsync(user, role);

        // Выход
        public async Task LogoutAsync()
            => await _signInManager.SignOutAsync();

        // Сброс пароля
        public async Task<string> GeneratePasswordResetTokenAsync(Student user)
            => await _userManager.GeneratePasswordResetTokenAsync(user);

        public async Task<IdentityResult> ResetPasswordAsync(Student user, string token, string newPassword)
            => await _userManager.ResetPasswordAsync(user, token, newPassword);

        // Стать инструктором
        public async Task ChangeRoleToInstructorAsync(Student user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, "Instructor");
            await _signInManager.RefreshSignInAsync(user);
        }
        public async Task SendPasswordResetEmailAsync(Student user, string resetLink)
        {
            await _emailService.SendEmailAsync(
                user.Email,
                "Сброс пароля на Dreams LMS",
                $@"<h2>Сброс пароля</h2>
           <p>Для сброса пароля перейдите по ссылке:</p>
           <p><a href='{resetLink}'>Сбросить пароль</a></p>
           <p>Если вы не запрашивали сброс пароля, проигнорируйте это письмо.</p>");
        }
    }
}
