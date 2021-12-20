var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = "Cookie";
    config.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookie")
    .AddOpenIdConnect("oidc", config =>
    {
        config.ClientId = "client_id_mvc";
        config.ClientSecret = "client_secret_mvc";
        config.Authority = "https://localhost:44325/";
        
        config.SaveTokens = true;

        config.ResponseType = "code";
    });

services.AddControllersWithViews();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoint =>
{
    endpoint.MapDefaultControllerRoute();
});

app.Run();
