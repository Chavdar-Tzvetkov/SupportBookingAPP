using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportBookingAPP.Areas.Admin.Controllers;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SupportBookingAPP.Tests.Areas.Admin
{
    public class EngineersControllerTests
    {
        private static ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewWithEngineers()
        {
            var context = GetDbContext();
            context.Engineers.Add(new Engineer { Name = "John Doe" });
            await context.SaveChangesAsync();

            var controller = new EngineersController(context);
            var result = await controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Engineer>>(view.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Create_Post_ValidEngineer_AddsToDbAndRedirects()
        {
            var context = GetDbContext();
            var controller = new EngineersController(context);

            var engineer = new Engineer { Name = "Jane Smith" };

            var result = await controller.Create(engineer);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Single(context.Engineers);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsView()
        {
            var context = GetDbContext();
            var controller = new EngineersController(context);
            controller.ModelState.AddModelError("Name", "Required");

            var engineer = new Engineer { Id = 1, Name = "" };

            var result = await controller.Edit(1, engineer);

            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal(engineer, view.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesEngineerAndRedirects()
        {
            var context = GetDbContext();
            var engineer = new Engineer { Name = "To Delete" };
            context.Engineers.Add(engineer);
            await context.SaveChangesAsync();

            var existingEngineer = context.Engineers.First();
            var controller = new EngineersController(context);

            var result = await controller.DeleteConfirmed(existingEngineer.Id);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(context.Engineers);
        }
    }
}
