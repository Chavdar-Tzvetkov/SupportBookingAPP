using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupportBookingAPP.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        [Required]
        [ForeignKey("Engineer")]
        public int EngineerId { get; set; }

        [Required]
        public DateTime SlotStart { get; set; }

        [Required]
        [DateGreaterThan(nameof(SlotStart), ErrorMessage = "End time must be after start time")]
        public DateTime SlotEnd { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Issue description must be between 10 and 500 characters.")]
        public string IssueDescription { get; set; }

        [ValidateNever]
        public ApplicationUser? User { get; set; }

        [ValidateNever]
        public Engineer Engineer { get; set; } = null!;
    }
}
