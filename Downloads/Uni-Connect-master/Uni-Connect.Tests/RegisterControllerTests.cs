using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Uni_Connect.Controllers;
using Uni_Connect.ViewModels;
using Xunit;

namespace Uni_Connect.Tests
{
    public class RegisterControllerTests
    {
        [Fact]
        public void Register_Get_ReturnsViewWithRegisterViewModel()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            var result = controller.Register() as ViewResult;

            Assert.NotNull(result);
            Assert.IsType<RegisterViewModel>(result.Model);
        }

        [Fact]
        public async Task Register_Post_WithValidData_CreatesUser()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new RegisterViewModel 
            { 
                Name = "Test Student", 
                Email = "testnew@philadelphia.edu.jo", 
                Faculty = "IT", 
                YearOfStudy = "1st", 
                Password = "Password123!", 
                ConfirmPassword = "Password123!" 
            };

            var result = await controller.Register(model) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            
            var user = dbContext.Users.First();
            Assert.Equal("testnew@philadelphia.edu.jo", user.Email);
            Assert.Equal(50, user.Points);
            Assert.Equal("Student", user.Role);
        }

        [Fact]
        public async Task Register_Post_WithDuplicateEmail_ShowsError()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            TestHelpers.CreateTestUser(dbContext, "duplicate@philadelphia.edu.jo", "Pass123!");
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new RegisterViewModel { Email = "duplicate@philadelphia.edu.jo" };
            var result = await controller.Register(model) as ViewResult;

            Assert.NotNull(result);
            Assert.Contains(controller.ModelState.Values.SelectMany(v => v.Errors), e => e.ErrorMessage.Contains("already exists"));
        }

        [Fact]
        public async Task Register_Post_WithNonUniversityEmail_ShowsError()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new RegisterViewModel { Email = "hacker@gmail.com" };
            var result = await controller.Register(model) as ViewResult;

            Assert.NotNull(result);
            Assert.Contains(controller.ModelState.Values.SelectMany(v => v.Errors), e => e.ErrorMessage.Contains("Only Philadelphia University emails"));
        }

        [Fact]
        public void Register_Post_WithWeakPassword_NoUppercase_ShowsError()
        {
            // Validation is traditionally tested via ModelState checks or Attribute validation
            // We simulate this as requested by asserting the DataAnnotations directly via the model
            var model = new RegisterViewModel { Password = "welcome@2024" };
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, results, true);
            
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("uppercase letter"));
        }

        [Fact]
        public void Register_Post_WithWeakPassword_NoNumber_ShowsError()
        {
            var model = new RegisterViewModel { Password = "Welcome@" };
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, results, true);
            
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("number"));
        }

        [Fact]
        public void Register_Post_WithWeakPassword_NoSpecialChar_ShowsError()
        {
            var model = new RegisterViewModel { Password = "Welcome2024" };
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, results, true);
            
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("special character"));
        }

        [Fact]
        public void Register_Post_WithWeakPassword_TooShort_ShowsError()
        {
            var model = new RegisterViewModel { Password = "Pass@1" };
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, results, true);
            
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("at least 8 characters"));
        }

        [Fact]
        public void Register_Post_WithMismatchedPasswords_ShowsError()
        {
            var model = new RegisterViewModel { Password = "Password123!", ConfirmPassword = "Password!23" };
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model);
            var results = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, results, true);
            
            Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("do not match"));
        }

        [Fact]
        public async Task Register_Post_ExtractsUniversityIDFromEmail()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new RegisterViewModel { Email = "202210456@philadelphia.edu.jo", Password = "Password123!" };
            await controller.Register(model);

            var user = dbContext.Users.First();
            Assert.Equal("202210456", user.UniversityID);
        }

        [Fact]
        public async Task Register_Post_SanitizesInput_TrimsName()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new RegisterViewModel { Email = "trim@philadelphia.edu.jo", Name = " John Doe ", Password = "Password123!" };
            await controller.Register(model);

            var user = dbContext.Users.First();
            Assert.Equal("John Doe", user.Name);
        }

        [Fact]
        public async Task Register_Post_SanitizesInput_LowercasesEmail()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new RegisterViewModel { Email = "UPPERCASE@PHILADELPHIA.EDU.JO", Password = "Password123!" };
            await controller.Register(model);

            var user = dbContext.Users.First();
            Assert.Equal("uppercase@philadelphia.edu.jo", user.Email);
        }

        [Fact]
        public async Task Register_Post_Awards50Points()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new RegisterViewModel { Email = "points@philadelphia.edu.jo", Password = "Password123!" };
            await controller.Register(model);

            Assert.Equal(50, dbContext.Users.First().Points);
        }

        [Fact]
        public async Task Register_Post_AssignsStudentRole()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new RegisterViewModel { Email = "role@philadelphia.edu.jo", Password = "Password123!" };
            await controller.Register(model);

            Assert.Equal("Student", dbContext.Users.First().Role);
        }

        [Fact]
        public async Task Register_Post_HashesPassword_WithBCrypt()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new RegisterViewModel { Email = "hash@philadelphia.edu.jo", Password = "Welcome@2024" };
            await controller.Register(model);

            var hash = dbContext.Users.First().PasswordHash;
            Assert.StartsWith("$2a$", hash);
            Assert.NotEqual("Welcome@2024", hash);
        }

        [Fact]
        public async Task Register_Post_SetIsDeletedToFalse()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new RegisterViewModel { Email = "delete@philadelphia.edu.jo", Password = "Password123!" };
            await controller.Register(model);

            Assert.False(dbContext.Users.First().IsDeleted);
        }

        [Fact]
        public async Task Register_Post_SetsCreatedAtToNow()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var model = new RegisterViewModel { Email = "time@philadelphia.edu.jo", Password = "Password123!" };
            await controller.Register(model);

            var date = dbContext.Users.First().CreatedAt;
            var diff = System.DateTime.Now - date;
            Assert.True(diff.TotalMinutes < 1);
        }

        [Fact]
        public async Task Register_Post_WithDatabaseError_ShowsFriendlyError()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            TestHelpers.SetControllerContext(controller);

            var result = await controller.Register(null) as ViewResult;
            Assert.Contains(controller.ModelState.Values.SelectMany(v => v.Errors), e => e.ErrorMessage.Contains("error occurred"));
        }

        [Fact]
        public async Task Register_Post_WithInvalidModelState_ReturnsView()
        {
            var dbContext = TestHelpers.CreateInMemoryDbContext();
            var controller = new LoginController(dbContext);
            controller.ModelState.AddModelError("Name", "Required");

            var result = await controller.Register(new RegisterViewModel()) as ViewResult;
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
        }
    }
}
