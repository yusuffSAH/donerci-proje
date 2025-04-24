using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MertcanDoner.Models
{
    public class Address
    {
        public int Id { get; set; }

        [Required]       
        [StringLength(10, ErrorMessage = "Başlık en fazla 10 karakter olabilir.")]
        public string Title { get; set; } // Ev, İş, Yurt, vb.

        [Required]
        public string FullAddress { get; set; } // Açık adres

        public bool IsDefault { get; set; } = false;

        // Kullanıcı ilişkisi
        
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
