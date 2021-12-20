﻿using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using IdentityModel.Client;

namespace ApiTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("/")]
        public async Task<IActionResult> Index()
        {
            // retrieve access token
            var serverClient = _httpClientFactory.CreateClient();

            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44325/");

            var tokenResponse = await serverClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "client_id",
                ClientSecret = "client_secret",
                Scope = "ApiOne"
            });

            // retrieve secret data
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);
            var apiResponse = await apiClient.GetAsync("https://localhost:44351/secret");
            var content = await apiResponse.Content.ReadAsStringAsync();

            return Ok(new
            {
                access_token = tokenResponse.AccessToken,
                message = content
            });
        }
    }
}
