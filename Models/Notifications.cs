using System.ComponentModel.DataAnnotations;

namespace SupportBookingAPP.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public DateTime NotifyAt { get; set; }

        public bool Sent { get; set; } = false;

        // navigation
        public virtual required Booking Booking { get; set; }
    }
}
