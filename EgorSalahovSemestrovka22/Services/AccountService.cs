using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.ViewModels;
using EgorSalahovSemestrovka22.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Sem.Web.Services
{
    public class AccountService
    {
        private readonly UserManager<Student> _userManager;
        private readonly SignInManager<Student> _signInManager;
        private readonly EmailService _emailService;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            UserManager<Student> userManager,
            SignInManager<Student> signInManager,
            EmailService emailService,
            ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<(IdentityResult result, Student student)> RegisterAsync(RegisterViewModel model)
        {
            _logger.LogInformation("Регистрация пользователя {Email}", model.Email);
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
            if (result.Succeeded)
                _logger.LogInformation("Пользователь {Email} зарегистрирован", model.Email);
            else
                _logger.LogWarning("Ошибка регистрации {Email}: {Errors}", model.Email, result.Errors);
            return (result, student);
        }

        public async Task AddStudentRoleAsync(Student student)
        {
            _logger.LogInformation("Добавление роли Student пользователю {Email}", student.Email);
            await _userManager.AddToRoleAsync(student, "Student");
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(Student student)
            => await _userManager.GenerateEmailConfirmationTokenAsync(student);

        public async Task<Student?> FindByIdAsync(string userId)
            => await _userManager.FindByIdAsync(userId);

        public async Task SendConfirmationEmailAsync(Student student, string confirmationLink)
        {
            _logger.LogInformation("Отправка подтверждения email для {Email}", student.Email);
            await _emailService.SendEmailAsync(
                student.Email,
                "Подтверждение регистрации на Dreams LMS",
                $@"<h2>Добро пожаловать, {student.FirstName}!</h2>
                   <p>Для подтверждения Email перейдите по ссылке:</p>
                   <p><a href='{confirmationLink}'>Подтвердить Email</a></p>");
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            _logger.LogInformation("Подтверждение email для пользователя {UserId}", userId);
            return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(userId), token);
        }

        public async Task<Student?> FindByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe)
        {
            _logger.LogInformation("Попытка входа пользователя {UserName}", userName);
            return await _signInManager.PasswordSignInAsync(userName, password, rememberMe, false);
        }

        public async Task<bool> IsInRoleAsync(Student user, string role)
            => await _userManager.IsInRoleAsync(user, role);

        public async Task LogoutAsync()
        {
            _logger.LogInformation("Выход пользователя");
            await _signInManager.SignOutAsync();
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Student user)
            => await _userManager.GeneratePasswordResetTokenAsync(user);

        public async Task<IdentityResult> ResetPasswordAsync(Student user, string token, string newPassword)
        {
            _logger.LogInformation("Сброс пароля для пользователя {Email}", user.Email);
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task ChangeRoleToInstructorAsync(Student user)
        {
            _logger.LogInformation("Смена роли на Instructor для {Email}", user.Email);
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, "Instructor");
            await _signInManager.RefreshSignInAsync(user);
        }

        public async Task SendPasswordResetEmailAsync(Student user, string resetLink)
        {
            _logger.LogInformation("Отправка ссылки сброса пароля для {Email}", user.Email);
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