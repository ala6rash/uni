using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Uni_Connect.Models;

namespace Uni_Connect.Tests
{
    public static class TestHelpers
    {
        public static ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        public static void SetControllerContext(Controller controller)
        {
            var httpContext = new DefaultHttpContext();
            
            // Mock Authentication Service to prevent SignInAsync/SignOutAsync NullReference Exception
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);
            authServiceMock
                .Setup(a => a.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IAuthenticationService>(authServiceMock.Object);
            httpContext.RequestServices = serviceCollection.BuildServiceProvider();

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            controller.ControllerContext = controllerContext;

            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;
        }

        public static string HashPassword(string rawPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(rawPassword);
        }

        public static User CreateTestUser(ApplicationDbContext context, string email, string password, int failedAttempts = 0, DateTime? lockedUntil = null, string? resetToken = null, DateTime? resetExpiry = null)
        {
            var user = new User
            {
                Name = "QA Tester",
                Email = email.ToLower(),
                Username = email.Split('@')[0],
                UniversityID = email.Split('@')[0],
                PasswordHash = HashPassword(password),
                Role = "Student",
                Points = 50,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                FailedLoginAttempts = failedAttempts,
                AccountLockedUntil = lockedUntil,
                PasswordResetToken = resetToken,
                PasswordResetTokenExpiry = resetExpiry
            };

            context.Users.Add(user);
            context.SaveChanges();

            return user;
        }
    }
}
