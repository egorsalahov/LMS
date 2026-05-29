using EgorSalahovSemestrovka22.Data;
using EgorSalahovSemestrovka22.Hubs;
using EgorSalahovSemestrovka22.Middlewares;
using EgorSalahovSemestrovka22.Models;
using EgorSalahovSemestrovka22.Models.Entities;
using EgorSalahovSemestrovka22.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sem.Web.Areas.Admin.Repositories;
using Sem.Web.Areas.Admin.Repositories.Interfaces;
using Sem.Web.Areas.Admin.Services;
using Sem.Web.Repositories;
using Sem.Web.Repositories.Interfaces;
using Sem.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

//репозитории
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

//сервисы
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<HomeService>();
builder.Services.AddScoped<InstructorService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<AdminService>();



// Сервисы
builder.Services.AddScoped<AdminService>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 200 * 1024 * 1024; // 200 MB видео лимит
});

//Identity
builder.Services.AddIdentity<Student, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

//Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

builder.Services.AddSignalR();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Обработка 404 и других кодов ошибок
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

//свой middleware
app.UseNoCache();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


// Сид пользователей (студентов)
// Сид пользователей
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Student>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // 1. Сид ролей
    string[] roles = { "Admin", "Instructor", "Student" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // 2. Сид админа (всегда)
    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new Student
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "Adminov",
            EmailConfirmed = true,
            PhoneNumber = "+70000000000",
            Gender = "Male",
            RegistrationDate = new DateTime(2026, 1, 1),
            DateOfBirth = new DateTime(1990, 1, 1),
            Bio = "System Administrator",
            AvatarPath = "admin-avatar.png"
        };
        var adminResult = await userManager.CreateAsync(adminUser, "Admin123!");
        if (adminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    else
    {
        // Если админ уже есть, но без роли — выдаём
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // 3. Сид студентов (если таблица была пуста)
    if (userManager.Users.Count() <= 1) // Только админ существует
    {
        var students = new List<(Student, string)>
        {
            (new Student { FirstName = "Ivan", LastName = "Tester", UserName = "ivan_test", Email = "ivan@test.com", PhoneNumber = "+1234567890", Gender = "Male", DateOfBirth = new DateTime(2000, 1, 1), RegistrationDate = new DateTime(2026, 1, 1), Bio = "Learning C#", AvatarPath = "student-1.png" }, "Password123"),
            (new Student { FirstName = "Maria", LastName = "Sokolova", UserName = "maria_dev", Email = "maria@example.com", PhoneNumber = "+1234567891", Gender = "Female", DateOfBirth = new DateTime(1999, 3, 15), RegistrationDate = new DateTime(2026, 1, 5), Bio = "Frontend enthusiast", AvatarPath = "student-2.png" }, "Password123"),
            (new Student { FirstName = "Alexey", LastName = "Petrov", UserName = "alex_p", Email = "alex@example.com", PhoneNumber = "+1234567892", Gender = "Male", DateOfBirth = new DateTime(2001, 7, 22), RegistrationDate = new DateTime(2026, 1, 8), Bio = "Backend developer", AvatarPath = "student-3.png" }, "Password123"),
            (new Student { FirstName = "Olga", LastName = "Ivanova", UserName = "olga_i", Email = "olga@example.com", PhoneNumber = "+1234567893", Gender = "Female", DateOfBirth = new DateTime(1998, 11, 3), RegistrationDate = new DateTime(2026, 1, 10), Bio = "Fullstack learner", AvatarPath = "student-4.png" }, "Password123"),
            (new Student { FirstName = "Dmitry", LastName = "Kozlov", UserName = "dmitry_k", Email = "dmitry@example.com", PhoneNumber = "+1234567894", Gender = "Male", DateOfBirth = new DateTime(2002, 5, 18), RegistrationDate = new DateTime(2026, 1, 12), Bio = "JavaScript fan", AvatarPath = "student-5.png" }, "Password123"),
            (new Student { FirstName = "Elena", LastName = "Smirnova", UserName = "elena_s", Email = "elena@example.com", PhoneNumber = "+1234567895", Gender = "Female", DateOfBirth = new DateTime(1997, 9, 30), RegistrationDate = new DateTime(2026, 1, 15), Bio = "React developer", AvatarPath = "student-6.png" }, "Password123"),
            (new Student { FirstName = "Sergey", LastName = "Volkov", UserName = "sergey_v", Email = "sergey@example.com", PhoneNumber = "+1234567896", Gender = "Male", DateOfBirth = new DateTime(2000, 12, 7), RegistrationDate = new DateTime(2026, 2, 1), Bio = "Python & C#", AvatarPath = "student-7.png" }, "Password123"),
            (new Student { FirstName = "Anna", LastName = "Kuznetsova", UserName = "anna_k", Email = "anna2@example.com", PhoneNumber = "+1234567897", Gender = "Female", DateOfBirth = new DateTime(2001, 4, 25), RegistrationDate = new DateTime(2026, 2, 5), Bio = "UI/UX designer", AvatarPath = "student-8.png" }, "Password123"),
            (new Student { FirstName = "Pavel", LastName = "Morozov", UserName = "pavel_m", Email = "pavel@example.com", PhoneNumber = "+1234567898", Gender = "Male", DateOfBirth = new DateTime(1999, 8, 14), RegistrationDate = new DateTime(2026, 2, 10), Bio = "Game dev interested", AvatarPath = "student-9.png" }, "Password123"),
            (new Student { FirstName = "Tatiana", LastName = "Orlova", UserName = "tatiana_o", Email = "tatiana@example.com", PhoneNumber = "+1234567899", Gender = "Female", DateOfBirth = new DateTime(2002, 2, 28), RegistrationDate = new DateTime(2026, 2, 15), Bio = "Data Science student", AvatarPath = "student-10.png" }, "Password123"),
            (new Student { FirstName = "Nikolay", LastName = "Fedorov", UserName = "nikolay_f", Email = "nikolay@example.com", PhoneNumber = "+1234567810", Gender = "Male", DateOfBirth = new DateTime(1998, 6, 9), RegistrationDate = new DateTime(2026, 2, 20), Bio = "ASP.NET Core fan", AvatarPath = "student-11.png" }, "Password123"),
            (new Student { FirstName = "Ekaterina", LastName = "Popova", UserName = "ekaterina_p", Email = "ekaterina@example.com", PhoneNumber = "+1234567811", Gender = "Female", DateOfBirth = new DateTime(2000, 10, 16), RegistrationDate = new DateTime(2026, 3, 1), Bio = "Mobile developer", AvatarPath = "student-12.png" }, "Password123"),
            (new Student { FirstName = "Andrey", LastName = "Sidorov", UserName = "andrey_s", Email = "andrey@example.com", PhoneNumber = "+1234567812", Gender = "Male", DateOfBirth = new DateTime(2001, 1, 5), RegistrationDate = new DateTime(2026, 3, 5), Bio = "DevOps learner", AvatarPath = "student-13.png" }, "Password123"),
            (new Student { FirstName = "Yulia", LastName = "Vasilieva", UserName = "yulia_v", Email = "yulia@example.com", PhoneNumber = "+1234567813", Gender = "Female", DateOfBirth = new DateTime(1999, 7, 11), RegistrationDate = new DateTime(2026, 3, 10), Bio = "QA Automation", AvatarPath = "student-14.png" }, "Password123"),
            (new Student { FirstName = "Maxim", LastName = "Belov", UserName = "maxim_b", Email = "maxim@example.com", PhoneNumber = "+1234567814", Gender = "Male", DateOfBirth = new DateTime(2002, 4, 3), RegistrationDate = new DateTime(2026, 3, 15), Bio = "Cloud computing", AvatarPath = "student-15.png" }, "Password123")
        };

        foreach (var (student, password) in students)
        {
            var result = await userManager.CreateAsync(student, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(student, "Student");
            }
        }
    }
    else
    {
        // 4. Выдать роль Student существующим, у кого её нет
        var allUsers = userManager.Users.ToList();
        foreach (var user in allUsers)
        {
            if (!await userManager.IsInRoleAsync(user, "Student") &&
                !await userManager.IsInRoleAsync(user, "Instructor") &&
                !await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Student");
            }
        }
    }
}

app.Run();
