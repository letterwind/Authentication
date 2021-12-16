using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using System.Security.Claims;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                var identityResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (identityResult.Succeeded)
                {
                    return RedirectToAction(nameof(Secret));
                }
            }

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var identityUser = new IdentityUser(username);
            var identityResult = await _userManager.CreateAsync(identityUser, password);

            if (identityResult.Succeeded)
            {
                // generation of the email token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                var link = Url.Action(nameof(VerifyEmail), "Home", new { userId = identityUser.Id, code }, Request.Scheme, Request.Host.ToString());

                await _emailService.SendAsync("letter@test.com", "Verify Mail", $"<a href={link} >Verify Email</a>", true);

                return RedirectToAction(nameof(EmailVerification));
                //await _signInManager.SignInAsync(identityUser, isPersistent: false);
                //return RedirectToAction(nameof(Secret));
            }

            return View();
        }

        public IActionResult EmailVerification() => View();

        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest();
            }

            var emailResult = await _userManager.ConfirmEmailAsync(user, code);

            if (emailResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(Secret));
            }

            return BadRequest();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
