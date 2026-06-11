using FluentValidation;
using Teacing_api.Models;

public class ProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public ProductDtoValidator()
    {
        // Правило для Email: не пустой и должен быть корректным адресом
        RuleFor(Product => Product.Name)
            .NotEmpty().WithMessage("Имя обязательно для заполнения.")
            .Length(3, 50).WithMessage("Длина названия от 3 до 50 символов"); ;

        // Правило для пароля: минимум 6 символов
        RuleFor(Product => Product.Price)
            .GreaterThan(0).WithMessage("Цена должна быть больше 0");
    }
}

