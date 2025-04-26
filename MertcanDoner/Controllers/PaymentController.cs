using Microsoft.AspNetCore.Mvc;
using MertcanDoner.Models;
using MertcanDoner.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MertcanDoner.Services;
using Microsoft.AspNetCore.SignalR;
using MertcanDoner.Hubs;
namespace MertcanDoner.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly FakePaymentService _paymentService;
        private readonly IHubContext<NotificationHub> _hubContext;


        public PaymentController(AppDbContext context, FakePaymentService paymentService,IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _paymentService = paymentService;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            var cartData = HttpContext.Session.GetString("CartData");
            var cart = string.IsNullOrEmpty(cartData)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartData);

            return View(cart);
        }
        [HttpPost]
            public async Task<IActionResult> Checkout(PaymentViewModel model)
          {
    // 1. Sepet kontrolü
            var cart = JsonConvert.DeserializeObject<List<CartItem>>(model.CartItemsJson);
            if (cart == null || !cart.Any())
            {
                ModelState.AddModelError(string.Empty, "Sepetiniz boş. Lütfen ürün ekleyin.");
                return View("Index", new List<CartItem>());
            }

            // 2. Kart bilgisi kontrolü (Fake ödeme API simülasyonu)
            var paymentResult = _paymentService.ProcessPayment(
                model.CardNumber,
                model.Expiration,
                model.CVV,
                model.CardName
            );

            if (!paymentResult)
            {
                ModelState.AddModelError(string.Empty, "Ödeme başarısız. Lütfen kart bilgilerinizi kontrol edin.");
                ViewBag.CartItems = cart;
                return View("Index", cart);
            }

            // 3. Siparişi kaydet
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var selectedAddressId = HttpContext.Session.GetInt32("SelectedAddressId");

            if (selectedAddressId == null)
            {
                TempData["Error"] = "Sipariş verebilmek için önce bir adres seçmelisiniz.";
                return RedirectToAction("Index", "Address");
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                AddressId = selectedAddressId.Value,
                Status = OrderStatus.Pending,
                Items = new List<OrderItem>()
            };

            foreach (var item in cart)
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId ?? 0,
                    Quantity = item.Quantity ?? 1,
                    Price = item.Product?.Price ?? 0,
                    SelectedOptions = new List<SelectedOption>()
                };

                if (item.SelectedOptions != null)
                {
                    foreach (var option in item.SelectedOptions)
                    {
                        orderItem.SelectedOptions.Add(new SelectedOption { Name = option });
                    }
                }

                order.Items.Add(orderItem);
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", "Yeni bir sipariş geldi!");

            HttpContext.Session.Remove("CartData");

            return RedirectToAction("Success");
        }


        
        public IActionResult Success()
        {
            return View();
        }
   
    }
}
