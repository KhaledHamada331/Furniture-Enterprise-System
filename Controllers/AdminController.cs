using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationFES.Data;

namespace WebApplicationFES.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /admin or /admin/dashboard
        [HttpGet("/admin")]
        [HttpGet("/admin/dashboard")]
        public IActionResult Index()
        {
            // Verify session or role for route guard defense in depth
            var sessionRole = HttpContext.Session.GetString("CurrentRole");
            if (!User.IsInRole("Admin") && sessionRole != "Admin")
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View("~/Views/Role/AdminDashboard.cshtml");
        }

        // Dedicated, non-indexed route for Admin Login
        [AllowAnonymous]
        [HttpGet("/portal-access")]
        [HttpGet("/admin/login")]
        public IActionResult Login()
        {
            if (User.IsInRole("Admin") || HttpContext.Session.GetString("CurrentRole") == "Admin")
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost("/portal-access")]
        [HttpPost("/admin/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string password)
        {
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

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                HttpContext.Session.SetString("CurrentRole", "Admin");
                HttpContext.Session.SetString("CurrentUser", "Admin");

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("password", "Invalid admin password.");
            return View();
        }

        // --- Strictly Protected Admin API Endpoints ---

        [HttpGet("/api/admin/stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalSuppliers = await _context.Suppliers.CountAsync();
            var totalPurchases = await _context.Purchases.CountAsync();
            var totalSales = await _context.Sales.CountAsync();
            var totalExpenses = await _context.Expenses.CountAsync();
            var totalPayrolls = await _context.Payrolls.CountAsync();

            return Ok(new
            {
                totalUsers,
                totalProducts,
                totalSuppliers,
                totalPurchases,
                totalSales,
                totalExpenses,
                totalPayrolls,
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("/api/admin/users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.Role,
                    u.IsActive
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("/api/admin/status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "OK",
                role = "Admin",
                authenticated = true,
                user = User.Identity?.Name ?? "Admin"
            });
        }
    }
}
