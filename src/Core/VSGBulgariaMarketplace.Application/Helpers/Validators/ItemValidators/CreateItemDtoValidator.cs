namespace VSGBulgariaMarketplace.Application.Helpers.Validators.ItemValidators
{
    using FluentValidation;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Domain.Enums;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Application.Helpers.Validators.CustomValidators;
    using VSGBulgariaMarketplace.Application.Helpers.Validators.Helpers;
    using VSGBulgariaMarketplace.Application.Constants;

    public class CreateItemDtoValidator : AbstractValidator<CreateItemDto>
    {
        public CreateItemDtoValidator()
        {
            RuleFor(i => i.Code).NotEmptyWithMessage<CreateItemDto, string, Item>()
                                            .MaximumLength(ValidationConstant.ITEM_CODE_MAX_STRING_LENGTH)
                                            .WithMessage(string.Format(ValidationConstant.ITEM_PROPERTY_CAN_NOT_BE_LONGER_THAN_ERROR_MESSAGE_TEMPLATE,
                                                            ValidationConstant.ITEM_CODE, ValidationConstant.ITEM_CODE_MAX_STRING_LENGTH));

            RuleFor(i => i.Name).NotEmptyWithMessage<CreateItemDto, string, Item>()
                                .MaximumLength(ValidationConstant.ITEM_CODE_MAX_STRING_LENGTH)
                                .WithMessage(string.Format(ValidationConstant.ITEM_PROPERTY_CAN_NOT_BE_LONGER_THAN_ERROR_MESSAGE_TEMPLATE,
                                                            ValidationConstant.ITEM_NAME, ValidationConstant.ITEM_NAME_MAX_STRING_LENGTH));

            RuleFor(i => i.Price).NotEmptyWithMessage<CreateItemDto, decimal, Item>()
                                    .InclusiveBetween(ValidationConstant.ITEM_PRICE_MIN_VALUE, ValidationConstant.ITEM_PRICE_MAX_VALUE).WithMessage(string.Format(
                                                                                        ValidationConstant.ITEM_PROPERTY_MUST_BE_BETWEEN_MIN_AND_MAX_VALUE_ERROR_MESSAGE_TEMPLATE,
                                                                                        ValidationConstant.ITEM_PRICE, ValidationConstant.ITEM_PRICE_MIN_VALUE,
                                                                                        ValidationConstant.ITEM_PRICE_MAX_VALUE))
                                    .PrecisionScale(ValidationConstant.ITEM_PRICE_PRECISION, ValidationConstant.ITEM_PRICE_SCALE, false)
                                    .WithMessage(ValidationConstant.ITEM_PRECISION_SCALE_ERROR_MESSAGE);

            RuleFor(i => i.Category).NotEmptyWithMessage<CreateItemDto, string, Item>()
                                    .Must(EnumValidationHelper.BeValid<Category>).WithMessage(ValidationConstant.INVALID_ITEM_CATEGORY_ERROR_MESSAGE);

            RuleFor(i => i.Quantity).NotEmptyWithMessage<CreateItemDto, short, Item>()
                                            .InclusiveBetween(ValidationConstant.ITEM_QUANTITY_MIN_VALUE, ValidationConstant.ITEM_QUANTITY_MAX_VALUE)
                                            .WithMessage(string.Format(ValidationConstant.ITEM_PROPERTY_MUST_BE_BETWEEN_MIN_AND_MAX_VALUE_ERROR_MESSAGE_TEMPLATE,
                                            ValidationConstant.ITEM_QUANTITY_COMBINED, ValidationConstant.ITEM_QUANTITY_MIN_VALUE, 
                                            ValidationConstant.ITEM_QUANTITY_MAX_VALUE));

            When(i => i.AvailableQuantity.HasValue, () =>
            {
                RuleFor(i => i.AvailableQuantity)
                             .InclusiveBetween((short)0, short.MaxValue)
                             .WithMessage(string.Format(ValidationConstant.ITEM_PROPERTY_MUST_BE_BETWEEN_MIN_AND_MAX_VALUE_ERROR_MESSAGE_TEMPLATE,
                                                                        ValidationConstant.ITEM_AVAILABLE_QUANTITY,
                                                                        ValidationConstant.ITEM_QUANTITY_MIN_VALUE, ValidationConstant.ITEM_QUANTITY_MAX_VALUE));
            });

            When(i => i.QuantityForSale.HasValue, () =>
            {
                RuleFor(i => i.QuantityForSale).InclusiveBetween(ValidationConstant.ITEM_QUANTITY_MIN_VALUE,
                                ValidationConstant.ITEM_QUANTITY_MAX_VALUE)
                                .WithMessage(string.Format(ValidationConstant.ITEM_PROPERTY_MUST_BE_BETWEEN_MIN_AND_MAX_VALUE_ERROR_MESSAGE_TEMPLATE,
                                ValidationConstant.ITEM_QUANTITY_FOR_SALE, ValidationConstant.ITEM_QUANTITY_MIN_VALUE,
                                ValidationConstant.ITEM_QUANTITY_MAX_VALUE));

                When(i => i.AvailableQuantity.HasValue, () =>
                {
                    RuleFor(i => i.Quantity).GreaterThanOrEqualTo(i => (short)(i.QuantityForSale.Value + i.AvailableQuantity.Value))
                                .WithMessage(
                        ValidationConstant.ITEM_QUANTITY_COMBINED_MUST_BE_GREATER_THAN_OR_EQUAL_TO_THE_SUM_OF_QUANTITY_FOR_SALE_AND_AVAILABLE_QUANTITY_ERROR_MESSAGE);
                })
                .Otherwise(() => 
                {
                    RuleFor(i => i.Quantity).GreaterThanOrEqualTo(i => i.QuantityForSale.Value)
                                                        .WithMessage(string.Format(
                                                            ValidationConstant.ITEM_QUANTITY_COMBINED_MUST_BE_GREATER_THAN_OR_EQUAL_TO_QUANTITY_ERROR_MESSAGE_TEMPLATE, 
                                                            ValidationConstant.ITEM_QUANTITY_FOR_SALE));
                });
            })
            .Otherwise(() => 
            {
                When(i => i.AvailableQuantity.HasValue, () =>
                {
                    RuleFor(i => i.Quantity).GreaterThanOrEqualTo(i => i.AvailableQuantity.Value)
                                                        .WithMessage(string.Format(
                                                            ValidationConstant.ITEM_QUANTITY_COMBINED_MUST_BE_GREATER_THAN_OR_EQUAL_TO_QUANTITY_ERROR_MESSAGE_TEMPLATE,
                                                            ValidationConstant.ITEM_AVAILABLE_QUANTITY));
                });
            });

            RuleFor(i => i.Description).MaximumLength(ValidationConstant.ITEM_DESCRIPTION_MAX_STRING_LENGTH)
                                                .WithMessage(string.Format(ValidationConstant.ITEM_PROPERTY_CAN_NOT_BE_LONGER_THAN_ERROR_MESSAGE_TEMPLATE,
                                                                                        ValidationConstant.ITEM_DESCRIPTION, 
                                                                                        ValidationConstant.ITEM_DESCRIPTION_MAX_STRING_LENGTH));

            When(i => i.Image is not null, () =>
            {
                RuleFor(i => i.Image).SetValidator(new ImageFileValidator());
            });

            RuleFor(i => i.Location).NotEmptyWithMessage<CreateItemDto, string, Item>()
                                    .Must(EnumValidationHelper.BeValid<Location>)
                                    .WithMessage(ValidationConstant.ITEM_LOCATION_MUST_BE_IN_THE_SPECIFIED_ONES_ERROR_MESSAAGE);
        }
    }
}