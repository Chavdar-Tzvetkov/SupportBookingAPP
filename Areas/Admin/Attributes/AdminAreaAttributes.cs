using Microsoft.AspNetCore.Mvc;

namespace SupportBookingAPP.Attributes
{
    public class AdminAreaAttribute : AreaAttribute
    {
        public AdminAreaAttribute() : base("Admin") { }
    }
}
