using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationFES.Data;
using WebApplicationFES.Dtos;
using WebApplicationFES.Models;

namespace WebApplicationFES.Controllers
{
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sale
        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales
                .Include(s => s.User)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
            return View(sales);
        }

        // GET: Sale/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(m => m.SaleId == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // GET: Sale/Create
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products
                .Where(p => p.StockQuantity > 0)
                .OrderBy(p => p.Name)
                .ToList();
            return View();
        }

        public class SaleItemViewModel
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
        }

        // POST: Sale/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerName,CustomerPhone,PaymentMethod,Notes")] SaleDto saleDto, List<SaleItemViewModel> SaleItems)
        {
            Sale sale = new Sale();
            if (ModelState.IsValid && SaleItems != null && SaleItems.Any())
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // Set sale properties
                        sale.SaleDate = DateTime.Now;
                        sale.CreatedAt = DateTime.Now;
                        sale.Status = "Pending";
                        sale.UserId = 5; // TODO: Get from authenticated user
                        sale.TotalAmount = SaleItems.Sum(item => item.TotalPrice);
                        sale.CustomerName = saleDto.CustomerName;
                        sale.CustomerPhone = saleDto.CustomerPhone;
                        sale.PaymentMethod = saleDto.PaymentMethod;
                        sale.Notes = saleDto.Notes;
                        // Add sale
                        _context.Add(sale);
                        await _context.SaveChangesAsync();

                        // Add sale items and update stock
                        foreach (var item in SaleItems)
                        {
                            var product = await _context.Products.FindAsync(item.ProductId);
                            if (product == null || product.StockQuantity < item.Quantity)
                            {
                                throw new Exception($"Insufficient stock for product {product?.Name ?? "Unknown"}");
                            }

                            var saleItem = new SaleItem
                            {
                                SaleId = sale.SaleId,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                TotalPrice = item.TotalPrice
                            };

                            _context.SaleItems.Add(saleItem);
                            product.StockQuantity -= item.Quantity;
                            _context.Update(product);
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", "Error creating sale: " + ex.Message);
                    }
                }
            }

            ViewBag.Products = _context.Products
                .Where(p => p.StockQuantity > 0)
                .OrderBy(p => p.Name)
                .ToList();
            return View(sale);
        }

        // GET: Sale/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            return View(sale);
        }

        // POST: Sale/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SaleId,CustomerName,CustomerPhone,Status,PaymentMethod,Notes")] Sale sale)
        {
            if (id != sale.SaleId)
            {
                return NotFound();
            }
            ModelState.Remove("SaleItems");
            ModelState.Remove("User");
            

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSale = await _context.Sales.FindAsync(id);
                    if (existingSale == null)
                    {
                        return NotFound();
                    }

                    existingSale.CustomerName = sale.CustomerName;
                    existingSale.CustomerPhone = sale.CustomerPhone;
                    existingSale.Status = sale.Status;
                    existingSale.PaymentMethod = sale.PaymentMethod;
                    existingSale.Notes = sale.Notes;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.SaleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sale);
        }

        // GET: Sale/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.SaleId == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // POST: Sale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var sale = await _context.Sales
                        .Include(s => s.SaleItems)
                        .FirstOrDefaultAsync(s => s.SaleId == id);

                    if (sale != null)
                    {
                        // Restore product quantities
                        foreach (var item in sale.SaleItems)
                        {
                            var product = await _context.Products.FindAsync(item.ProductId);
                            if (product != null)
                            {
                                product.StockQuantity += item.Quantity;
                                _context.Update(product);
                            }
                        }

                        _context.Sales.Remove(sale);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.SaleId == id);
        }
    }
} 