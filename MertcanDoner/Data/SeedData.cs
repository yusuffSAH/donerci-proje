using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using MertcanDoner.Models;

namespace MertcanDoner.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            string adminEmail = "admin@donerci.com";
            string adminPassword = "Admin123!";

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            // 🔥 Ürünleri ekleyelim ama tekrar tekrar eklenmesin
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product { Name = "Tavuk Döner", Category = "Döner", Price = 50, Description = "Lezzetli tavuk döner", ImageUrl = "" },
                    new Product { Name = "Et Döner", Category = "Döner", Price = 70, Description = "Gerçek et döner", ImageUrl = "" },
                    new Product { Name = "Ayran", Category = "İçecek", Price = 10, Description = "Soğuk ayran", ImageUrl = "" },
                    new Product { Name = "Coca-Cola", Category = "İçecek", Price = 15, Description = "Kutu kola", ImageUrl = "" },
                    new Product { Name = "Tavuk Menü", Category = "Menü", Price = 80, Description = "Tavuk döner + içecek", ImageUrl = "" }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
