using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SupportBookingAPP.Services
{
    public class DevEmailService : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Debug.WriteLine("=== MOCK EMAIL ===");
            Debug.WriteLine($"To: {email}");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Body: {htmlMessage}");
            Debug.WriteLine("==================");

            return Task.CompletedTask;
        }
    }
}
