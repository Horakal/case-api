using FluentAssertions;
using FluentValidation.TestHelper;
using RestApiCase.Application.Tasks.DTOS.Requests;
using RestApiCase.Application.Tasks.Validators;
using Xunit;

namespace RestApiCase.Tests.Application
{
    public class UpdateTaskValidatorTests
    {
        private readonly UpdateTaskValidator _validator = new();

        [Fact]
        public void Valid_Request_Should_Pass()
        {
            var request = new UpdateTask { Id = Guid.NewGuid() };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Empty_Id_Should_Fail()
        {
            var request = new UpdateTask { Id = Guid.Empty };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("Id is required.");
        }
    }
}
