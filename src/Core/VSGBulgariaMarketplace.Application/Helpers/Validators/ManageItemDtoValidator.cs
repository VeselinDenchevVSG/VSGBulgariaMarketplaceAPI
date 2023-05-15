namespace VSGBulgariaMarketplace.Application.Helpers.Validators
{
    using FluentValidation;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Domain.Enums;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Application.Helpers.Validators.CustomValidators;

    public class ManageItemDtoValidator : AbstractValidator<ManageItemDto>
    {
        private const int ITEM_NAME_MAX_STRING_LENGTH = 120;

        public ManageItemDtoValidator()
        {
            RuleFor(i => i.Code).InclusiveBetween(1, int.MaxValue).WithMessage($"Item code must be between 1 and {int.MaxValue}!");

            RuleFor(i => i.Name).NotEmptyWithMessage<ManageItemDto, string, Item>()
                                .MaximumLength(ITEM_NAME_MAX_STRING_LENGTH).WithMessage($"Item name can't be logner than {ITEM_NAME_MAX_STRING_LENGTH}!");

            RuleFor(i => i.Price).NotEmptyWithMessage<ManageItemDto, decimal, Item>()
                                    .InclusiveBetween(0m, decimal.MaxValue).WithMessage($"Item price must be between 0 and {decimal.MaxValue}!")
                                    .PrecisionScale(28, 2, false)
                                    .WithMessage("Item price must have no more than two digits after the decimal point and mustn't be longer than 28 digits in total"!);

            RuleFor(i => i.Category).NotEmptyWithMessage<ManageItemDto, string, Item>()
                                    .Must(BeValidCategory).WithMessage($"Item category must be in the specified ones!");

            RuleFor(i => i.QuantityCombined).NotEmptyWithMessage<ManageItemDto, short, Item>()
                                            .InclusiveBetween((short)0, short.MaxValue).WithMessage($"Item quantity combined must be between 0 and {short.MaxValue}!");

            RuleFor(i => i.QuantityForSale).InclusiveBetween((short)0, short.MaxValue).WithMessage($"Item quantity for sale must be between 0 and {short.MaxValue}!")
                                            .LessThanOrEqualTo(i => i.QuantityCombined).WithMessage("Item quantity for sale must be less than or equal to quantity combined!");

            RuleFor(i => i.Description).MaximumLength(1_000);
        }

        private bool BeValidCategory(string categoryName) => Enum.TryParse(categoryName, true, out Category category);
    }
}