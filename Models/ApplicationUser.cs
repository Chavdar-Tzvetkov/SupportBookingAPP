using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SupportBookingAPP.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string EquipmentId { get; set; } = string.Empty;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
