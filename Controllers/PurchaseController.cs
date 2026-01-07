using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationFES.Data;
using WebApplicationFES.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplicationFES.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Purchase
        public async Task<IActionResult> Index()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .ToListAsync();
            return View(purchases);
        }

        // GET: Purchase/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(m => m.PurchaseId == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // GET: Purchase/Create
        public IActionResult Create()
        {
            PopulateSuppliersDropDownList();
            PopulateProductsDropDownList();
            return View();
        }

        // POST: Purchase/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierId,PurchaseDate,TotalAmount,Status,PaymentMethod,Notes")] Purchase purchase, List<PurchaseItem> purchaseItems)
        {
            ModelState.Remove("Supplier");
            ModelState.Remove("PurchaseItems");
            ModelState.Remove("TotalAmount");

            for (int i = 0; i < purchaseItems.Count; i++)
            {
                ModelState.Remove($"purchaseItems[{i}].Product");
                ModelState.Remove($"purchaseItems[{i}].Purchase");
            }
            if (ModelState.IsValid)
            {
                // Calculate total amount
                purchase.TotalAmount = purchaseItems.Sum(i => i.UnitPrice * i.Quantity);
                _context.Add(purchase);
                await _context.SaveChangesAsync();

                foreach (var item in purchaseItems)
                {
                    if (item.ProductId > 0 && item.Quantity > 0)
                    {
                        item.PurchaseId = purchase.PurchaseId;
                        item.TotalPrice = item.UnitPrice * item.Quantity;
                        _context.Add(item);
                    }
                    var product = await _context.Products.FindAsync(item.ProductId);
                    product.StockQuantity += item.Quantity;

                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateSuppliersDropDownList(purchase.SupplierId);
            PopulateProductsDropDownList();
            return View(purchase);
        }

        // GET: Purchase/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);
            if (purchase == null)
                return NotFound();

            PopulateSuppliersDropDownList(purchase.SupplierId);
            PopulateProductsDropDownList();
            return View(purchase);
        }

        // POST: Purchase/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PurchaseId,SupplierId,PurchaseDate,TotalAmount,Status,PaymentMethod,Notes")] Purchase purchase, List<PurchaseItem> purchaseItems)
        {
            if (id != purchase.PurchaseId)
                return NotFound();

            ModelState.Remove("Supplier");
            ModelState.Remove("PurchaseItems");

            for (int i = 0; i < purchaseItems.Count; i++)
            {
                ModelState.Remove($"purchaseItems[{i}].Product");
                ModelState.Remove($"purchaseItems[{i}].Purchase");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchase);

                    // Restore inventory for old items
                    var existingItems = await _context.PurchaseItems
                        .Where(pi => pi.PurchaseId == purchase.PurchaseId)
                        .ToListAsync();
                    foreach (var oldItem in existingItems)
                    {
                        var product = await _context.Products.FindAsync(oldItem.ProductId);
                        if (product != null)
                            product.StockQuantity -= oldItem.Quantity;
                    }

                    // Remove existing items
                    _context.PurchaseItems.RemoveRange(existingItems);

                    // Add new items and update inventory
                    foreach (var item in purchaseItems)
                    {
                        if (item.ProductId > 0 && item.Quantity > 0)
                        {
                            item.PurchaseId = purchase.PurchaseId;
                            item.TotalPrice = item.UnitPrice * item.Quantity;
                            _context.Add(item);

                            var product = await _context.Products.FindAsync(item.ProductId);
                            if (product != null)
                                product.StockQuantity += item.Quantity;
                        }
                    }

                    purchase.TotalAmount = purchaseItems.Sum(i => i.UnitPrice * i.Quantity);

                    // Save all changes once
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseExists(purchase.PurchaseId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateSuppliersDropDownList(purchase.SupplierId);
            PopulateProductsDropDownList();
            purchase.PurchaseItems = purchaseItems;
            return View(purchase);
        }

        // GET: Purchase/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);
            if (purchase == null)
                return NotFound();

            return View(purchase);
        }

        // POST: Purchase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);
            if (purchase != null)
            {
                // Restore inventory for all items
                foreach (var item in purchase.PurchaseItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                        product.StockQuantity -= item.Quantity;
                }
                _context.Purchases.Remove(purchase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.PurchaseId == id);
        }

        private void PopulateSuppliersDropDownList(object selectedSupplier = null)
        {
            ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", selectedSupplier);
        }
        private void PopulateProductsDropDownList()
        {
            ViewBag.Products = new SelectList(_context.Products, "ProductId", "Name");
        }
    }
} 