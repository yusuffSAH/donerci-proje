using Microsoft.AspNetCore.Mvc;
using MertcanDoner.Data;
using Microsoft.EntityFrameworkCore;
using MertcanDoner.Models;

namespace MertcanDoner.Controllers
{
    public class UserviewController : Controller
    {
        private readonly AppDbContext _context;

        public UserviewController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? category)
        {
           var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
            products = products.Where(p => p.Category == category);
            }

            var categories = _context.Products
            .Select(p => p.Category)
            .Distinct()
            .ToList();

            ViewBag.Categories = categories;

             return View(products.ToList());
        }
        public IActionResult Details(int id)
        {
        var product = _context.Products
        .Include(p => p.Options)
        .FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
        return NotFound();
        }

        return View(product);
        }
       
    }
}
