using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer
{
    public static class Configurations
    {
        public static List<ApiResource> GetApi()
        {
            return new List<ApiResource>
            {
                new ApiResource() { Name = "ApiOne", Scopes = { "ApiOne" } },
                new ApiResource() { Name = "ApiTwo", Scopes = { "ApiTwo" } }
            };
        }

        public static List<Client> GetClients()
        {
            return new List<Client>
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
                    PostLogoutRedirectUris = { "https://localhost:44300/Home/Index" },
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
                    PostLogoutRedirectUris = { "https://localhost:44342/Home/Index" },
                    //AccessTokenLifetime = 1,

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
            };
        }

        public static List<ApiScope> GetApiScope()
        {
            return new List<ApiScope>()
            {
                new ApiScope { Name = "ApiOne" },
                new ApiScope { Name = "ApiTwo", UserClaims = { "letter.api.boss" } }
            };
        }

        public static List<IdentityResource> GetIdentityResource()
        {
            return new List<IdentityResource>
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
            };
        }
    }
}
