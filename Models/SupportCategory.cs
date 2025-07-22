using System.ComponentModel.DataAnnotations;

namespace SupportBookingAPP.Models
{
    public class SupportCategory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
    }
}
