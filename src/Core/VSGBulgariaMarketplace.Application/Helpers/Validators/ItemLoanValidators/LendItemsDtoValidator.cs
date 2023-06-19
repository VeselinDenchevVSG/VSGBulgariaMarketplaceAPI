namespace VSGBulgariaMarketplace.Application.Helpers.Validators.ItemLoanValidators
{
    using FluentValidation;

    using VSGBulgariaMarketplace.Application.Constants;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos;

    public class LendItemsDtoValidator : AbstractValidator<LendItemsDto>
    {
        public LendItemsDtoValidator()
        {
            RuleFor(li => li.Email).NotEmpty().Matches(ValidationConstant.VSG_EMAIL_REGEX_PATTERN)
                                            .WithMessage(ValidationConstant.INVALID_EMAIL_FORMAT_ERROR_MESSAGE);

            RuleFor(li => li.Quantity).InclusiveBetween(ValidationConstant.LEND_ITEMS_MIN_QUANTITY, ValidationConstant.LEND_ITEMS_MAX_QUANTITY)
                                                .WithMessage(string.Format(ValidationConstant.ITEM_PROPERTY_MUST_BE_BETWEEN_MIN_AND_MAX_VALUE_ERROR_MESSAGE_TEMPLATE,
                                                                            ValidationConstant.ITEM_LENT_QUANTITY, ValidationConstant.LEND_ITEMS_MIN_QUANTITY, 
                                                                            ValidationConstant.LEND_ITEMS_MAX_QUANTITY));
        }
    }
}
