using FluentValidation;

namespace Api.Features.Product.IncreaseInventory;

public record IncreaseInventoryDto(uint Count);

public class IncreaseInventoryDtoValidator : AbstractValidator<IncreaseInventoryDto>
{
    public IncreaseInventoryDtoValidator()
    {
        RuleFor(x => x.Count)
            .NotEmpty()
            .NotEqual((uint)0);
    }
}