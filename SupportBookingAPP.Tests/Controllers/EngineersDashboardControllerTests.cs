using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SupportBookingAPP.Areas.Engineers.Controllers;
using SupportBookingAPP.Areas.Engineers.Models;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace SupportBookingAPP.Tests.Areas.Engineers
{
    public class EngineersDashboardControllerTests
    {
        private static ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private static ClaimsPrincipal GetEngineerPrincipal(string email, string userId)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "Engineer")
            }, "mock"));
        }

        [Fact]
        public async Task Index_ReturnsEngineerDashboardView_WithBookingsAndNotifications()
        {
            // Arrange
            var context = GetDbContext();

            var engineer = new Engineer { Id = 1, Name = "Eng Test", Email = "eng@example.com" };
            var user = new ApplicationUser { Id = "eng-user-id", Email = engineer.Email, EngineerId = engineer.Id };
            var category = new SupportCategory { Id = 1, Name = "Hardware" };

            context.Users.Add(user);
            context.Engineers.Add(engineer);
            context.SupportCategories.Add(category);

            var booking = new Booking
            {
                EngineerId = engineer.Id,
                UserId = user.Id,
                SlotStart = DateTime.Now,
                SlotEnd = DateTime.Now.AddHours(1),
                IssueDescription = "Test Booking",
                SupportCategoryId = category.Id
            };

            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            context.Notifications.Add(new Notification
            {
                BookingId = booking.Id,
                NotifyAt = DateTime.Now.AddMinutes(-5),
                Sent = false,
                Booking = booking
            });

            await context.SaveChangesAsync();

            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var controller = new DashboardController(context, mockUserManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = GetEngineerPrincipal(user.Email, user.Id)
                    }
                }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EngineerDashboardViewModel>(viewResult.Model);

            Assert.Single(model.Bookings);
            Assert.Single(model.Notifications);
        }

    }
}
