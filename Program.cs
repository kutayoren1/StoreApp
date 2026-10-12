using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MyAspNetCoreApp.Web.Models;
using StoreApp.Services;

var builder = WebApplication.CreateBuilder(args);
// 1. Kimlik doğrulama servisini projeye dahil ediyoruz ve Cookie şemasını seçiyoruz
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Kullanıcı giriş yapmamışsa yönlendirileceği adres
        options.LoginPath = "/cms/login";

        // Yetkisi olmayan bir yere girmeye çalışırsa yönlendirileceği adres (isteğe bağlı)
        options.AccessDeniedPath = "/cms/login";

        // Çerezin tarayıcıda kalacağı süre (Örn: 60 dakika)
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddDbContext<AppDbContext>(Options =>
{
    Options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
