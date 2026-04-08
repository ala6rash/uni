using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Uni_Connect.Controllers;
using Uni_Connect.ViewModels;
using Xunit;

namespace Uni_Connect.Tests
{
    public class ForgotPasswordControllerTests
    {
        [Fact]
        public void ForgotPassword_Get_ReturnsViewWithViewModel()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var result = controller.ForgotPassword() as ViewResult;

            Assert.NotNull(result);
            Assert.IsType<ForgotPasswordViewModel>(result.Model);
        }

        [Fact]
        public async Task ForgotPassword_Post_WithValidEmail_GeneratesToken()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            TestHelpers.CreateTestUser(dbContext, "valid@philadelphia.edu.jo", "Password123!");
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new ForgotPasswordViewModel { Email = "valid@philadelphia.edu.jo" };
            var result = await controller.ForgotPassword(model) as ViewResult;

            Assert.NotNull(result);
            Assert.True(controller.ViewBag.EmailSent);
            
            var user = dbContext.Users.First();
            Assert.NotNull(user.PasswordResetToken);
        }

        [Fact]
        public async Task ForgotPassword_Post_Token_Is6Digits()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            TestHelpers.CreateTestUser(dbContext, "tokenlen@philadelphia.edu.jo", "Password123!");
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new ForgotPasswordViewModel { Email = "tokenlen@philadelphia.edu.jo" };
            await controller.ForgotPassword(model);

            var token = dbContext.Users.First().PasswordResetToken;
            Assert.Equal(6, token?.Length);
            Assert.True(int.TryParse(token, out _));
        }

        [Fact]
        public async Task ForgotPassword_Post_TokenExpiry_Is30Minutes()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            TestHelpers.CreateTestUser(dbContext, "expiry@philadelphia.edu.jo", "Password123!");
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new ForgotPasswordViewModel { Email = "expiry@philadelphia.edu.jo" };
            await controller.ForgotPassword(model);

            var expiry = dbContext.Users.First().PasswordResetTokenExpiry;
            Assert.NotNull(expiry);
            
            var diff = expiry.Value - System.DateTime.Now;
            Assert.True(diff.TotalMinutes > 29 && diff.TotalMinutes <= 30);
        }

        [Fact]
        public async Task ForgotPassword_Post_WithNonExistentEmail_StillShowsSuccess()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new ForgotPasswordViewModel { Email = "nobody@philadelphia.edu.jo" };
            var result = await controller.ForgotPassword(model) as ViewResult;

            // Should show success (don't reveal user doesn't exist)
            Assert.NotNull(result);
            Assert.True(controller.ViewBag.EmailSent);
        }

        [Fact]
        public async Task ForgotPassword_Post_WithDatabaseError_ShowsFriendlyError()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var result = await controller.ForgotPassword(null) as ViewResult;
            Assert.Contains(controller.ModelState.Values.SelectMany(v => v.Errors), e => e.ErrorMessage.Contains("error occurred"));
        }
    }
}
