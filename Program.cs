using Microsoft.AspNetCore.Authentication.Cookies;
using TailorApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add MVC
builder.Services.AddControllersWithViews();

// Register repositories with DI
var connectionString = builder.Configuration.GetConnectionString("TailorShopDB")!;
builder.Services.AddScoped<ICustomerRepository>(_ => new CustomerRepository(connectionString));
builder.Services.AddScoped<ISizeRepository>(_ => new SizeRepository(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
