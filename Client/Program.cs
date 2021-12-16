using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication(config =>
{
    // 登入驗證用 cookie
    config.DefaultAuthenticateScheme = "ClientCookie";
    // 登入後要移除的cookie
    config.DefaultSignInScheme = "ClientCookie";
    // 檢查權限用cookie
    config.DefaultChallengeScheme = "OurServer";
})
    .AddCookie("ClientCookie")
    .AddOAuth("OurServer", config =>
    {
        config.ClientId = "client_id";
        config.ClientSecret = "client_sercret";
        config.CallbackPath = "/oauth/callback";
        config.AuthorizationEndpoint = "https://localhost:44387/oauth/authorize";
        config.TokenEndpoint = "https://localhost:44387/oauth/token";

        config.SaveTokens = true;

        config.Events = new OAuthEvents()
        {
            OnCreatingTicket = context =>
            {
                var token = context.AccessToken;
                if(token != null)
                {
                    var tokens = token.Split('.');
                    var payloadBase64 = tokens[1];
                    var bytes = Convert.FromBase64String(payloadBase64);
                    var payloadJson = System.Text.Encoding.UTF8.GetString(bytes);
                    var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(payloadJson);
                    if(claims != null)
                    {
                        foreach (var claim in claims)
                        {
                            context.Identity?.AddClaim(new Claim(claim.Key, claim.Value));
                        }
                    }
                    
                }
                

                return Task.CompletedTask;
            }
        };
    });

services.AddHttpClient();

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
app.Run();
