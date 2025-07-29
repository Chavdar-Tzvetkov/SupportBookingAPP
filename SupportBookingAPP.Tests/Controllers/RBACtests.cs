using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SupportBookingAPP.Areas.Admin.Controllers;
using SupportBookingAPP.Controllers;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;
using System;
using System.Reflection;
using System.Security.Claims;
using Xunit;

namespace SupportBookingAPP.Tests
{
    public class RBACLogicTests
    {
        private AdminController CreateAdminControllerWithUserRole(string role)
        {
            // Set up a unique in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed a user for testing
            var testUser = new ApplicationUser
            {
                Id = "test-user-id",
                Email = "admin@test.com",
                UserName = "admin@test.com",
                FirstName = "Test",
                LastName = "Admin"
            };
            context.Users.Add(testUser);
            context.SaveChanges();

            // Use real UserStore over the in-memory context
            var store = new UserStore<ApplicationUser>(context);

            // Create UserManager with only required services
            var userManager = new UserManager<ApplicationUser>(
                store, null, new PasswordHasher<ApplicationUser>(), null, null, null, null, null, null
            );

            // Manually assign role (since EF doesn’t auto-link roles)
            context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = testUser.Id,
                RoleId = "admin-role-id"
            });
            context.SaveChanges();

            // Simulate logged-in user with specified role
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, testUser.Id),
            new Claim(ClaimTypes.Email, testUser.Email),
            new Claim(ClaimTypes.Role, role)
            }, "TestAuth"));

            var controller = new AdminController(context, userManager)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            return controller;
        }

        private EngineersController CreateEngineersControllerWithUserRole(string role)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"EngineersTestDb_{Guid.NewGuid()}")
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed test engineer
            context.Engineers.Add(new Engineer
            {
                Id = 1,
                Name = "Test Eng",
                Email = "eng@test.com",
                Specialty = "TestSpec",
                WorkdayStart = DateTime.Now,
                WorkdayEnd = DateTime.Now.AddHours(8)
            });
            context.SaveChanges();

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Role, role)
            }, "TestAuth"));

            var controller = new EngineersController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            return controller;
        }


        [Fact]
        public void AdminController_HasAuthorizeAttribute_WithAdminRole()
        {
            var type = typeof(AdminController);
            var attr = type.GetCustomAttribute<AuthorizeAttribute>();

            Assert.NotNull(attr);
            Assert.Equal("Admin", attr.Roles);
        }

        [Fact]
        public void BookingsController_HasAuthorizeAttribute_WithoutSpecificRole()
        {
            var type = typeof(BookingsController);
            var attr = type.GetCustomAttribute<AuthorizeAttribute>();

            Assert.NotNull(attr);
            Assert.Null(attr.Roles);
        }

        [Fact]
        public async Task AdminController_Index_ShouldReturnView_ForAdminRole()
        {
            // Arrange
            var controller = CreateAdminControllerWithUserRole("Admin");

            // Act
            var result = await controller.Index();

            // Assert
            var view = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task EngineerRole_Hits_Index_Despite_NoAccessPolicyBeingChecked()
        {
            var controller = CreateEngineersControllerWithUserRole("Engineer");

            var result = await controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            Assert.NotNull(view.Model);
        }


    }
}
