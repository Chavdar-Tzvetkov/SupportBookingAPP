using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SupportBookingAPP.Models;
using SupportBookingAPP.Data;
using SupportBookingAPP.Services;

namespace SupportBookingAPP.Tests.Services
{
    public class BookingServiceTests
    {
        // In-memory DB context setup
        private static ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        // Helper method for mocking UserManager
        private static UserManager<ApplicationUser> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null).Object;
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldAddBookingToDb()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();

            // seed one category
            var category = new SupportCategory { Id = 1, Name = "General" };
            dbContext.SupportCategories.Add(category);
            await dbContext.SaveChangesAsync();

            var service = new BookingService(dbContext, GetMockUserManager(), new Mock<IEmailSender>().Object);

            var booking = new Booking
            {
                UserId = "test-user",
                EngineerId = 1,
                SupportCategoryId = category.Id,       
                SlotStart = DateTime.Now,
                SlotEnd = DateTime.Now.AddHours(1),
                IssueDescription = "Test issue"
            };

            var user = new ApplicationUser { Id = "test-user", Email = "test@example.com", UserName = "testuser" };

            // Act
            await service.CreateAsync(booking, user);

            // Assert
            var count = await dbContext.Bookings.CountAsync();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllBookings()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();

            // Seed engineers and users
            var engineer1 = new Engineer { Id = 1, Name = "Eng1", Email = "eng1@example.com" };
            var engineer2 = new Engineer { Id = 2, Name = "Eng2", Email = "eng2@example.com" };
            var user1 = new ApplicationUser { Id = "U1", Email = "user1@example.com", UserName = "user1" };
            var user2 = new ApplicationUser { Id = "U2", Email = "user2@example.com", UserName = "user2" };

            dbContext.Engineers.AddRange(engineer1, engineer2);
            dbContext.Users.AddRange(user1, user2);

            // seed two categories
            var cat1 = new SupportCategory { Id = 10, Name = "Hardware" };
            var cat2 = new SupportCategory { Id = 20, Name = "Software" };
            dbContext.SupportCategories.AddRange(cat1, cat2);

            // Seed bookings, now with SupportCategoryId
            dbContext.Bookings.AddRange(
                new Booking
                {
                    UserId = user1.Id,
                    EngineerId = engineer1.Id,
                    SupportCategoryId = cat1.Id,   
                    SlotStart = DateTime.Now,
                    SlotEnd = DateTime.Now.AddHours(1),
                    IssueDescription = "Issue 1"
                },
                new Booking
                {
                    UserId = user2.Id,
                    EngineerId = engineer2.Id,
                    SupportCategoryId = cat2.Id,   
                    SlotStart = DateTime.Now.AddHours(2),
                    SlotEnd = DateTime.Now.AddHours(3),
                    IssueDescription = "Issue 2"
                }
            );

            await dbContext.SaveChangesAsync();

            var service = new BookingService(
                dbContext,
                GetMockUserManager(),
                new Mock<IEmailSender>().Object
            );

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, b => Assert.NotNull(b.Engineer));
            Assert.All(result, b => Assert.NotNull(b.User));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectBooking()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();

            // Seed engineer, user
            var engineer = new Engineer { Id = 1, Name = "Engineer1", Email = "engineer1@example.com" };
            var user = new ApplicationUser { Id = "U1", Email = "user1@example.com", UserName = "user1" };
            dbContext.Engineers.Add(engineer);
            dbContext.Users.Add(user);

            // seed a category
            var category = new SupportCategory { Id = 42, Name = "Networking" };
            dbContext.SupportCategories.Add(category);

            // Seed booking with SupportCategoryId
            var booking = new Booking
            {
                Id = 100,
                UserId = user.Id,
                EngineerId = engineer.Id,
                SupportCategoryId = category.Id,   
                SlotStart = DateTime.Now,
                SlotEnd = DateTime.Now.AddHours(1),
                IssueDescription = "GetById test booking"
            };
            dbContext.Bookings.Add(booking);

            await dbContext.SaveChangesAsync();

            var service = new BookingService(
                dbContext,
                GetMockUserManager(),
                new Mock<IEmailSender>().Object
            );

            // Act
            var result = await service.GetByIdAsync(100);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result.Id);
            Assert.Equal("GetById test booking", result.IssueDescription);
            Assert.NotNull(result.Engineer);
            Assert.NotNull(result.User);
        }

        [Fact]
        public void Exists_ShouldReturnTrueIfBookingExists()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();

            // seed at least one category
            dbContext.SupportCategories.Add(new SupportCategory { Id = 99, Name = "Email" });

            // Seed booking with a valid category
            dbContext.Bookings.Add(new Booking
            {
                Id = 200,
                UserId = "test-user",
                EngineerId = 1,
                SupportCategoryId = 99,  // ← assign
                SlotStart = DateTime.Now,
                SlotEnd = DateTime.Now.AddHours(1),
                IssueDescription = "Check exists logic"
            });
            dbContext.SaveChanges();

            var service = new BookingService(dbContext, GetMockUserManager(), new Mock<IEmailSender>().Object);

            // Act
            var exists = service.Exists(200);

            // Assert
            Assert.True(exists);
        }
    }
}
