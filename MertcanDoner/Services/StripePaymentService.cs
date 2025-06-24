using MertcanDoner.Settings;
using Stripe;

using Microsoft.Extensions.Options;

namespace MertcanDoner.Services
{
    public class StripePaymentService
    {
        private readonly StripeSettings _stripeSettings;

        public StripePaymentService(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(long amount, string currency)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" }
            };

            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }
    }
}