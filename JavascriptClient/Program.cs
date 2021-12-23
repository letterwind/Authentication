var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddControllersWithViews();
//services.AddRazorRuntimeCompilation();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoint =>
{
    endpoint.MapDefaultControllerRoute();
});

app.Run();
