using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationFES.Data;
using WebApplicationFES.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplicationFES.Controllers
{
    public class PayrollController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayrollController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Payroll
        public async Task<IActionResult> Index()
        {
            var payrolls = await _context.Payrolls
                .Include(p => p.User)
                .ToListAsync();
            return View(payrolls);
        }

        // GET: Payroll/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PayrollId == id);
            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // GET: Payroll/Create
        public IActionResult Create()
        {
            PopulateUsersDropDownList();
            return View();
        }

        // POST: Payroll/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,PayPeriodStart,PayPeriodEnd,BaseSalary,OvertimePay,Bonuses,Deductions,Status,PaymentMethod,Notes")] Payroll payroll)
        {
            ModelState.Remove("User");
            ModelState.Remove("NetSalary");
            if (ModelState.IsValid)
            {
                payroll.NetSalary = payroll.BaseSalary + payroll.OvertimePay + payroll.Bonuses - payroll.Deductions;
                _context.Add(payroll);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateUsersDropDownList(payroll.UserId);
            return View(payroll);
        }

        // GET: Payroll/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var payroll = await _context.Payrolls
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PayrollId == id);
            if (payroll == null)
                return NotFound();

            PopulateUsersDropDownList(payroll.UserId);
            return View(payroll);
        }

        // POST: Payroll/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PayrollId,UserId,PayPeriodStart,PayPeriodEnd,BaseSalary,OvertimePay,Bonuses,Deductions,NetSalary,Status,PaymentDate,PaymentMethod,Notes")] Payroll payroll)
        {
            if (id != payroll.PayrollId)
                return NotFound();

            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payroll);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayrollExists(payroll.PayrollId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateUsersDropDownList(payroll.UserId);
            return View(payroll);
        }

        // GET: Payroll/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var payroll = await _context.Payrolls
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PayrollId == id);
            if (payroll == null)
                return NotFound();

            return View(payroll);
        }

        // POST: Payroll/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll != null)
                _context.Payrolls.Remove(payroll);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateUsersDropDownList(object selectedUser = null)
        {
            ViewBag.Users = new SelectList(_context.Users, "UserId", "FirstName", selectedUser);
        }

        private bool PayrollExists(int id)
        {
            return _context.Payrolls.Any(e => e.PayrollId == id);
        }
    }
} 