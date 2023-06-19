namespace VSGBulgariaMarketplace.Application.Helpers.Validators.Order
{
    using FluentValidation;

    using VSGBulgariaMarketplace.Application.Constants;
    using VSGBulgariaMarketplace.Application.Models.Order.Dtos;

    internal class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(o => o.Quantity).InclusiveBetween((short)1, short.MaxValue)
                                                .WithMessage(string.Format(ValidationConstant.ORDER_QUANTITY_MUST_BE_BETWEEN_MIN_AND_MAX_VALUE_ERROR_MESSAGE, 
                                                                            ValidationConstant.ORDER_QUANTITY_MIN_VALUE, 
                                                                            ValidationConstant.ORDER_QUANTITY_MAX_VALUE));
        }
    }
}