namespace MertcanDoner.Models
{
    public class PaymentViewModel
    {
        public string CardName { get; set; } = "";
        public string CardNumber { get; set; } = "";
        public string Expiration { get; set; } = "";
        public string CVV { get; set; } = "";
        public string CartItemsJson { get; set; } = "";
    }
}
