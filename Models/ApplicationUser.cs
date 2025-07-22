using Microsoft.AspNetCore.Identity;

namespace SupportBookingAPP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string EquipmentId { get; set; } = string.Empty;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
