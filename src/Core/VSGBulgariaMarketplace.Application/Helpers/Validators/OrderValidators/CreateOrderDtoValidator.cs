namespace VSGBulgariaMarketplace.Application.Helpers.Validators.Order
{
    using FluentValidation;

    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;

    internal class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(o => o.Quantity).InclusiveBetween((short)1, short.MaxValue).WithMessage($"Order quantity must be between 1 and {short.MaxValue}!");
        }
    }
}