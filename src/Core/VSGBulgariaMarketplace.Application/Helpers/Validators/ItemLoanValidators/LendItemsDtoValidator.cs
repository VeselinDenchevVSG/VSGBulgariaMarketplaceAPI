namespace VSGBulgariaMarketplace.Application.Helpers.Validators.ItemLoanValidators
{
    using FluentValidation;

    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Dtos;

    public class LendItemsDtoValidator : AbstractValidator<LendItemsDto>
    {
        private const string VSG_EMAIL_REGEX_PATTERN = "[\\w]+@vsgbg\\.com$";

        public LendItemsDtoValidator()
        {
            RuleFor(li => li.Email).NotEmpty().Matches(VSG_EMAIL_REGEX_PATTERN).WithMessage("Invalid email format!");

            RuleFor(li => li.Quantity).InclusiveBetween((short)0, short.MaxValue)
                                                .WithMessage($"Item loan quantity combined must be between 0 and {short.MaxValue}!");
        }
    }
}
