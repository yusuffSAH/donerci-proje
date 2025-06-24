using MertcanDoner.Settings;
using Stripe;
using MertcanDoner.Data;
using MertcanDoner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MertcanDoner.Services;
using MertcanDoner.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Razor, SignalR, Session ve diğer servisler
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSession();
builder.Services.AddSingleton<IUserIdProvider, NameIdentifierProvider>();

// Identity sistemi (kullanıcı giriş/rol yönetimi)
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Stripe ayarları
var stripeSection = builder.Configuration.GetSection("Stripe");
if (stripeSection == null)
{
    throw new InvalidOperationException("Stripe ayarları bulunamadı. Lütfen appsettings.json dosyasını kontrol edin.");
}

var stripeSettings = stripeSection.Get<StripeSettings>();
if (stripeSettings == null)
{
    throw new InvalidOperationException("Stripe ayarları geçersiz. Lütfen appsettings.json dosyasını kontrol edin.");
}

if (string.IsNullOrEmpty(stripeSettings.SecretKey) || string.IsNullOrEmpty(stripeSettings.PublishableKey))
{
    throw new InvalidOperationException("Stripe SecretKey veya PublishableKey bulunamadı. Lütfen appsettings.json dosyasını kontrol edin.");
}

builder.Services.Configure<StripeSettings>(stripeSection);
StripeConfiguration.ApiKey = stripeSettings.SecretKey;
builder.Services.AddScoped<StripePaymentService>();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Hata yönetimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Yönlendirmeler
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Userview}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/notificationHub");
app.MapRazorPages();

// Seed data (admin, ürün vs.)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}

app.Run();
