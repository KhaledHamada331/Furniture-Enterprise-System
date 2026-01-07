using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationFES.Data;
using WebApplicationFES.Models;

namespace WebApplicationFES.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Validate selling price is higher than purchase price
                if (product.SellingPrice <= product.PurchasePrice)
                {
                    ModelState.AddModelError("SellingPrice", "Selling price must be higher than purchase price");
                    return View(product);
                }

                // Handle image upload
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/uploads/products/" + uniqueFileName;
                }

                product.CreatedAt = DateTime.Now;
                product.IsActive = true;

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,SKU,Description,PurchasePrice,SellingPrice,StockQuantity,MinimumStockLevel,Category,ImageUrl,IsActive,CreatedAt")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validate selling price is higher than purchase price
                    if (product.SellingPrice <= product.PurchasePrice)
                    {
                        ModelState.AddModelError("SellingPrice", "Selling price must be higher than purchase price");
                        return View(product);
                    }

                    // Handle image upload
                    //if (ImageFile != null && ImageFile.Length > 0)
                    //{
                    //    // Delete old image if exists
                    //    if (!string.IsNullOrEmpty(product.ImageUrl))
                    //    {
                    //        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                    //        if (System.IO.File.Exists(oldImagePath))
                    //        {
                    //            System.IO.File.Delete(oldImagePath);
                    //        }
                    //    }

                    //    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/products");
                    //    if (!Directory.Exists(uploadsFolder))
                    //    {
                    //        Directory.CreateDirectory(uploadsFolder);
                    //    }

                    //    var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    //    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    //    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    //    {
                    //        await ImageFile.CopyToAsync(fileStream);
                    //    }

                    //    product.ImageUrl = "/uploads/products/" + uniqueFileName;
                    //}

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Delete product image if exists
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
} 