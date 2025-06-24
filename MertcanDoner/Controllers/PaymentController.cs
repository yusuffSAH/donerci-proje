using Microsoft.AspNetCore.Mvc;
using MertcanDoner.Models;
using MertcanDoner.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MertcanDoner.Services;
using Microsoft.AspNetCore.SignalR;
using MertcanDoner.Hubs;
using Microsoft.Extensions.Configuration;
using Stripe;
using MertcanDoner.Settings;
using Microsoft.Extensions.Options;

namespace MertcanDoner.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly StripePaymentService _paymentService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly StripeSettings _stripeSettings;

        // Controller constructor - Stripe ayarları ve servisler alınır
        public PaymentController(
            AppDbContext context,
            StripePaymentService paymentService,
            IHubContext<NotificationHub> hubContext,
            IOptions<StripeSettings> stripeSettings)
        {
            _context = context;
            _paymentService = paymentService;
            _hubContext = hubContext;
            _stripeSettings = stripeSettings.Value;
        }

        // Ödeme Sayfası - Sepet bilgisi alınıp ödeme ekranına gönderilir
        public IActionResult Index()
        {
            var cartData = HttpContext.Session.GetString("CartData");
            var cart = string.IsNullOrEmpty(cartData)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartData);

            ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;

            return View(cart);
        }

        // Stripe ödeme isteği oluşturuluyor (PaymentIntent)
        [HttpPost]
        [Route("Payment/CreatePaymentIntent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
        {
            try
            {
                // Sepet verisini session'dan al
                var cartData = HttpContext.Session.GetString("CartData");
                var cart = string.IsNullOrEmpty(cartData)
                    ? new List<CartItem>()
                    : JsonConvert.DeserializeObject<List<CartItem>>(cartData);

                // Toplam tutarı kuruş cinsinden hesapla
                long amount = (long)(cart.Sum(item =>
                    (item.Product?.Price ?? 0) * (item.Quantity ?? 1)) * 100);

                // Stripe PaymentIntent oluştur
                var intent = await _paymentService.CreatePaymentIntentAsync(amount, "TRY");

                ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;

                // clientSecret ve intentId frontend'e gönderiliyor
                return Json(new
                {
                    paymentIntentId = intent.Id,
                    paymentIntentClientSecret = intent.ClientSecret
                });
            }
            catch (Exception ex)
            {
                ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;
                return Json(new { error = ex.Message });
            }
        }

        // Ödeme başarılı mı kontrol edilir ve sipariş oluşturulur
        [HttpPost]
        public async Task<IActionResult> Checkout(PaymentViewModel model)
        {
            var cart = JsonConvert.DeserializeObject<List<CartItem>>(model.CartItemsJson);
            if (cart == null || !cart.Any())
            {
                ModelState.AddModelError(string.Empty, "Sepetiniz boş. Lütfen ürün ekleyin.");
                ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;
                return View("Index", new List<CartItem>());
            }

            if (string.IsNullOrEmpty(model.PaymentIntentId))
            {
                ModelState.AddModelError(string.Empty, "Ödeme işlemi başlatılamadı.");
                ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;
                return View("Index", cart);
            }

            try
            {
                // Stripe ile ödeme durumu kontrolü
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(model.PaymentIntentId);

                if (paymentIntent.Status != "succeeded")
                {
                    ModelState.AddModelError(string.Empty, "Ödeme henüz tamamlanmadı.");
                    ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;
                    return View("Index", cart);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ödeme hatası: {ex.Message}");
                ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;
                return View("Index", cart);
            }

            // Kullanıcı ve adres bilgileri kontrolü
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var selectedAddressId = HttpContext.Session.GetInt32("SelectedAddressId");

            if (selectedAddressId == null)
            {
                TempData["Error"] = "Sipariş verebilmek için önce bir adres seçmelisiniz.";
                return RedirectToAction("Index", "Address");
            }

            // Sipariş veritabanına kaydediliyor
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
                    SelectedOptions = item.SelectedOptions?.Select(o => new SelectedOption { Name = o }).ToList()
                };
                order.Items.Add(orderItem);
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Admin'e anlık bildirim gönder
            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", "Yeni bir sipariş geldi!");

            // Sepet temizleniyor
            HttpContext.Session.Remove("CartData");

            ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;
            return RedirectToAction("Success");
        }

        // Ödeme başarılı sayfası
        public IActionResult Success()
        {
            ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;
            return View();
        }
    }

    // Stripe ödeme isteği model (frontend'den gelen)
    public class CreatePaymentIntentRequest
    {
        public string PaymentMethodId { get; set; }
    }
}
