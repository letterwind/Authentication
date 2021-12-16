using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class SecretController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return Ok("Secret");
        }
    }
}
