namespace VSGBulgariaMarketplace.Application.Helpers.Validators.CustomValidators
{
    using FluentValidation;

    using VSGBulgariaMarketplace.Application.Constants;

    internal static class NotEmptyWithMessageValidator
    {
        public static IRuleBuilderOptions<T, string> NotEmptyWithMessage<T, TElement, TEntity>(this IRuleBuilder<T, string> ruleBuilder, 
                                                                                                string message = ValidationConstant.NOT_EMPTY_ERROR_MESSAGE)
        {
            if (message == ValidationConstant.NOT_EMPTY_ERROR_MESSAGE)
            {
                message = FormatNotEmptyMessage(typeof(TEntity));
            }

            return ruleBuilder.NotEmpty().WithMessage(message);
        }

        public static IRuleBuilderOptions<T, int> NotEmptyWithMessage<T, TElement, TEntity>(this IRuleBuilder<T, int> ruleBuilder,
                                                                                                string message = ValidationConstant.NOT_EMPTY_ERROR_MESSAGE)
        {
            if (message == ValidationConstant.NOT_EMPTY_ERROR_MESSAGE)
            {
                message = FormatNotEmptyMessage(typeof(TEntity));
            }

            return ruleBuilder.NotEmpty().WithMessage(message);
        }

        public static IRuleBuilderOptions<T, short> NotEmptyWithMessage<T, TElement, TEntity>(this IRuleBuilder<T, short> ruleBuilder, 
                                                                                                string message = ValidationConstant.NOT_EMPTY_ERROR_MESSAGE)
        {
            if (message == ValidationConstant.NOT_EMPTY_ERROR_MESSAGE)
            {
                message = FormatNotEmptyMessage(typeof(TEntity));
            }

            return ruleBuilder.NotEmpty().WithMessage(message);
        }

        public static IRuleBuilderOptions<T, decimal> NotEmptyWithMessage<T, TElement, TEntity>(this IRuleBuilder<T, decimal> ruleBuilder, 
                                                                                                string message = ValidationConstant.NOT_EMPTY_ERROR_MESSAGE)
        {
            if (message == ValidationConstant.NOT_EMPTY_ERROR_MESSAGE)
            {
                message = FormatNotEmptyMessage(typeof(TEntity));
            }

            return ruleBuilder.NotEmpty().WithMessage(message);
        }

        private static string FormatNotEmptyMessage(Type type) => $"{type.Name} {ValidationConstant.NOT_EMPTY_ERROR_MESSAGE}";
    }
}