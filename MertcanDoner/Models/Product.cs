using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MertcanDoner.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }           // Örn: Tavuk Döner
        public string? Description { get; set; }    // Açıklama
        public decimal? Price { get; set; }         // Fiyat
        public string? ImageUrl { get; set; }       // Ürün resmi
        public List<ProductOption> Options { get; set; } = new();

    }
}