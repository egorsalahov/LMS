using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.ViewModels;
using EgorSalahovSemestrovka22.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EgorSalahovSemestrovka22.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Student> _userManager;
        private readonly SignInManager<Student> _signInManager;
        private readonly EmailService _emailService;

        public AccountController(
            UserManager<Student> userManager,
            SignInManager<Student> signInManager,
            EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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
            {
                // Выдаём роль Student
                await _userManager.AddToRoleAsync(student, "Student");

                // Генерируем токен подтверждения Email
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(student);

                // Формируем ссылку подтверждения
                var confirmationLink = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = student.Id, token = token },
                    protocol: HttpContext.Request.Scheme);

                // Отправляем письмо
                await _emailService.SendEmailAsync(
                    student.Email,
                    "Подтверждение регистрации на Dreams LMS",
                    $@"
                        <h2>Добро пожаловать, {student.FirstName}!</h2>
                        <p>Для подтверждения Email перейдите по ссылке:</p>
                        <p><a href='{confirmationLink}'>Подтвердить Email</a></p>
                        <p>Если вы не регистрировались на сайте, проигнорируйте это письмо.</p>
                    ");

                TempData["SuccessMessage"] = "На ваш Email отправлено письмо с подтверждением. Проверьте почту!";

                // НЕ входим автоматически — пусть сначала подтвердит Email
                return RedirectToAction("SignIn");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"Пользователь не найден.");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Email успешно подтверждён! Теперь вы можете войти.";
                return RedirectToAction("SignIn");
            }

            TempData["ErrorMessage"] = "Ошибка подтверждения Email. Возможно, токен устарел.";
            return RedirectToAction("SignIn");
        }

        // GET: /Account/SignIn
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        // POST: /Account/SignIn
        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Находим пользователя по Email
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Неверный Email или пароль.");
                return View(model);
            }

            // Проверяем, подтверждён ли Email
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Email не подтверждён. Проверьте почту и перейдите по ссылке в письме.");
                return View(model);
            }

            // Логинимся по UserName (который равен Email)
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, "Instructor"))
                    return RedirectToAction("Dashboard", "Instructor");

                return RedirectToAction("MyProfile", "Student");
            }



            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Аккаунт заблокирован. Попробуйте позже.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Неверный Email или пароль.");
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // GET: /Account/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult BecomeInstructor()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BecomeInstructorConfirmed()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("SignIn");

            // Получаем текущие роли
            var roles = await _userManager.GetRolesAsync(user);

            // Удаляем все существующие роли
            await _userManager.RemoveFromRolesAsync(user, roles);

            // Добавляем только Instructor
            await _userManager.AddToRoleAsync(user, "Instructor");

            // Обновляем cookie
            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction("Dashboard", "Instructor");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}