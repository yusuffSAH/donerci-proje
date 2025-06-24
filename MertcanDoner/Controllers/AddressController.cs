using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MertcanDoner.Data;
using MertcanDoner.Models;
using System.Security.Claims;

namespace MertcanDoner.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly AppDbContext _context;

        public AddressController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var addresses = _context.Addresses
                .Where(a => a.UserId == userId)
                .ToList();

            return View(addresses);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Address address)
        {
            ModelState.Remove("User");
            ModelState.Remove("UserId");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            address.UserId = userId;

           
            if (ModelState.IsValid)
            {
                _context.Add(address);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(address);
        }
        [HttpPost]
        public IActionResult Select(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var address = _context.Addresses.FirstOrDefault(a => a.Id == id && a.UserId == userId);
            if (address == null)
            {
                return NotFound();
            }

            // Seçilen adresi session'a yaz
            HttpContext.Session.SetInt32("SelectedAddressId", address.Id);

            TempData["Message"] = "Adres seçildi: " + address.Title;
            return RedirectToAction("Index", "Userview"); // İstediğin sayfaya yönlendir
        }
    }
}
