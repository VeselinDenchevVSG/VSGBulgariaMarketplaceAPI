namespace VSGBulgariaMarketplace.Application.Helpers.Validators.ItemValidators
{
    using FluentValidation;

    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Domain.Enums;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Application.Helpers.Validators.CustomValidators;
    using VSGBulgariaMarketplace.Application.Helpers.Validators.Helpers;

    using static VSGBulgariaMarketplace.Application.Constants.ValidationConstant;

    public class UpdateItemDtoValidator : AbstractValidator<UpdateItemDto>
    {
        public UpdateItemDtoValidator()
        {
            RuleFor(i => i.Code).NotEmptyWithMessage<UpdateItemDto, string, Item>(ITEM_CODE)
                                .MaximumLength(ITEM_CODE_MAX_STRING_LENGTH)
                                .WithMessage(string.Format(ITEM_PROPERTY_CAN_NOT_BE_LONGER_THAN_ERROR_MESSAGE_TEMPLATE,
                                                            ITEM_CODE, ITEM_CODE_MAX_STRING_LENGTH));

            RuleFor(i => i.Name).NotEmptyWithMessage<UpdateItemDto, string, Item>(ITEM_NAME)
                                .MaximumLength(ITEM_CODE_MAX_STRING_LENGTH)
                                .WithMessage(string.Format(ITEM_PROPERTY_CAN_NOT_BE_LONGER_THAN_ERROR_MESSAGE_TEMPLATE,
                                                            ITEM_NAME, ITEM_NAME_MAX_STRING_LENGTH));

            RuleFor(i => i.Category).Must(EnumValidationHelper.BeValid<Category>).WithMessage(INVALID_ITEM_CATEGORY_ERROR_MESSAGE);

            RuleFor(i => i.Quantity).NotEmptyWithMessage<UpdateItemDto, short, Item>(ITEM_QUANTITY_COMBINED)
                                            .InclusiveBetween(ITEM_QUANTITY_MIN_VALUE, ITEM_QUANTITY_MAX_VALUE)
                                            .WithMessage(string.Format(ITEM_PROPERTY_MUST_BE_BETWEEN_MIN_AND_MAX_VALUE_ERROR_MESSAGE_TEMPLATE,
                                                                        ITEM_QUANTITY_COMBINED, ITEM_QUANTITY_MIN_VALUE,
                                                                        ITEM_QUANTITY_MAX_VALUE));

            RuleFor(i => i.Price).PrecisionScale(ITEM_PRICE_PRECISION, ITEM_PRICE_SCALE, false)
                                            .WithMessage(ITEM_PRECISION_SCALE_ERROR_MESSAGE);

            When(i => i.AvailableQuantity.HasValue, () =>
            {
                RuleFor(i => i.AvailableQuantity)
                             .InclusiveBetween(ITEM_QUANTITY_MIN_VALUE, ITEM_QUANTITY_MAX_VALUE)
                             .WithMessage(string.Format(ITEM_PROPERTY_MUST_BE_BETWEEN_MIN_AND_MAX_VALUE_ERROR_MESSAGE_TEMPLATE,
                                                        ITEM_AVAILABLE_QUANTITY,
                                                        ITEM_QUANTITY_MIN_VALUE, ITEM_QUANTITY_MAX_VALUE));
            });

            When(i => i.QuantityForSale.HasValue, () =>
            {
                RuleFor(i => i.QuantityForSale).InclusiveBetween(ITEM_QUANTITY_MIN_VALUE,
                                ITEM_QUANTITY_MAX_VALUE)
                                .WithMessage(string.Format(ITEM_PROPERTY_MUST_BE_BETWEEN_MIN_AND_MAX_VALUE_ERROR_MESSAGE_TEMPLATE,
                                                            ITEM_QUANTITY_FOR_SALE, ITEM_QUANTITY_MIN_VALUE,
                                                            ITEM_QUANTITY_MAX_VALUE));

                RuleFor(i => i.Price).NotEmptyWithMessage<UpdateItemDto, decimal?, Item>(ITEM_PRICE,
                                                                                    ITEM_PRICE_MUST_NOT_BE_EMPTY_WHEN_IT_HAS_QUANTITY_FOR_SALE_ERROR_MESSAGE)
                                                .GreaterThan(ITEM_PRICE_LOWER_BOUNDARY)
                                                .WithMessage(string.Format(ITEM_PRICE_MUST_BE_GREATER_THAN_TEMPLATE, ITEM_PRICE_LOWER_BOUNDARY))
                                                .LessThanOrEqualTo(ITEM_PRICE_MAX_VALUE);

                When(i => i.AvailableQuantity.HasValue, () =>
                {
                    RuleFor(i => i.Quantity).GreaterThanOrEqualTo(i => (short)(i.QuantityForSale.Value + i.AvailableQuantity.Value))
                                .WithMessage(
                        ITEM_QUANTITY_COMBINED_MUST_BE_GREATER_THAN_OR_EQUAL_TO_THE_SUM_OF_QUANTITY_FOR_SALE_AND_AVAILABLE_QUANTITY_ERROR_MESSAGE);
                })
                .Otherwise(() =>
                {
                    RuleFor(i => i.Quantity).GreaterThanOrEqualTo(i => i.QuantityForSale.Value)
                                                        .WithMessage(string.Format(
                                                            ITEM_QUANTITY_COMBINED_MUST_BE_GREATER_THAN_OR_EQUAL_TO_QUANTITY_ERROR_MESSAGE_TEMPLATE,
                                                            ITEM_QUANTITY_FOR_SALE));
                });
            })
            .Otherwise(() =>
            {
                When(i => i.Price.HasValue, () =>
                {
                    RuleFor(i => i.Price).GreaterThan(ITEM_PRICE_LOWER_BOUNDARY)
                                                    .WithMessage(string.Format(ITEM_PRICE_MUST_BE_GREATER_THAN_TEMPLATE,
                                                                                ITEM_PRICE_LOWER_BOUNDARY));
                });

                When(i => i.AvailableQuantity.HasValue, () =>
                {
                    RuleFor(i => i.Quantity).GreaterThanOrEqualTo(i => i.AvailableQuantity.Value)
                                                        .WithMessage(string.Format(
                                                            ITEM_QUANTITY_COMBINED_MUST_BE_GREATER_THAN_OR_EQUAL_TO_QUANTITY_ERROR_MESSAGE_TEMPLATE,
                                                            ITEM_AVAILABLE_QUANTITY));
                });
            });

            RuleFor(i => i.Description).MaximumLength(ITEM_DESCRIPTION_MAX_STRING_LENGTH)
                                                .WithMessage(string.Format(ITEM_PROPERTY_CAN_NOT_BE_LONGER_THAN_ERROR_MESSAGE_TEMPLATE,
                                                                            ITEM_DESCRIPTION,
                                                                            ITEM_DESCRIPTION_MAX_STRING_LENGTH));

            When(i => i.Image is not null, () =>
            {
                RuleFor(i => i.Image).SetValidator(new ImageFileValidator());
            });

            RuleFor(i => i.Location).Must(EnumValidationHelper.BeValid<Location>)
                                                .WithMessage(ITEM_LOCATION_MUST_BE_IN_THE_SPECIFIED_ONES_ERROR_MESSAGE);
        }
    }
}