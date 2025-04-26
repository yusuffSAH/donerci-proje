using Microsoft.AspNetCore.Mvc;
using MertcanDoner.Data;
using Microsoft.EntityFrameworkCore;

namespace MertcanDoner.Controllers
{
    public class UserviewController : Controller
    {
        private readonly AppDbContext _context;

        public UserviewController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
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
