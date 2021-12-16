using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Authenticate()
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Letter"),
                new Claim(ClaimTypes.Email, "letter@aaa.ccc"),
                new Claim("Letter is a", "Great Man")
            };

            var adminClaim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Admin"), new Claim("Admin is ", "Boss")
            };

            var userIdentity = new ClaimsIdentity(userClaims, "User Identity");

            var adminIdentity = new ClaimsIdentity(adminClaim, "Admin Identity");

            var userPrincipal = new ClaimsPrincipal(new[] { userIdentity, adminIdentity });

            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");
        }
    }
}
