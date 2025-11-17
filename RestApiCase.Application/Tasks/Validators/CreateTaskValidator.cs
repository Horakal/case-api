using FluentValidation;
using RestApiCase.Application.Tasks.DTOS.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Application.Tasks.Validators
{
    public class CreateTaskValidator : AbstractValidator<CreateTask>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");
        }
    }
}