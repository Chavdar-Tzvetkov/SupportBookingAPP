using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SupportBookingAPP.Controllers;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;
using SupportBookingAPP.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SupportBookingAPP.Tests.Controllers
{
    public class BookingsControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            return new ApplicationDbContext(options);
        }

        private UserManager<ApplicationUser> GetMockUserManager(ApplicationUser user)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            mgr.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            mgr.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
            return mgr.Object;
        }

        private IEmailSender GetMockEmailSender()
        {
            // NOTE: This is the correct (non-generic) interface!
            return Mock.Of<IEmailSender>();
        }

        [Fact]
        public async Task Index_ReturnsViewWithBookings()
        {
            // Arrange
            var context = GetDbContext();
            context.Bookings.Add(new Booking { Id = 1, IssueDescription = "Sample Booking" });
            context.SaveChanges();

            var user = new ApplicationUser { Id = "U1", Email = "user@example.com" };
            var controller = new BookingsController(
                new BookingService(context, GetMockUserManager(user), GetMockEmailSender()),
                GetMockUserManager(user));

            // Act
            var result = await controller.Index();

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Booking>>(view.Model);
        }

        [Fact]
        public async Task Details_ReturnsError404_WhenIdIsNull()
        {
            // Arrange
            var controller = new BookingsController(null!, null!);

            // Act
            var result = await controller.Details(null);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error404", redirect.ActionName);
        }

        [Fact]
        public async Task Details_ReturnsError404_WhenBookingNotFound()
        {
            // Arrange
            var context = GetDbContext();
            var user = new ApplicationUser { Id = "U1", Email = "user@example.com" };

            var controller = new BookingsController(
                new BookingService(context, GetMockUserManager(user), GetMockEmailSender()),
                GetMockUserManager(user));

            // Act
            var result = await controller.Details(999);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error404", redirect.ActionName);
        }

        [Fact]
        public async Task Details_ReturnsForbid_WhenUserNotAuthorized()
        {
            // Arrange: Test user
            var currentUser = new ApplicationUser
            {
                Id = "user-1",
                Email = "user@example.com"
            };

            // Booking that belongs to another user
            var booking = new Booking
            {
                Id = 1,
                UserId = "different-user-id",
                IssueDescription = "Unauthorized booking"
            };

            // Mock IBookingService
            var mockBookingService = new Mock<IBookingService>();
            mockBookingService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(booking);
            mockBookingService.Setup(s => s.UserCanAccessBooking(currentUser, booking)).ReturnsAsync(false);

            // Mock UserManager
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null);

            mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(currentUser);

            // Set up controller with context
            var controller = new BookingsController(mockBookingService.Object, mockUserManager.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, currentUser.Id),
                new Claim(ClaimTypes.Email, currentUser.Email)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await controller.Details(1);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }
        [Fact]
        public async Task Edit_ReturnsForbid_WhenUserNotAuthorized()
        {
            // Arrange
            var currentUser = new ApplicationUser { Id = "user-1", Email = "user@example.com" };
            var booking = new Booking { Id = 1, UserId = "different-user-id" };

            var mockService = new Mock<IBookingService>();
            mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(booking);
            mockService.Setup(s => s.UserCanAccessBooking(currentUser, booking)).ReturnsAsync(false);

            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);

            var controller = new BookingsController(mockService.Object, mockUserManager.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, currentUser.Id)
        }))
                }
            };

            // Act
            var result = await controller.Edit(1);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsForbid_WhenUserNotAuthorized()
        {
            // Arrange
            var currentUser = new ApplicationUser { Id = "user-1", Email = "user@example.com" };
            var booking = new Booking { Id = 2, UserId = "other-user" };

            var mockService = new Mock<IBookingService>();
            mockService.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(booking);
            mockService.Setup(s => s.UserCanAccessBooking(currentUser, booking)).ReturnsAsync(false);

            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            mockUserManager.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);

            var controller = new BookingsController(mockService.Object, mockUserManager.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, currentUser.Id)
        }))
                }
            };

            // Act
            var result = await controller.Delete(2);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsViewWithErrors()
        {
            // Arrange
            var mockService = new Mock<IBookingService>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var controller = new BookingsController(mockService.Object, mockUserManager.Object);
            controller.ModelState.AddModelError("SlotStart", "Required");

            var booking = new Booking(); // missing required fields

            // Act
            var result = await controller.Create(booking);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(booking, view.Model);
            Assert.False(controller.ModelState.IsValid);
        }




    }
}
