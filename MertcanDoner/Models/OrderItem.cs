using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MertcanDoner.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; } // Sipariş anındaki fiyat
        public List<SelectedOption> SelectedOptions { get; set; } = new();
    }
}