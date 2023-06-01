﻿namespace VSGBulgariaMarketplace.Application.Helpers.Validators
{
    using FluentValidation;

    using VSGBulgariaMarketplace.Application.Helpers.Validators.CustomValidators;
    using VSGBulgariaMarketplace.Application.Helpers.Validators.ItemValidators;
    using VSGBulgariaMarketplace.Application.Models.Item.Dtos;
    using VSGBulgariaMarketplace.Application.Services.HelpServices;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Domain.Enums;

    public class UpdateItemDtoValidator : AbstractValidator<UpdateItemDto>
    {
        private const int ITEM_NAME_MAX_STRING_LENGTH = 120;

        public UpdateItemDtoValidator()
        {
            RuleFor(i => i.Code).InclusiveBetween(1, int.MaxValue).WithMessage($"Item code must be between 1 and {int.MaxValue}!");

            RuleFor(i => i.Name).NotEmptyWithMessage<UpdateItemDto, string, Item>()
                                .MaximumLength(ITEM_NAME_MAX_STRING_LENGTH).WithMessage($"Item name can't be logner than {ITEM_NAME_MAX_STRING_LENGTH}!");

            RuleFor(i => i.Price).NotEmptyWithMessage<UpdateItemDto, decimal, Item>()
                                    .InclusiveBetween(0m, decimal.MaxValue).WithMessage($"Item price must be between 0 and {decimal.MaxValue}!")
                                    .PrecisionScale(28, 2, false)
                                    .WithMessage("Item price must have no more than two digits after the decimal point and mustn't be longer than 28 digits in total"!);

            RuleFor(i => i.Category).NotEmptyWithMessage<UpdateItemDto, string, Item>()
                                    .Must(BeValidCategory).WithMessage($"Item category must be in the specified ones!");

            RuleFor(i => i.Quantity).NotEmptyWithMessage<UpdateItemDto, short, Item>()
                                            .InclusiveBetween((short)0, short.MaxValue).WithMessage($"Item quantity combined must be between 0 and {short.MaxValue}!");

            RuleFor(i => i.QuantityForSale).InclusiveBetween((short)0, short.MaxValue).WithMessage($"Item quantity for sale must be between 0 and {short.MaxValue}!")
                                            .LessThanOrEqualTo(i => i.Quantity).WithMessage("Item quantity for sale must be less than or equal to quantity combined!");

            RuleFor(i => i.Description).MaximumLength(1_000);

            When(i => i.Image is not null, () =>
            {
                RuleFor(i => i.Image).SetValidator(new ImageFileValidator());
            });
        }

        private bool BeValidCategory(string categoryName)
        {
            try
            {
                EnumService.GetEnumValueFromDisplayName<Category>(categoryName);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}