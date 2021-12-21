using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = "Cookie";
    config.DefaultChallengeScheme = "oidc";
    config.DefaultSignInScheme = "Cookie";
})
    .AddCookie("Cookie")
    .AddOpenIdConnect("oidc", config =>
    {
        config.ClientId = "client_id_mvc";
        config.ClientSecret = "client_secret_mvc";
        config.Authority = "https://localhost:44325/";
        
        config.SaveTokens = true;

        config.ResponseType = "code";
        
        config.ClaimActions.DeleteClaim("amr");
        config.ClaimActions.MapUniqueJsonKey("GGGFFF", "letter.boss");

        config.GetClaimsFromUserInfoEndpoint = true;

        config.Scope.Clear();
        config.Scope.Add("my.scope");
        config.Scope.Add("openid");
        config.Scope.Add("ApiOne");
        config.Scope.Add("ApiTwo");
        config.Scope.Add("offline_access");
    });

services.AddHttpClient();
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
