using WebBarberShopBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using WebBarberShopBooking.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Thêm DbContext sử dụng SQL Server (hoặc provider khác)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Thêm Identity với User tùy chỉnh và hỗ trợ Roles
builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false) // Tắt xác nhận tài khoản email cho mục đích demo
   .AddEntityFrameworkStores<ApplicationDbContext>()
   .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IServiceRepository, EFServiceRepository>();
builder.Services.AddRazorPages();

builder.Services.AddTransient<IEmailSender, NoOpEmailSender>();

var app = builder.Build();

// Cấu hình HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Thêm middleware xác thực
app.UseAuthorization();  // Thêm middleware ủy quyền

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Để hỗ trợ các trang Razor của Identity

// Tạo scope để truy cập các dịch vụ
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

    // Tạo database nếu chưa tồn tại và áp dụng migrations
    await dbContext.Database.MigrateAsync();

    // Tạo role "Admin" nếu chưa tồn tại
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
        Console.WriteLine("Role 'Admin' created successfully.");
    }

    // Tìm user theo email "admin@gmail.com" và gán quyền Admin
    var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");

    if (adminUser != null)
    {
        // Gán role "Admin" cho user nếu chưa có
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            var result = await userManager.AddToRoleAsync(adminUser, "Admin");
            if (result.Succeeded)
            {
                Console.WriteLine($"User with email 'admin@gmail.com' added to 'Admin' role.");
            }
            else
            {
                Console.WriteLine($"Failed to add user with email 'admin@gmail.com' to 'Admin' role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            Console.WriteLine($"User with email 'admin@gmail.com' is already in 'Admin' role.");
        }
    }
    else
    {
        Console.WriteLine("Admin user with email 'admin@gmail.com' not found. Make sure a user with this email exists.");
    }
}

app.Run();