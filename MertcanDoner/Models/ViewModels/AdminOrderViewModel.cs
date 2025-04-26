namespace MertcanDoner.Models.ViewModels
{
    public class AdminOrderViewModel
    {
        public List<Order>? PendingOrders { get; set; }
         public List<Order> AcceptedAndShippedOrders { get; set; } = new();
        public List<Order>? CancelledOrders { get; set; }
       
        public List<Order>? DeliveredOrders { get; set; } = new();
         public DateTime? FilterDate { get; set; } 

    }
}
