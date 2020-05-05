using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AcutePediatricsOrientation.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using AcutePediatricsOrientation.Enums;
using Microsoft.AspNetCore.Authorization;
using AcutePediatricsOrientation.ViewModels;

namespace AcutePediatricsOrientation.Controllers
{
    public class AccountController : Controller
    {
        private readonly AcutePediatricsContext _context;

        public AccountController(AcutePediatricsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Account account)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Account.SingleOrDefault(a => a.Email == account.Email && a.Password == account.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "username or password is invalid");
                    return View();
                }
                else
                {
                    // Create the identity from the user info
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                    identity.AddClaim(new Claim(ClaimTypes.Name, account.Email));
                    if (user.RoleId == (int)ProjectEnum.Role.Educator)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, "Educator"));
                    }

                    // Authenticate using the identity
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = false });

                    return RedirectToAction("Index", "Package");
                }
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Policy = "Educator")]
        [HttpGet]
        public IActionResult Register()
        {
            var registerViewModel = new RegisterViewModel
            {
                Roles = _context.Role.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
            };
            return View(registerViewModel);
        }

        [Authorize(Policy = "Educator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                if (!_context.Account.Any(a => a.Email == registerViewModel.Email))
                {
                    var account = new Account {
                        Email = registerViewModel.Email,
                        FirstName = registerViewModel.FirstName,
                        LastName = registerViewModel.LastName,
                        Password = registerViewModel.Password,
                        RoleId = registerViewModel.RoleId,
                    };

                    _context.Account.Add(account);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Package");
                }
            }

            registerViewModel.Roles = _context.Role.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name });
            return View(registerViewModel);
        }
    }
}
