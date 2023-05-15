namespace VSGBulgariaMarketplace.Application.Helpers.Validators.CustomValidators
{
    using FluentValidation;

    internal static class NotEmptyWithMessageValidator
    {
        private const string NOT_EMPTY_MESSAGE = " {PropertyName} can't be empty!";

        public static IRuleBuilderOptions<T, string> NotEmptyWithMessage<T, TElement, TEntity>(this IRuleBuilder<T, string> ruleBuilder, string message = NOT_EMPTY_MESSAGE)
        {
            if (message == NOT_EMPTY_MESSAGE)
            {
                message = FormatNotEmptyMessage(typeof(TEntity));
            }

            return ruleBuilder.NotEmpty().WithMessage(message);
        }

        public static IRuleBuilderOptions<T, int> NotEmptyWithMessage<T, TElement, TEntity>(this IRuleBuilder<T, int> ruleBuilder, string message = NOT_EMPTY_MESSAGE)
        {
            if (message == NOT_EMPTY_MESSAGE)
            {
                message = FormatNotEmptyMessage(typeof(TEntity));
            }

            return ruleBuilder.NotEmpty().WithMessage(message);
        }

        public static IRuleBuilderOptions<T, short> NotEmptyWithMessage<T, TElement, TEntity>(this IRuleBuilder<T, short> ruleBuilder, string message = NOT_EMPTY_MESSAGE)
        {
            if (message == NOT_EMPTY_MESSAGE)
            {
                message = FormatNotEmptyMessage(typeof(TEntity));
            }

            return ruleBuilder.NotEmpty().WithMessage(message);
        }

        public static IRuleBuilderOptions<T, decimal> NotEmptyWithMessage<T, TElement, TEntity>(this IRuleBuilder<T, decimal> ruleBuilder, string message = NOT_EMPTY_MESSAGE)
        {
            if (message == NOT_EMPTY_MESSAGE)
            {
                message = FormatNotEmptyMessage(typeof(TEntity));
            }

            return ruleBuilder.NotEmpty().WithMessage(message);
        }

        private static string FormatNotEmptyMessage(Type type) => type.Name + NOT_EMPTY_MESSAGE;
    }
}