using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MertcanDoner.Data;
using MertcanDoner.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MertcanDoner.Controllers
{
    
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
         private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
            [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var orders = _context.Orders
            .Include(o => o.Address)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.Items)
                .ThenInclude(i => i.SelectedOptions)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.User)
                .ToList();

            return View(orders);
        }
        [Authorize]
public IActionResult MyOrders()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    var orders = _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Address)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.Items)
            .ThenInclude(i => i.SelectedOptions)
            .OrderByDescending(o => o.OrderDate)
             .Include(o => o.User)
            .ToList();
            

    return View(orders);
}
    }
}
