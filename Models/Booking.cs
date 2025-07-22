using System.ComponentModel.DataAnnotations;

namespace SupportBookingAPP.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int EngineerId { get; set; }

        [Required]
        public DateTime SlotStart { get; set; }

        [Required]
        [DateGreaterThan(nameof(SlotStart), ErrorMessage = "End must be after start")]
        public DateTime SlotEnd { get; set; }

        [Required, StringLength(500)]
        public string IssueDescription { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Engineer Engineer { get; set; }
    }


}
