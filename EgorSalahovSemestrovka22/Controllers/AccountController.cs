using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.ViewModels;
using EgorSalahovSemestrovka22.Models.ViewModels.EgorSalahovSemestrovka22.Models.ViewModels;
using EgorSalahovSemestrovka22.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sem.Web.Services;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (result, student) = await _accountService.RegisterAsync(model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            await _accountService.AddStudentRoleAsync(student);
            var token = await _accountService.GenerateEmailConfirmationTokenAsync(student);
            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = student.Id, token }, Request.Scheme);

            try
            {
                await _accountService.SendConfirmationEmailAsync(student, confirmationLink);
            }
            catch
            {
                // логирование
            }

            TempData["SuccessMessage"] = "На ваш Email отправлено письмо с подтверждением. Проверьте почту!";
            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToAction("Index", "Home");

            var user = await _accountService.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _accountService.ConfirmEmailAsync(userId, token);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Email успешно подтверждён!";
                return RedirectToAction("SignIn");
            }
            TempData["ErrorMessage"] = "Ошибка подтверждения Email.";
            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public IActionResult SignIn() => View();

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _accountService.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Неверный Email или пароль.");
                return View(model);
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Email не подтверждён.");
                return View(model);
            }

            var result = await _accountService.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe);
            if (result.Succeeded)
            {
                if (await _accountService.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                if (await _accountService.IsInRoleAsync(user, "Instructor"))
                    return RedirectToAction("Dashboard", "Instructor");
                return RedirectToAction("MyProfile", "Student");
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Аккаунт заблокирован.");
                return View(model);
            }

            ModelState.AddModelError("", "Неверный Email или пароль.");
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BecomeInstructorConfirmed()
        {
            var user = await _accountService.FindByEmailAsync(User.Identity.Name);
            if (user == null) return RedirectToAction("SignIn");
            await _accountService.ChangeRoleToInstructorAsync(user);
            return RedirectToAction("Dashboard", "Instructor");
        }

        // ========== FORGOT PASSWORD ==========
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _accountService.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var token = await _accountService.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account",
                    new { email = user.Email, token }, Request.Scheme);

                await _accountService.SendPasswordResetEmailAsync(user, resetLink);
            }

            // Всегда показываем одно и то же сообщение (безопасность)
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

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _accountService.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Пользователь не найден.";
                return RedirectToAction("SignIn");
            }

            var result = await _accountService.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
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