using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace server.Controllers
{
    public class OAuthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(
            string response_type // authorization flow type
            , string client_id // client's id
            , string redirect_uri // 
            , string scope // what info i want => email, tel
            , string state) // random string generated to confirm that we are going to the same cliet
        {
            var query = new QueryBuilder();
            query.Add("redirectUri", redirect_uri);
            query.Add("state", state);
            return View(model: query.ToString());
        }

        [HttpPost]
        public IActionResult Authorize(string username, string redirectUri, string state)
        {
            var code = "LALALALA";
            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);
            return Redirect($"{redirectUri}{query}");
        }

        public async Task<IActionResult> Token(
            string grant_type
            ,string code
            ,string redirect_uri
            ,string client_id
            ,string refresh_token)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "N124078806"),
                new Claim("name", "Letter"),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString())
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audience,
                claims,
                notBefore: DateTime.Now,
                expires: grant_type == "refresh_token" ? DateTime.Now.AddMinutes(5) : DateTime.Now.AddMilliseconds(1),
                signingCredentials);

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial",
                refresh_token = "RefreshTokenSampleValueLetterIsGood"
            };

            var responseJson = JsonConvert.SerializeObject(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(responseJson);
            await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);

            return Redirect(redirect_uri);
        }

        [Authorize]
        public IActionResult Validate()
        {
            if (Request.Query.TryGetValue("access_token", out var accessToken))
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
