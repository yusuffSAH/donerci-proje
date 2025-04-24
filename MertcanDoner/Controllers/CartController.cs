using Microsoft.AspNetCore.Mvc;
using MertcanDoner.Data;
using MertcanDoner.Models;
using Newtonsoft.Json;

namespace MertcanDoner.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // 🟢 Sepeti Göster
        public IActionResult Index()
        {
            var cartData = HttpContext.Session.GetString("CartData");
            var cart = string.IsNullOrEmpty(cartData)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartData);

            return View(cart);
        }

        // 🟢 Sepete Ekle
       [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, List<string> selectedOptions)
        {
            var cartData = HttpContext.Session.GetString("CartData");
            var cart = string.IsNullOrEmpty(cartData)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartData);

            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return RedirectToAction("Index", "Userview");
            }

            var newItem = new CartItem
            {
                ProductId = productId,
                Product = product,
                Quantity = quantity,
                SelectedOptions = selectedOptions
            };

            cart.Add(newItem);
            HttpContext.Session.SetString("CartData", JsonConvert.SerializeObject(cart));

            return RedirectToAction("Index", "Userview");
        }

        // 🟢 Sepetten Sil
        public IActionResult Remove(int productId)
        {
            var cartData = HttpContext.Session.GetString("CartData");
            var cart = string.IsNullOrEmpty(cartData)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartData);

            var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                HttpContext.Session.SetString("CartData", JsonConvert.SerializeObject(cart));
            }

            return RedirectToAction("Index");
        }
    }
}
