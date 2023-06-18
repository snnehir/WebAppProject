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

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var user = await _shoppingContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(emailClaim.Value));
            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _shoppingContext.Users.FindAsync(id);
            var model = new UserEditViewModel()
            {
                Email = user.Email,
                FullName = user.FullName,
                Id = id,
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            var user = await _shoppingContext.Users.FindAsync(model.Id);
            if (ModelState.IsValid)
            {
                user.FullName = model.FullName;
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    bool isValidPassword = Validation.IsValidPassword(model.NewPassword);
                    if (!isValidPassword)
                    {
                        ViewBag.ErrorMessage = "Invalid password format.";
                        return View();
                    }
                    var salt = SecurityHelper.GenerateSalt(70);
                    var passwordHash = SecurityHelper.HashPassword(model.NewPassword, salt);
                    user.PasswordSalt = salt;
                    user.PasswordHash = passwordHash;
                }
                if (!model.Email.Equals(user.Email))
                {
                    var isValidEmail = Validation.IsValidEmail(model.Email);
                    if (!isValidEmail)
                    {
                        ViewBag.ErrorMessage = "Invalid email.";
                        return View();
                    }
                    var userExist = await _shoppingContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Equals(model.Email));
                    if (userExist != null)
                    {
                        ViewBag.ErrorMessage = "This email is already used.";
                        return View();
                    }
                    user.Email = model.Email;
                }

                _shoppingContext.Users.Update(user);
                await _shoppingContext.SaveChangesAsync();
                ViewBag.SuccessMessage = "Profile is updated successfully!";
            }

            return View();
        }
    }
}
