using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.ViewModels;
using EgorSalahovSemestrovka22.Models.ViewModels.EgorSalahovSemestrovka22.Models.ViewModels;
using EgorSalahovSemestrovka22.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sem.Web.Services;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Регистрация с некорректными данными: {Email}", model?.Email);
                return View(model);
            }

            _logger.LogInformation("Попытка регистрации: {Email}", model.Email);
            var (result, student) = await _accountService.RegisterAsync(model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                _logger.LogWarning("Ошибка регистрации {Email}: {Errors}", model.Email, result.Errors);
                return View(model);
            }

            await _accountService.AddStudentRoleAsync(student);
            var token = await _accountService.GenerateEmailConfirmationTokenAsync(student);
            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = student.Id, token }, Request.Scheme);

            try
            {
                await _accountService.SendConfirmationEmailAsync(student, confirmationLink);
                _logger.LogInformation("Письмо подтверждения отправлено: {Email}", student.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка отправки подтверждения для {Email}", student.Email);
            }

            TempData["SuccessMessage"] = "На ваш Email отправлено письмо с подтверждением. Проверьте почту!";
            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                _logger.LogWarning("Подтверждение email: отсутствуют параметры");
                return RedirectToAction("Index", "Home");
            }

            var user = await _accountService.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _accountService.ConfirmEmailAsync(userId, token);
            if (result.Succeeded)
            {
                _logger.LogInformation("Email подтверждён: {Email}", user.Email);
                TempData["SuccessMessage"] = "Email успешно подтверждён!";
            }
            else
            {
                _logger.LogWarning("Ошибка подтверждения email: {Email}", user.Email);
                TempData["ErrorMessage"] = "Ошибка подтверждения Email.";
            }
            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public IActionResult SignIn() => View();

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Попытка входа с некорректными данными");
                return View(model);
            }

            _logger.LogInformation("Попытка входа: {Email}", model.Email);
            var user = await _accountService.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Пользователь {Email} не найден", model.Email);
                ModelState.AddModelError("", "Неверный Email или пароль.");
                return View(model);
            }
            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Email не подтверждён: {Email}", model.Email);
                ModelState.AddModelError("", "Email не подтверждён.");
                return View(model);
            }

            var result = await _accountService.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe);
            if (result.Succeeded)
            {
                _logger.LogInformation("Вход выполнен: {Email}", user.Email);
                if (await _accountService.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                if (await _accountService.IsInRoleAsync(user, "Instructor"))
                    return RedirectToAction("Dashboard", "Instructor");
                return RedirectToAction("MyProfile", "Student");
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("Аккаунт {Email} заблокирован", user.Email);
                ModelState.AddModelError("", "Аккаунт заблокирован.");
                return View(model);
            }

            _logger.LogWarning("Неверный пароль для {Email}", user.Email);
            ModelState.AddModelError("", "Неверный Email или пароль.");
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Выход пользователя {User}", User.Identity?.Name);
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BecomeInstructorConfirmed()
        {
            var user = await _accountService.FindByEmailAsync(User.Identity?.Name);
            if (user == null) return RedirectToAction("SignIn");
            _logger.LogInformation("Студент {Email} становится инструктором", user.Email);
            await _accountService.ChangeRoleToInstructorAsync(user);
            return RedirectToAction("Dashboard", "Instructor");
        }

        // ========== FORGOT PASSWORD ==========
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            _logger.LogInformation("Запрос сброса пароля для {Email}", model.Email);
            var user = await _accountService.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await _accountService.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account",
                    new { email = user.Email, token }, Request.Scheme);
                await _accountService.SendPasswordResetEmailAsync(user, resetLink);
                _logger.LogInformation("Ссылка сброса пароля отправлена: {Email}", user.Email);
            }
            TempData["SuccessMessage"] = "Если такой Email зарегистрирован, на него отправлена ссылка для сброса пароля.";
            return RedirectToAction("SignIn");
        }

        // ========== RESET PASSWORD ==========
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Home");

            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _accountService.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Сброс пароля: пользователь {Email} не найден", model.Email);
                TempData["ErrorMessage"] = "Пользователь не найден.";
                return RedirectToAction("SignIn");
            }

            var result = await _accountService.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("Пароль успешно сброшен для {Email}", user.Email);
                TempData["SuccessMessage"] = "Пароль успешно изменён. Теперь вы можете войти.";
                return RedirectToAction("SignIn");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() => View();
    }
}