using System;

namespace SupportBookingAPP.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int EngineerId { get; set; }
        public Engineer Engineer { get; set; } = null!;

        public DateTime SlotStart { get; set; }
        public DateTime SlotEnd { get; set; }

        public string IssueDescription { get; set; } = string.Empty;
    }

}
