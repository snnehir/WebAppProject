using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ShoppingApp.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using ShoppingApp.Helpers;
using ShoppingApp.Models.Entities;

namespace ShoppingApp.Controllers
{
    public class AccountController : Controller
    {
        ShoppingContext _shoppingContext;
        public AccountController(ShoppingContext shoppingContext)
        {
            _shoppingContext = shoppingContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login(string? redirectUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(redirectUrl))
                    return Redirect(redirectUrl);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                ViewBag.RedirectUrl = redirectUrl;
                return View();
            }

        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel model, string? redirectUrl)
        {
            ViewBag.ErrorMessage = "";
            if (ModelState.IsValid)
            {
                var isValidEmail = Validation.IsValidEmail(model.Email);
                if (!isValidEmail)
                {
                    ViewBag.ErrorMessage = "Invalid email.";
                    return View();
                }
                var user = await _shoppingContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(model.Email));
                if (user == null)
                {
                    ViewBag.ErrorMessage = "No user found with this email.";
                    return View();
                }
                var passwordHash = SecurityHelper.HashPassword(model.Password, user.PasswordSalt);

                if (!user.PasswordHash.Equals(passwordHash))
                {
                    ViewBag.ErrorMessage = "Email and password are not matching.";
                    return View();
                }
                // successful login & role based cookie authentication
                Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                };
                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);
                if (!string.IsNullOrEmpty(redirectUrl) && Url.IsLocalUrl(redirectUrl))
                {
                    return Redirect(redirectUrl);
                }
                return Redirect("/");
            }
            return View();
        }
        [AllowAnonymous]
        public IActionResult SignUp(string? redirectUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(redirectUrl))
                    return Redirect(redirectUrl);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                ViewBag.RedirectUrl = redirectUrl;
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserSignUpViewModel model, string? redirectUrl)
        {
            ViewBag.ErrorMessage = "";
            if (ModelState.IsValid)
            {
                var isValidEmail = Validation.IsValidEmail(model.Email);
                if (!isValidEmail)
                {
                    ViewBag.ErrorMessage = "Invalid email.";
                    return View();
                }
                var user = await _shoppingContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(model.Email));
                if (user != null)
                {
                    ViewBag.ErrorMessage = "This email is already used.";
                    return View();
                }
                bool isValidPassword = Validation.IsValidPassword(model.Password);
                if (!isValidPassword)
                {
                    ViewBag.ErrorMessage = "Invalid password format.";
                    return View();
                }

                var salt = SecurityHelper.GenerateSalt(70);
                var passwordHash = SecurityHelper.HashPassword(model.Password, salt);
                // save user to database
                await _shoppingContext.Users.AddAsync(new User()
                {
                    Email = model.Email,
                    PasswordHash = passwordHash,
                    FullName = model.FullName,
                    PasswordSalt = salt,
                    Role = "Client"
                });
                await _shoppingContext.SaveChangesAsync();

                // role-based cookie authentication
                Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, "Client"),
                };
                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);
                if (!string.IsNullOrEmpty(redirectUrl) && Url.IsLocalUrl(redirectUrl))
                {
                    return Redirect(redirectUrl);
                }
                return Redirect("/");
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
