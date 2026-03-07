using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTOs.UserDto;
using ContactsManager.Core.Enums;
using ContactsManager.UI.Areas.Admin.Controllers;
using CRUDs.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    [Route("/[controller]/[action]")]
    //[AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        [Authorize("NotAuthenticated")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [Authorize("NotAuthenticated")]
        public async Task<IActionResult> Register(RegisterDto user)
        {
            if(ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(user);
            }
            var newUser = new ApplicationUser
                {
                    UserName = user.Email,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    PersonName = user.Name
                };
            var result = await _userManager.CreateAsync(newUser, user.Password);
            if (result.Succeeded)
            {
                if(user.UserType == UserTypeOptions.Admin)
                {
                    await AssignRoleAndSignIn(UserTypeOptions.Admin.ToString(), newUser);
                }
                else
                {
                    await AssignRoleAndSignIn(UserTypeOptions.User.ToString(), newUser);
                }
                    return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(user);
        }
        [HttpGet]
        [Authorize("NotAuthenticated")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [Authorize("NotAuthenticated")]
        public async Task<IActionResult> Login(LoginDto user, string? ReturnUrl)
        {
            if(ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return View(user);
            }

            var result = await _signInManager
                .PasswordSignInAsync(user.Email, user.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var userInDb = await _userManager.FindByEmailAsync(user.Email);
                if (userInDb != null)
                {
                    if(await _userManager.IsInRoleAsync(userInDb, UserTypeOptions.Admin.ToString()))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                }

                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }

                return RedirectToAction("Index", "Persons");
            }

            ModelState.AddModelError("Login", "Invalid email or password");
            return View(user);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Persons");
        }
        public async Task AssignRoleAndSignIn(string role, ApplicationUser newUser)
        {
            if (await _roleManager.FindByNameAsync(role) == null)
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = role });
            }
            await _userManager.AddToRoleAsync(newUser, role);
            await _signInManager.SignInAsync(newUser, isPersistent: false);
        }
    }
}
