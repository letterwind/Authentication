var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "letter.cookie";
        config.LoginPath = "/Home/Authenticate/";
    });

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
