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
        public IActionResult Login(string role, string username, string password)
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
                    HttpContext.Session.SetString("CurrentRole", role);
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
                var user = db.Users.FirstOrDefault(u => (u.Username == username || u.Email == username) && u.Password == password && u.Role == role && u.IsActive);
                if (user != null)
                {
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

        public IActionResult AdminDashboard() => View();
        public IActionResult AccountantDashboard() => View();
        public IActionResult InventoryDashboard() => View();
        public IActionResult SalesDashboard() => View();

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
} 