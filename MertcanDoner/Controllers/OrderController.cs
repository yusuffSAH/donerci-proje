using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MertcanDoner.Data;
using MertcanDoner.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

using MertcanDoner.ViewModels;

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
            
        [Authorize]
    public IActionResult MyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var allOrders = _context.Orders
        .Where(o => o.UserId == userId)
        .Include(o => o.Address)
        .Include(o => o.Items)
            .ThenInclude(i => i.Product)
        .Include(o => o.Items)
            .ThenInclude(i => i.SelectedOptions)
        .Include(o => o.User)
        .OrderByDescending(o => o.OrderDate)
        .ToList();

        var viewModel = new UserOrderViewModel
    {
        ActiveOrders = allOrders
            .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Accepted || o.Status == OrderStatus.Shipped)
            .ToList(),

        PastOrders = allOrders
            .Where(o => o.Status == OrderStatus.Delivered || o.Status == OrderStatus.Cancelled)
            .ToList()
    };
                

        return View(viewModel);
    }
        }
    }
