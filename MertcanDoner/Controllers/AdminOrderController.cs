using MertcanDoner.Data;
using MertcanDoner.Hubs;
using MertcanDoner.Models;
using MertcanDoner.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace MertcanDoner.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public AdminOrderController(AppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IActionResult Index(DateTime? filterDate)
        {
            var orders = _context.Orders
            .Include(o => o.User)
            .Include(o => o.Address)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.SelectedOptions) 
            .ToList();

        

            // ViewModel kullanmak daha düzenli olur
            var viewModel = new AdminOrderViewModel
            {

                FilterDate = filterDate,

                PendingOrders = orders
                    .Where(o => o.Status == OrderStatus.Pending)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList(),

                AcceptedAndShippedOrders = orders
                    .Where(o => (o.Status == OrderStatus.Accepted || o.Status == OrderStatus.Shipped))
                    .OrderByDescending(o => o.OrderDate)
                    .ToList(),

                DeliveredOrders = orders
                    .Where(o => o.Status == OrderStatus.Delivered &&
                                (!filterDate.HasValue || o.DeliveredAt?.Date == filterDate.Value.Date))
                    .OrderByDescending(o => o.OrderDate)
                    .ToList(),

                CancelledOrders = orders
                    .Where(o => o.Status == OrderStatus.Cancelled &&
                                (!filterDate.HasValue || o.CancelledAt?.Date == filterDate.Value.Date))
                    .OrderByDescending(o => o.OrderDate)
                    .ToList()
            };


                return View(viewModel);
        }      

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int orderId, OrderStatus newStatus)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
                return NotFound();

            order.Status = newStatus;
        switch (newStatus)
    {
        case OrderStatus.Accepted:
            order.AcceptedAt = DateTime.Now;
            break;
        case OrderStatus.Shipped:
            order.ShippedAt = DateTime.Now;
            break;
        case OrderStatus.Delivered:
            order.DeliveredAt = DateTime.Now;
            break;
        case OrderStatus.Cancelled:
            order.CancelledAt = DateTime.Now;
            break;
    }
             await _context.SaveChangesAsync();
             await _hubContext.Clients.Group($"User_{order.UserId}")
        .SendAsync("ReceiveNotification", $"📦 Siparişiniz '{order.Status}' durumuna güncellendi.");

            return RedirectToAction("Index");
        }
    }
}
