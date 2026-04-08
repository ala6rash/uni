using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Uni_Connect.Controllers;
using Uni_Connect.ViewModels;
using Xunit;

namespace Uni_Connect.Tests
{
    public class ResetPasswordControllerTests
    {
        [Fact]
        public void ResetPassword_Get_WithValidToken_ReturnsForm()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var result = controller.ResetPassword("123456") as ViewResult;

            Assert.NotNull(result);
            Assert.IsType<ResetPasswordViewModel>(result.Model);
            Assert.Equal("123456", ((ResetPasswordViewModel)result.Model).ResetToken);
        }

        [Fact]
        public void ResetPassword_Get_WithoutToken_RedirectsToForgotPassword()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var result = controller.ResetPassword((string)null) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("ForgotPassword", result.ActionName);
        }

        [Fact]
        public async Task ResetPassword_Post_WithValidTokenAndPassword_UpdatesPassword()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var user = TestHelpers.CreateTestUser(dbContext, "resetme@philadelphia.edu.jo", "OldPass123!", resetToken: "654321", resetExpiry: DateTime.Now.AddMinutes(10));
            var oldHash = user.PasswordHash;

            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new ResetPasswordViewModel { ResetToken = "654321", NewPassword = "NewPassword123!", ConfirmPassword = "NewPassword123!" };
            var result = await controller.ResetPassword(model) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);

            var dbUser = dbContext.Users.First();
            Assert.NotEqual(oldHash, dbUser.PasswordHash);
            Assert.Null(dbUser.PasswordResetToken);
        }

        [Fact]
        public async Task ResetPassword_Post_WithInvalidToken_ShowsError()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            TestHelpers.CreateTestUser(dbContext, "invalid@philadelphia.edu.jo", "Pass123!", resetToken: "111111", resetExpiry: DateTime.Now.AddMinutes(10));
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new ResetPasswordViewModel { ResetToken = "999999", NewPassword = "NewPassword123!", ConfirmPassword = "NewPassword123!" };
            var result = await controller.ResetPassword(model) as ViewResult;

            Assert.NotNull(result);
            Assert.Contains(controller.ModelState.Values.SelectMany(v => v.Errors), e => e.ErrorMessage.Contains("Invalid reset code"));
        }

        [Fact]
        public async Task ResetPassword_Post_WithExpiredToken_ShowsError()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            TestHelpers.CreateTestUser(dbContext, "expired@philadelphia.edu.jo", "Pass123!", resetToken: "222222", resetExpiry: DateTime.Now.AddMinutes(-10));
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new ResetPasswordViewModel { ResetToken = "222222", NewPassword = "NewPassword123!", ConfirmPassword = "NewPassword123!" };
            var result = await controller.ResetPassword(model) as ViewResult;

            Assert.NotNull(result);
            Assert.Contains(controller.ModelState.Values.SelectMany(v => v.Errors), e => e.ErrorMessage.Contains("expired"));
        }

        [Fact]
        public async Task ResetPassword_Post_ClearsToken_AfterSuccessfulReset()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            TestHelpers.CreateTestUser(dbContext, "clear@philadelphia.edu.jo", "OldPass123!", resetToken: "333333", resetExpiry: DateTime.Now.AddMinutes(10));
            
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new ResetPasswordViewModel { ResetToken = "333333", NewPassword = "NewPassword123!", ConfirmPassword = "NewPassword123!" };
            await controller.ResetPassword(model);

            Assert.Null(dbContext.Users.First().PasswordResetToken);
        }

        [Fact]
        public async Task ResetPassword_Post_ClearsTokenExpiry_AfterReset()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            TestHelpers.CreateTestUser(dbContext, "clearexpiry@philadelphia.edu.jo", "OldPass123!", resetToken: "444444", resetExpiry: DateTime.Now.AddMinutes(10));
            
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new ResetPasswordViewModel { ResetToken = "444444", NewPassword = "NewPassword123!", ConfirmPassword = "NewPassword123!" };
            await controller.ResetPassword(model);

            Assert.Null(dbContext.Users.First().PasswordResetTokenExpiry);
        }

        [Fact]
        public async Task ResetPassword_Post_ResetsFailedLoginAttempts()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            TestHelpers.CreateTestUser(dbContext, "resetfails@philadelphia.edu.jo", "OldPass123!", failedAttempts: 3, resetToken: "555555", resetExpiry: DateTime.Now.AddMinutes(10));
            
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new ResetPasswordViewModel { ResetToken = "555555", NewPassword = "NewPassword123!", ConfirmPassword = "NewPassword123!" };
            await controller.ResetPassword(model);

            Assert.Equal(0, dbContext.Users.First().FailedLoginAttempts);
        }

        [Fact]
        public async Task ResetPassword_Post_UnlocksAccount_IfLocked()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            TestHelpers.CreateTestUser(dbContext, "unlock@philadelphia.edu.jo", "OldPass123!", lockedUntil: DateTime.Now.AddMinutes(30), resetToken: "666666", resetExpiry: DateTime.Now.AddMinutes(10));
            
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new ResetPasswordViewModel { ResetToken = "666666", NewPassword = "NewPassword123!", ConfirmPassword = "NewPassword123!" };
            await controller.ResetPassword(model);

            Assert.Null(dbContext.Users.First().AccountLockedUntil);
        }

        [Fact]
        public async Task ResetPassword_Post_HashesNewPassword_WithBCrypt()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            TestHelpers.CreateTestUser(dbContext, "hash@philadelphia.edu.jo", "OldPass123!", resetToken: "777777", resetExpiry: DateTime.Now.AddMinutes(10));
            
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new ResetPasswordViewModel { ResetToken = "777777", NewPassword = "NewPassword123!", ConfirmPassword = "NewPassword123!" };
            await controller.ResetPassword(model);

            var newHash = dbContext.Users.First().PasswordHash;
            Assert.StartsWith("$2a$", newHash);
            Assert.NotEqual("NewPassword123!", newHash);
        }

        [Fact]
        public async Task ResetPassword_Post_WithInvalidModelState_ReturnsView()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);
            controller.ModelState.AddModelError("NewPassword", "Required");

            var result = await controller.ResetPassword(new ResetPasswordViewModel()) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task ResetPassword_Post_WithDatabaseError_ShowsFriendlyError()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var result = await controller.ResetPassword((ResetPasswordViewModel)null) as ViewResult;
            Assert.Contains(controller.ModelState.Values.SelectMany(v => v.Errors), e => e.ErrorMessage.Contains("error occurred"));
        }
    }
}
