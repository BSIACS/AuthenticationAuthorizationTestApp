﻿using AuthenticationAuthorizationTestApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TestDataLibrary.DAL.Context;
using TestDataLibrary.Domain.Entities;

namespace AuthenticationAuthorizationTestApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly TestDataLibraryContext _dataContext;

        public AccountController(TestDataLibraryContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                User user = _dataContext.Users.FirstOrDefault(u => u.Email == loginViewModel.Email && u.Password == loginViewModel.Password);

                if (user is null)
                {
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");

                    return View(loginViewModel);
                }
                else {
                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }
            }

            return View(loginViewModel);
        }


        public async Task<IActionResult> Register(RegisterViewModel registerViewModel) {
            if (ModelState.IsValid) {
                User user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == registerViewModel.Email);
                if (user == null)
                {
                    user = new User { Email = registerViewModel.Email, Password = registerViewModel.Password };
                    Role userRole = await _dataContext.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                        user.Role = userRole;

                    _dataContext.Users.Add(user);
                    await _dataContext.SaveChangesAsync();

                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) парол");  
            }
            return View(registerViewModel);
        }

        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        private async Task Authenticate(User user) {
            var claims = new List<Claim> {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name),
            };

            ClaimsIdentity id = new ClaimsIdentity(
                claims,
                "ApplicationCoockie", 
                ClaimsIdentity.DefaultNameClaimType, 
                ClaimsIdentity.DefaultRoleClaimType
                );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
                    , new ClaimsPrincipal(id)
                    , new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMonths(1),
                        IsPersistent = true
                    }
                );
        }
    }
}
