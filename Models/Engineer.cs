using System.ComponentModel.DataAnnotations;

namespace SupportBookingAPP.Models
{
    public class Engineer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Specialty { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Start of Workday")]
        public DateTime WorkdayStart { get; set; }

        [Required]
        [Display(Name = "End of Workday")]
        public DateTime WorkdayEnd { get; set; }

        //Navigation property
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
