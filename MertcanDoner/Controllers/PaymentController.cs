using Microsoft.AspNetCore.Mvc;
using MertcanDoner.Models;
using MertcanDoner.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MertcanDoner.Services;

namespace MertcanDoner.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly FakePaymentService _paymentService;


        public PaymentController(AppDbContext context, FakePaymentService paymentService)
        {
            _context = context;
              _paymentService = paymentService;
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
        public IActionResult Checkout(PaymentViewModel model)
        {
    // 1. Sepet kontrolü
          var cart = JsonConvert.DeserializeObject<List<CartItem>>(model.CartItemsJson);
             if (cart == null || !cart.Any())
             {
                return RedirectToAction("Index", "Userview");
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

            // tekrar sepeti göstermek için deserialize et
            var css = JsonConvert.DeserializeObject<List<CartItem>>(model.CartItemsJson);

            // formun tekrar gösterilmesi için aynı ViewModel'deki cart'ı inject et
            ViewBag.CartItems = css;

            return View("Index", css); // aynı sayfaya dön
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
            orderItem.SelectedOptions.Add(new SelectedOption
            {
                Name = option
            });
             }
     }

    order.Items.Add(orderItem);
}
        _context.Orders.Add(order);
        _context.SaveChanges();

        HttpContext.Session.Remove("CartData");

        return RedirectToAction("Success");
    }


        
        public IActionResult Success()
        {
            return View();
        }
   
    }
}
