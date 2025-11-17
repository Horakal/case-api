using FluentValidation;
using RestApiCase.Application.Tasks.DTOS.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Application.Tasks.Validators
{
    public class UpdateTaskValidator : AbstractValidator<UpdateTask>
    {
        public UpdateTaskValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Id is required.");
        }
    }
}