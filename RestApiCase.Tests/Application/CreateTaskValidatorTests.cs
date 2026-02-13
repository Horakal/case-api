using FluentAssertions;
using FluentValidation.TestHelper;
using RestApiCase.Application.Tasks.DTOS.Requests;
using RestApiCase.Application.Tasks.Validators;
using Xunit;

namespace RestApiCase.Tests.Application
{
    public class CreateTaskValidatorTests
    {
        private readonly CreateTaskValidator _validator = new();

        [Fact]
        public void Valid_Request_Should_Pass()
        {
            var request = new CreateTask { Title = "Valid Title", Description = "Valid Description" };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Empty_Title_Should_Fail()
        {
            var request = new CreateTask { Title = "", Description = "Valid Description" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title is required.");
        }

        [Fact]
        public void Null_Title_Should_Fail()
        {
            var request = new CreateTask { Title = null!, Description = "Valid Description" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Empty_Description_Should_Fail()
        {
            var request = new CreateTask { Title = "Valid Title", Description = "" };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage("Description is required.");
        }

        [Fact]
        public void Null_Description_Should_Fail()
        {
            var request = new CreateTask { Title = "Valid Title", Description = null! };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
    }
}
