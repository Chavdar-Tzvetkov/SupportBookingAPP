using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SupportBookingAPP.Controllers;
using Xunit;

namespace SupportBookingAPP.Tests
{
    public class ErrorControllerTests
    {
        [Fact]
        public void Error500_Returns_Error500View()
        {
            // Arrange
            var controller = new ErrorController();
            var httpContext = new DefaultHttpContext();

            var featureMock = new Mock<IExceptionHandlerPathFeature>();
            featureMock.Setup(f => f.Path).Returns("/test");
            featureMock.Setup(f => f.Error).Returns(new Exception("Test error"));

            httpContext.Features.Set(featureMock.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = controller.Error500() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Error500", result.ViewName);
            Assert.Equal("/test", controller.ViewData["Path"]);
            Assert.Equal("Test error", controller.ViewData["Error"]);
        }

        [Theory]
        [InlineData(404, "Error404")]
        [InlineData(403, "Error403")]
        [InlineData(501, "ErrorGeneric")]
        public void HttpStatusCodeHandler_Returns_CorrectView(int statusCode, string expectedViewName)
        {
            // Arrange
            var controller = new ErrorController();

            // Act
            var result = controller.HttpStatusCodeHandler(statusCode) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedViewName, result.ViewName);
            if (statusCode != 404 && statusCode != 403)
            {
                Assert.Equal(statusCode, result.Model);
            }
        }
        [Fact]
        public void HttpStatusCodeHandler_UnknownCode_Returns_GenericView()
        {
            // Arrange
            var controller = new ErrorController();

            // Act
            var result = controller.HttpStatusCodeHandler(999) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ErrorGeneric", result.ViewName);
            Assert.Equal(999, result.Model);
        }

    }
}