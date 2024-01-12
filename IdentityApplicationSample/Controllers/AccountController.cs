using IdentityApplicationSample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApplicationSample.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {


        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;

        //Login islemleri için
        public AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager)
        {
            this.userManager = _userManager;
            this.signInManager = _signInManager;
        }

        [AllowAnonymous] //herkese izin ver

        public IActionResult Login(string returnUrl)
        {
            Login login = new Login();
            login.ReturnUrl = returnUrl;
            return View(login);


        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {

                AppUser appUser = await userManager.FindByEmailAsync(login.Email);



                if (appUser != null)

                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, false, false);

                    //ilk false beni hatırladaki yapı için
                    if (result.Succeeded)
                        return RedirectToAction("Index","Home");

                }
                ModelState.AddModelError(nameof(login.Email), "Login Failed : Email or password wrong");


            }
            return View(login);

        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");



        }
        public IActionResult AccesDenied()
        {

            return View();  
        }




        public IActionResult Index()
        {
            return View();
        }
    }
}
