using System.ComponentModel.DataAnnotations;

namespace SupportBookingAPP.Models
{
    public class Engineer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Specialty { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Start of Workday")]
        public DateTime WorkdayStart { get; set; }

        [Required]
        [Display(Name = "End of Workday")]
        public DateTime WorkdayEnd { get; set; }

        // Optional: Navigation property
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
