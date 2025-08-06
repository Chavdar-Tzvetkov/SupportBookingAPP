using SupportBookingAPP.Areas.Admin.Models;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Areas.Engineers.Models
{
    public class EngineerDashboardViewModel
    {
        
        public List<Booking> Bookings { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<SupportCategory>? Categories { get; set; }
    }
    
}
