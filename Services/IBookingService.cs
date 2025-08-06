using Microsoft.AspNetCore.Mvc.Rendering;
using SupportBookingAPP.Models;

namespace SupportBookingAPP.Services
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task<bool> UserCanAccessBooking(ApplicationUser user, Booking booking);
        Task<List<SelectListItem>> GetEngineerSelectListAsync();
        Task<List<SelectListItem>> GetSupportCategorySelectListAsync();
        IEnumerable<SupportCategory> GetAllCategories();
        Task CreateAsync(Booking booking, ApplicationUser user);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(Booking booking);
        Task MarkAsCompletedAsync(int bookingId);
        bool Exists(int id);
    }
}
