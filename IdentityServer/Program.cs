using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddIdentityServer()
    .AddInMemoryApiResources(new List<ApiResource>
    {
        new ApiResource(){Name = "ApiOne", Scopes = {"ApiOne"} }
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
            AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
            RedirectUris = { "https://localhost:44300/signin-oidc" },
            AllowedScopes = { "ApiOne", "ApiTwo", IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile }
        }
    })
    .AddInMemoryApiScopes(new List<ApiScope>()
    {
        new ApiScope{ Name = "ApiOne" },
        new ApiScope{ Name = "ApiTwo" }
    })
    .AddInMemoryIdentityResources(new List<IdentityResource>()
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
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

app.Run();
