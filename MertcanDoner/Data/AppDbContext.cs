using MertcanDoner.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MertcanDoner.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductOption> ProductOptions { get; set; }
        public DbSet<SelectedOption> SelectedOptions { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            // Ürün silinince, geçmiş siparişin bozulmaması için bağlantıyı kopar ama siparişi silme
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            // Kullanıcı silinirse siparişler kalsın
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItem silinirse ona bağlı opsiyonlar da silinsin (varsayılan cascade)
            modelBuilder.Entity<OrderItem>()
                .HasMany(oi => oi.SelectedOptions)
                .WithOne(so => so.OrderItem)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
