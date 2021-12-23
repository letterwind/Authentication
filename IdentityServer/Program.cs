using System.Security.Claims;
using IdentityModel;
using IdentityServer.Data;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddDbContext<AppDbContext>(config =>
{
    config.UseInMemoryDatabase("Memory");
});

// register Identity services
services.AddIdentity<IdentityUser, IdentityRole>(config =>
    {
        config.Password.RequiredLength = 4;
        config.Password.RequireDigit = false;
        config.Password.RequireLowercase = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
        //config.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

services.ConfigureApplicationCookie(config =>
{
    config.Cookie.Name = "IdentityServer.Cookie";
    config.LoginPath = "/Auth/Login";

});

services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddInMemoryApiResources(new List<ApiResource>
    {
        new ApiResource(){Name = "ApiOne", Scopes = {"ApiOne" } },
        new ApiResource(){Name = "ApiTwo", Scopes = {"ApiTwo"} }
    })
    .AddInMemoryClients(new List<Client>
    {
        new Client
        {
            ClientId = "client_id",
            ClientSecrets = { new Secret("client_secret".ToSha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AllowedScopes = { "ApiOne" }
        },
        new Client
        {
            ClientId = "client_id_mvc",
            ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },
            AllowedGrantTypes = GrantTypes.Code,
            //AlwaysIncludeUserClaimsInIdToken = true,
            RedirectUris = { "https://localhost:44300/signin-oidc" },
            AllowedScopes =
            {
                "ApiOne", 
                "ApiTwo", 
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "my.scope"
            },
            AllowOfflineAccess = true
        },
        new Client
        {
            ClientId = "client_id_js",
            AllowedCorsOrigins = { "https://localhost:44342" },
            AllowedGrantTypes = GrantTypes.Implicit,
            RedirectUris = { "https://localhost:44342/signin" },
            AccessTokenLifetime = 1,
            
            AllowedScopes =
            {
                "ApiOne",
                "ApiTwo",
                IdentityServerConstants.StandardScopes.OpenId,
                "my.scope"
            },
            AllowAccessTokensViaBrowser = true,
            RequireConsent = false
        }
    })
    .AddInMemoryApiScopes(new List<ApiScope>()
    {
        new ApiScope{ Name = "ApiOne"},
        new ApiScope{ Name = "ApiTwo", UserClaims = {"letter.api.boss"} }
    })
    .AddInMemoryIdentityResources(new List<IdentityResource>()
    {
        new IdentityResources.OpenId(),
        //new IdentityResources.Profile(),
        new IdentityResource
        {
            Name = "my.scope",
            UserClaims =
            {
                "letter.boss"
            }
        }
    })
    .AddDeveloperSigningCredential();


services.AddControllersWithViews();

var app = builder.Build();


app.UseIdentityServer();

app.UseRouting();
app.UseEndpoints(config =>
{
    config.MapDefaultControllerRoute();
});

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var user = new IdentityUser("letter");
    userManager.CreateAsync(user, "password").GetAwaiter().GetResult();
    userManager.AddClaimAsync(user, new Claim("letter.boss", "big.cookies")).GetAwaiter().GetResult();
    userManager.AddClaimAsync(user, new Claim("letter.api.boss", "big.api.cookies")).GetAwaiter().GetResult();
}

app.Run();
