using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Api.AuthRequirements
{
    public class JwtRequirement : IAuthorizationRequirement{ }

    public class JwtRequirementHandler : AuthorizationHandler<JwtRequirement>
    {
        private readonly HttpClient _httpClient;
        private readonly HttpContext? _httpContext;

        public JwtRequirementHandler(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpContext = contextAccessor.HttpContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            JwtRequirement requirement)
        {
            if (_httpContext!.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var accessToken = authHeader.ToString().Split(" ")[1];
                var response =
                    await _httpClient.GetAsync($"https://localhost:44387/oauth/validate?access_token={accessToken}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
                
        }
    }
}
