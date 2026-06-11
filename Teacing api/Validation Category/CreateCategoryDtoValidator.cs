using FluentValidation;
using Teacing_api.Validation;

namespace Teacing_api.Validation_Category
    
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(50).WithMessage("Имя не должно превышать 50 символов")
                .NotEmpty().WithMessage("Имя не должно быть пустым");
        }

    }
}
