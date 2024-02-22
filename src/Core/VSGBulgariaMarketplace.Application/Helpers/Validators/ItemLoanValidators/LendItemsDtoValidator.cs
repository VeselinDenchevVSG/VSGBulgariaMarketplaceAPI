namespace VSGBulgariaMarketplace.Application.Helpers.Validators.ItemLoanValidators
{
    using FluentValidation;

    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos;

    using static VSGBulgariaMarketplace.Application.Constants.ValidationConstant;

    public class LendItemsDtoValidator : AbstractValidator<LendItemsDto>
    {
        public LendItemsDtoValidator()
        {
            RuleFor(li => li.Email).NotEmpty().Matches(VSG_EMAIL_REGEX_PATTERN)
                                            .WithMessage(INVALID_EMAIL_FORMAT_ERROR_MESSAGE);

            RuleFor(li => li.Quantity).InclusiveBetween(LEND_ITEMS_MIN_QUANTITY, LEND_ITEMS_MAX_QUANTITY)
                                                .WithMessage(string.Format(ITEM_PROPERTY_MUST_BE_BETWEEN_MIN_AND_MAX_VALUE_ERROR_MESSAGE_TEMPLATE,
                                                                            ITEM_LENT_QUANTITY, LEND_ITEMS_MIN_QUANTITY, 
                                                                            LEND_ITEMS_MAX_QUANTITY));
        }
    }
}
