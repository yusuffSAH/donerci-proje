using Microsoft.AspNetCore.Mvc;
using MertcanDoner.Data;
using MertcanDoner.Models;
using Microsoft.AspNetCore.Authorization;


namespace MertcanDoner.Controllers
{   [Authorize(Roles = "Admin")]
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
public IActionResult Create(ProductCreateViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);

    var product = new Product
    {
        Name = model.Name,
        Description = model.Description,
        Price = model.Price,
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

    return RedirectToAction("Index");
}   

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
