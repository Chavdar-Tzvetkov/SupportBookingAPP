using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SupportBookingAPP.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
