using IdentityExample.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddDbContext<AppDbContext>(config =>
{
    config.UseInMemoryDatabase("Memory");
});


services.AddMailKit(option =>
{
    var emailKitOption = new MailKitOptions();
    emailKitOption.SenderEmail = "test@ttest.com";
    emailKitOption.Port = 25;
    emailKitOption.Server = "127.0.0.1";
    emailKitOption.SenderName = "Letter";

    option.UseMailKit(emailKitOption);
});

// register Identity services
services.AddIdentity<IdentityUser, IdentityRole>(config =>
{
    config.Password.RequiredLength = 4;
    config.Password.RequireDigit = false;
    config.Password.RequireLowercase = false;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireUppercase = false;
    config.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

services.ConfigureApplicationCookie(config =>
{
    config.Cookie.Name = "Identity.cookie";
    config.LoginPath = "/Home/Login";
    
});

//services.AddAuthentication("CookieAuth")
//    .AddCookie("CookieAuth", config =>
//    {
//        config.Cookie.Name = "letter.cookie";
//        config.LoginPath = "/Home/Authenticate/";
//    });

services.AddControllersWithViews();

var app = builder.Build();

app.UseRouting();

// who you are?
app.UseAuthentication();

// are you allowed?
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

//app.MapGet("/", () => "Hello World!");

app.Run();
