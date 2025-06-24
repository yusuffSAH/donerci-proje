using Microsoft.AspNetCore.Mvc;
using MertcanDoner.Data;
using MertcanDoner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MertcanDoner.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Product/
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // GET: /Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Category = model.Category,
                ImageUrl = model.ImageUrl,
                Options = new List<ProductOption>()
            };

            if (!string.IsNullOrEmpty(model.OptionsRaw))
            {
                var options = model.OptionsRaw.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var option in options)
                {
                    product.Options.Add(new ProductOption
                    {
                        Name = option.Trim()
                    });
                }
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Product/Edit/5
       public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _context.Products
                .Include(p => p.Options)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            var model = new ProductCreateViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price ?? 0,  // Burayı böyle düzelt
                Category = product.Category,
                ImageUrl = product.ImageUrl,
                OptionsRaw = product.Options != null ? string.Join(", ", product.Options.Select(o => o.Name)) : ""
            };

            return View(model);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductCreateViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var product = _context.Products
                .Include(p => p.Options)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Category = model.Category;
            product.ImageUrl = model.ImageUrl;

            // Eski Options temizleniyor
            _context.ProductOptions.RemoveRange(product.Options);

            // Yeni Options ekleniyor
            product.Options = new List<ProductOption>();
            if (!string.IsNullOrEmpty(model.OptionsRaw))
            {
                var options = model.OptionsRaw.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var option in options)
                {
                    product.Options.Add(new ProductOption
                    {
                        Name = option.Trim()
                    });
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Product/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
