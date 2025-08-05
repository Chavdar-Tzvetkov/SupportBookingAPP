using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SupportBookingAPP.Areas.Admin.Controllers;
using SupportBookingAPP.Areas.Admin.Models;
using SupportBookingAPP.Data;
using SupportBookingAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SupportBookingAPP.Tests.Areas.Admin
{
    public class AdminControllerTests
    {
        private static ApplicationDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsDashboardViewModelWithExpectedData()
        {
            // Arrange
            var db = GetInMemoryDb();

            var engineer = new Engineer { Id = 1, Name = "Engineer1", Email = "eng1@example.com" };
            var category = new SupportCategory { Id = 1, Name = "Software" };
            var user = new ApplicationUser { Id = "user-1", Email = "admin@example.com" };

            db.Engineers.Add(engineer);
            db.SupportCategories.Add(category);
            db.Users.Add(user);
            db.Bookings.Add(new Booking
            {
                Id = 1,
                UserId = user.Id,
                User = user,
                Engineer = engineer,
                EngineerId = engineer.Id,
                SlotStart = DateTime.Now,
                SlotEnd = DateTime.Now.AddHours(1),
                IssueDescription = "Dashboard issue",
                SupportCategory = category,
                SupportCategoryId = category.Id
            });

            await db.SaveChangesAsync();

            var userList = new List<ApplicationUser> { user }.AsQueryable();

            // Use async-capable query provider
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null
            );

            mockUserManager.Setup(m => m.Users).Returns(new TestAsyncEnumerable<ApplicationUser>(userList));
            mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                           .ReturnsAsync(["Admin"]);

            var controller = new AdminController(db, mockUserManager.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AdminDashboardViewModel>(viewResult.Model);
            Assert.Single(model.Users!);
            Assert.Single(model.Engineers!);
            Assert.Single(model.Categories!);
            Assert.Single(model.Bookings);
        }
    }

    // Async-capable test helper for IQueryable
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
            => new TestAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression)
            => _inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression)
            => _inner.Execute<TResult>(expression);

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            => Execute<TResult>(expression);
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

        public T Current => _inner.Current;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());
    }
}
