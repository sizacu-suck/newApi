using FluentValidation;
namespace Teacing_api.Validation

{
    public class UpdateProductDtoValidator: AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator ()
        {

            RuleFor(product => product.Name)
                .NotEmpty().WithMessage("Не может быть пустым");
            RuleFor(product => product.Price)
                .GreaterThan(0).WithMessage("Цена должна быть больше 0");
            RuleFor(product => product.Id)
                .GreaterThan(0).WithMessage("Id должно быть больше 0");
            RuleFor(Product => Product.CategoryId)
    .GreaterThan(0).WithMessage("Категория должна быть больше 0");
        }
    }
}
