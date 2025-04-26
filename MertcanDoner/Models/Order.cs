using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MertcanDoner.Models
{

    public enum OrderStatus
    {
    Pending,     // Bekliyor
    Accepted,    // Kabul Edildi
    Cancelled,   // İptal Edildi
    Delivered,  
    Shipped
    }
    public class Order
    {
    public int Id { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; } // Identity kullanıyorsan

    public DateTime OrderDate { get; set; }
    public int? AddressId { get; set; }
    public Address Address { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    }
}