using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationFES.Data;
using WebApplicationFES.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplicationFES.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expense
        public async Task<IActionResult> Index()
        {
            var expenses = await _context.Expenses
                .Include(e => e.User)
                .ToListAsync();
            return View(expenses);
        }

        // GET: Expense/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.ExpenseId == id);
            if (expense == null)
                return NotFound();

            return View(expense);
        }

        // GET: Expense/Create
        public IActionResult Create()
        {
            PopulateUsersDropDownList();
            return View();
        }

        // POST: Expense/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpenseDate,Category,Amount,Description,UserId,PaymentMethod,ReceiptNumber")] Expense expense)
        {
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateUsersDropDownList(expense.UserId);
            return View(expense);
        }

        // GET: Expense/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.ExpenseId == id);
            if (expense == null)
                return NotFound();

            PopulateUsersDropDownList(expense.UserId);
            return View(expense);
        }

        // POST: Expense/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpenseId,ExpenseDate,Category,Amount,Description,UserId,PaymentMethod,ReceiptNumber")] Expense expense)
        {
            if (id != expense.ExpenseId)
                return NotFound();

            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.ExpenseId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateUsersDropDownList(expense.UserId);
            return View(expense);
        }

        // GET: Expense/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var expense = await _context.Expenses
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.ExpenseId == id);
            if (expense == null)
                return NotFound();

            return View(expense);
        }

        // POST: Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.ExpenseId == id);
        }

        private void PopulateUsersDropDownList(object selectedUser = null)
        {
            ViewBag.Users = new SelectList(_context.Users, "UserId", "FirstName", selectedUser);
        }
    }
} 