using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EgorSalahovSemestrovka22.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly UserManager<Student> _userManager;

        public StudentController(UserManager<Student> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("SignIn", "Account");
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Вернёмся на страницу профиля с ошибками
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("SignIn", "Account");
                // Передадим модель с ошибками, а также самого user для отображения (необязательно)
                ViewData["EditMode"] = true; // чтобы форма была показана
                return View("MyProfile", user);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("SignIn", "Account");

            currentUser.FirstName = model.FirstName;
            currentUser.LastName = model.LastName;
            currentUser.Gender = model.Gender;
            currentUser.PhoneNumber = model.PhoneNumber;
            currentUser.DateOfBirth = model.DateOfBirth;
            currentUser.Bio = model.Bio;

            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Профиль успешно обновлён!";
                return RedirectToAction("MyProfile");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View("MyProfile", currentUser);
        }
        public IActionResult Courses() => View();
        public IActionResult Wishlist() => View();
        public IActionResult OrderHistory() => View();
        public IActionResult Settings() => View();
        public IActionResult BecomeInstructor() => View();
    }
}
