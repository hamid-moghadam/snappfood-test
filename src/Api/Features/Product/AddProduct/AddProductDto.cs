using Api.Infrastructure.Persistence;
using FluentValidation;

namespace Api.Features.Product.AddProduct;

public record AddProductDto(string Title, uint Price, sbyte DiscountPercent);

public class AddProductDtoValidator : AbstractValidator<AddProductDto>
{
    public AddProductDtoValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty()
            .MaximumLength(Domain.Product.TitleMaxLength);

        RuleFor(v => v.DiscountPercent)
            .NotNull()
            .InclusiveBetween((sbyte)0, (sbyte)100);

        RuleFor(v => v.Price)
            .NotEmpty()
            .NotEqual((uint)0);
    }
}