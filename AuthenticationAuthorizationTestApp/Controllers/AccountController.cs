using AuthenticationAuthorizationTestApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        public AccountController(TestDataLibraryContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
                User user = null;

                #region as entity ORM query
                //user = _dataContext.Users
                //    .Include(u => u.Role)
                //    .FirstOrDefault(u => u.Email == loginViewModel.Email && u.Password == loginViewModel.Password);
                #endregion


                #region as raw sql query
                using (var connection = new SqlConnection(Configuration.GetConnectionString("DefaultConnection_1")))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(
                        "SELECT[u].[Id], [u].[Email], [u].[Password], [u].[RoleId], [r].[Name]"
                        + "FROM[Users] AS[u]"
                        + "LEFT JOIN[Roles] AS[r] ON[u].[RoleId] = [r].[Id]"
                        + $"WHERE (Email='{loginViewModel.Email}' AND Password = '{loginViewModel.Password}')"
                        , connection);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        user = new User();
                        user.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        user.Email = reader.GetString(reader.GetOrdinal("Email"));
                        user.Password = reader.GetString(reader.GetOrdinal("Password"));
                        user.RoleId = reader.GetInt32(reader.GetOrdinal("RoleId"));
                        user.Role = new Role { Id = user.RoleId.Value, Name = reader.GetString(reader.GetOrdinal("Name")) };
                    };
                }
                #endregion

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

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
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
