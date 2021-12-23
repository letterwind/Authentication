using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace JavascriptClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("/signin")]
        public async Task<IActionResult> SignIn()
        {
            //var accessToken = await HttpContext.GetTokenAsync("access_token");
            return View();
        }
    }
}
