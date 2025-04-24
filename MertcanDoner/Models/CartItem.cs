using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MertcanDoner.Models
{
    public class CartItem
    {
        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public int? Quantity { get; set; } // Kaç adet
        public List<string>? SelectedOptions { get; set; }
    }
}