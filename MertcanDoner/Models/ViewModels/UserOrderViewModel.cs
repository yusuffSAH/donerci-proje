using MertcanDoner.Models;

namespace MertcanDoner.ViewModels
{
    public class UserOrderViewModel
    {
        public List<Order> ActiveOrders { get; set; } = new();
        public List<Order> PastOrders { get; set; } = new();
    }
}
