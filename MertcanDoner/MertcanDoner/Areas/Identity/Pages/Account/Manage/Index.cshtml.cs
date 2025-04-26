using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MertcanDoner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MertcanDoner.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Display(Name = "Ad Soyad")]
            public string FullName { get; set; }

            [Display(Name = "Adres")]
            public string Address { get; set; }

            [Display(Name = "Telefon")]
            public string Phone { get; set; }

            [Phone]
            [Display(Name = "Telefon (Identity)")]
            public string PhoneNumber { get; set; } // Identity içinde var zaten
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = user.UserName;

            Input = new InputModel
            {
                FullName = user.FullName,
                Address = user.Address,
                Phone = user.Phone,
                PhoneNumber = phoneNumber
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Telefon numarası Identity kısmında ayrı tutuluyor
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Telefon numarası güncellenemedi.";
                    return RedirectToPage();
                }
            }

            // Ekstra özel alanları kaydet
            user.FullName = Input.FullName;
            user.Address = Input.Address;
            user.Phone = Input.Phone;

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Profiliniz güncellendi.";
            return RedirectToPage();
        }
    }
}
