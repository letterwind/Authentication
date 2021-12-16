using Api;
using Api.AuthRequirements;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthentication("DefaultAuth")
    .AddScheme<AuthenticationSchemeOptions, CustomAuthencationHandler>("DefaultAuth", null);

services.AddAuthorization(config =>
{
    var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    var defaulPolicy = defaultAuthBuilder
        .AddRequirements(new JwtRequirement())
        .Build();

    config.DefaultPolicy = defaulPolicy;
});

services.AddHttpClient()
    .AddHttpContextAccessor();

services.AddScoped<IAuthorizationHandler, JwtRequirementHandler>();

services.AddControllers();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
