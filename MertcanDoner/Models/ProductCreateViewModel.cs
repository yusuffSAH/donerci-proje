    using System.ComponentModel.DataAnnotations;

namespace MertcanDoner.Models
{
    public class ProductCreateViewModel
    {
        [Required]
        public string Name { get; set; } = "";

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        // Adminin yazacağı opsiyonlar (virgülle ayrılmış: Domates, Ketçap, Mayonez)
        public string? OptionsRaw { get; set; }
    }
}
