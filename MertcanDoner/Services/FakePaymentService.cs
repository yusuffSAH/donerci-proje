namespace MertcanDoner.Services
{
    public class FakePaymentService
    {
        public bool ProcessPayment(string cardNumber, string expiration, string cvv, string cardName)
        {
            // 👇 Gerçek API gibi davranalım
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length != 16)
                return false;

            if (string.IsNullOrWhiteSpace(expiration) || !expiration.Contains("/"))
                return false;

            if (string.IsNullOrWhiteSpace(cvv) || (cvv.Length < 3 || cvv.Length > 4))
                return false;

            if (string.IsNullOrWhiteSpace(cardName))
                return false;

            // Her şey doğruysa "başarılı ödeme" kabul et
            return true;
        }
    }
}
