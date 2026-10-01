using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebGuestBook.Data;
using WebGuestBook.Models.User;
using WebGuestBook.Services;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<UserContext>(options => options.UseSqlite(connection));

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
                options.LoginPath = "/GuestBook/Login";
                options.LogoutPath = "/GuestBook/Logout";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
        });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ReviewService>();

builder.Services.AddControllersWithViews();

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
        pattern: "{controller=GuestBook}/{action=Index}/{id?}");

app.Run();