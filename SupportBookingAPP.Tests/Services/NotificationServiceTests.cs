using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;
using SupportBookingAPP.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SupportBookingAPP.Tests.Services
{
    public class NotificationServiceTests
    {
        private static ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task RunPendingAsync_ShouldSendEmailsAndMarkNotificationsAsSent()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();

            var user = new ApplicationUser { Id = "user1", Email = "user@example.com", UserName = "user" };
            var engineer = new Engineer { Id = 1, Name = "Engineer One", Email = "eng@example.com" };

            var booking = new Booking
            {
                Id = 1,
                UserId = user.Id,
                User = user,
                EngineerId = engineer.Id,
                Engineer = engineer,
                SlotStart = DateTime.Now.AddHours(1),
                SlotEnd = DateTime.Now.AddHours(2),
                IssueDescription = "Test",
            };

            var notification = new Notification
            {
                Id = 1,
                Booking = booking,
                NotifyAt = DateTime.Now.AddMinutes(-10),
                Sent = false
            };

            dbContext.Users.Add(user);
            dbContext.Engineers.Add(engineer);
            dbContext.Bookings.Add(booking);
            dbContext.Notifications.Add(notification);
            await dbContext.SaveChangesAsync();

            var emailSenderMock = new Mock<IEmailSender>();
            emailSenderMock
                .Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var services = new ServiceCollection();
            services.AddSingleton(dbContext); 
            var provider = services.BuildServiceProvider();

            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var scopeMock = new Mock<IServiceScope>();

            scopeMock.Setup(s => s.ServiceProvider).Returns(provider);
            scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);

            var service = new NotificationService(scopeFactoryMock.Object, emailSenderMock.Object);

            // Act
            await service.RunPendingAsync();

            // Assert
            emailSenderMock.Verify(x => x.SendEmailAsync(
                user.Email,
                It.IsAny<string>(),
                It.Is<string>(body => body.Contains(engineer.Name))),
                Times.Once);

            var updatedNotification = await dbContext.Notifications.FirstOrDefaultAsync();
            Assert.True(updatedNotification!.Sent);
        }
    }
}
