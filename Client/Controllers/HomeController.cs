using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var serverResponse = await RefreshAccessTokenWrapper(() => SecuredGetRequest("https://localhost:44387/secret/index"));
            
            var apiResponse = await RefreshAccessTokenWrapper(() => SecuredGetRequest("https://localhost:44309/secret/index"));

            return View();
        }

        private async Task<HttpResponseMessage> SecuredGetRequest(string url)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync(url);

            return response;
        }

        public async Task<HttpResponseMessage> RefreshAccessTokenWrapper(Func<Task<HttpResponseMessage>> initialRequest)
        {
            var response = await initialRequest();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshAccessToken();
                response = await initialRequest();
            }

            return response;
        }

        private async Task RefreshAccessToken()
        {
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var refreshTokenClien = _httpClientFactory.CreateClient();

            var requestData = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken!
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44387/oauth/token")
            {
                Content = new FormUrlEncodedContent(requestData)
            };

            var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("letter:123456"));

            request.Headers.Add("Authorization", $"Basic {base64Credentials}");

            var response = await refreshTokenClien.SendAsync(request);

            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString)!;

            var newAccessToken = responseData.GetValueOrDefault("access_token")!;
            var newRefreshToken = responseData.GetValueOrDefault("refresh_token")!;

            var authInfo = await HttpContext.AuthenticateAsync("ClientCookie")!;
            authInfo.Properties!.UpdateTokenValue("access_token", newAccessToken);
            authInfo.Properties!.UpdateTokenValue("refresh_token", newRefreshToken);

            await HttpContext.SignInAsync("ClientCookie", authInfo.Principal!, authInfo.Properties);
        }

        public IActionResult Authenticate()
        {
            

            return RedirectToAction("Index");
        }
    }
}
