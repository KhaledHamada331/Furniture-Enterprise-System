using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace WebApplicationFES.Controllers
{
    public class RoleController : Controller
    {
        public IActionResult SelectRole(string role)
        {
            if (!string.IsNullOrEmpty(role))
            {
                if (role == "Admin")
                {
                    return RedirectToAction("Login", "Admin");
                }
                TempData["SelectedRole"] = role;
                return RedirectToAction("Login");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Role = TempData["SelectedRole"]?.ToString();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string role, string username, string password)
        {
            if (string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError("role", "Role is required.");
                return View();
            }
            if (role == "Admin")
            {
                ModelState.Remove("Username");
                if (password == "Nothing")
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, "Admin"),
                        new Claim(ClaimTypes.Role, "Admin")
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    HttpContext.Session.SetString("CurrentRole", role);
                    HttpContext.Session.SetString("CurrentUser", "Admin");
                    return RedirectToAction("AdminDashboard");
                }
                ModelState.AddModelError("password", "Invalid admin password.");
                ViewBag.Role = role;
                return View();
            }
            else
            {
                // Check user exists with username/email, password, and role
                var db = HttpContext.RequestServices.GetService(typeof(WebApplicationFES.Data.ApplicationDbContext)) as WebApplicationFES.Data.ApplicationDbContext;
                var user = db?.Users.FirstOrDefault(u => (u.Username == username || u.Email == username) && u.Password == password && u.Role == role && u.IsActive);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, role)
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    HttpContext.Session.SetString("CurrentRole", role);
                    HttpContext.Session.SetString("CurrentUser", user.Username);
                    switch (role)
                    {
                        case "Accountant": return RedirectToAction("AccountantDashboard");
                        case "InventoryManager": return RedirectToAction("InventoryDashboard");
                        case "SalesManager": return RedirectToAction("SalesDashboard");
                    }
                }
                ModelState.AddModelError("login", "Invalid credentials or inactive user.");
                ViewBag.Role = role;
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            var sessionRole = HttpContext.Session.GetString("CurrentRole");
            if (!User.IsInRole("Admin") && sessionRole != "Admin")
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            return View();
        }

        public IActionResult AccountantDashboard() => View();
        public IActionResult InventoryDashboard() => View();
        public IActionResult SalesDashboard() => View();

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
} 