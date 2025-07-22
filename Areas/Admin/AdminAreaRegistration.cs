using Microsoft.AspNetCore.Mvc;

namespace SupportBookingAPP.Areas.Admin
{
    public class AdminAreaAttribute : AreaAttribute
    {
        public AdminAreaAttribute() : base("Admin") { }
    }
}
