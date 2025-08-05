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
    public class SupportCategoriesControllerTests
    {
        private static ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Ensures isolated DB
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewWithCategories()
        {
            var context = GetDbContext();
            context.SupportCategories.Add(new SupportCategory { Name = "TestCat" });
            await context.SaveChangesAsync();

            var controller = new SupportCategoriesController(context);
            var result = await controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<SupportCategory>>(view.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Create_Post_Valid_AddsToDbAndRedirects()
        {
            var context = GetDbContext();
            var controller = new SupportCategoriesController(context);
            var category = new SupportCategory { Name = "Networking" };

            var result = await controller.Create(category);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Single(context.SupportCategories);
        }

        [Fact]
        public async Task Edit_Post_IdMismatch_ReturnsNotFound()
        {
            var context = GetDbContext();
            var controller = new SupportCategoriesController(context);
            var category = new SupportCategory { Id = 2, Name = "Mismatch" };

            var result = await controller.Edit(1, category);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesCategoryAndRedirects()
        {
            var context = GetDbContext();
            var category = new SupportCategory { Name = "To Delete" };
            context.SupportCategories.Add(category);
            await context.SaveChangesAsync();

            var existingCategory = context.SupportCategories.First();
            var controller = new SupportCategoriesController(context);
            var result = await controller.DeleteConfirmed(existingCategory.Id);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(context.SupportCategories);
        }
    }
}
