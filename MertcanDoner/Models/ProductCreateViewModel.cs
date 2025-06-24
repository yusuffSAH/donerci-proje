    using System.ComponentModel.DataAnnotations;

namespace MertcanDoner.Models
{
    public class ProductCreateViewModel
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Category { get; set; } = "";

        public string? ImageUrl { get; set; }

        // Adminin yazacağı opsiyonlar (virgülle ayrılmış: Domates, Ketçap, Mayonez)
        public string? OptionsRaw { get; set; }
        

        
    }
}
