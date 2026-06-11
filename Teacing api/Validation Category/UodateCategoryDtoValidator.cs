
using FluentValidation;
namespace Teacing_api.Validation_Category
{
    public class UodateCategoryDtoValidator: AbstractValidator<UpdateCategoryDto>
    {
        public UodateCategoryDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Некорректно задан ID");
            RuleFor(x => x.Name)
                .MaximumLength(50).WithMessage("Имя не должно превышать 50 символов")
                .NotEmpty().WithMessage("Имя не должно быть пустым");
        }

    }
}
