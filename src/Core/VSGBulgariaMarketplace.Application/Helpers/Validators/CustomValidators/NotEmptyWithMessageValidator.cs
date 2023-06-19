namespace VSGBulgariaMarketplace.Application.Helpers.Validators.CustomValidators
{
    using FluentValidation;

    using VSGBulgariaMarketplace.Application.Constants;

    internal static class NotEmptyWithMessageValidator
    {
        public static IRuleBuilderOptions<T, string> NotEmptyWithMessage<T, TElement, TEntity>(this IRuleBuilder<T, string> ruleBuilder, string propertyName, string message = null)
        {
            message = NotEmptyWithMessageBase<TEntity>(propertyName, message);

            return ruleBuilder.NotEmpty().WithMessage(message);
        }

        public static IRuleBuilderOptions<T, int> NotEmptyWithMessage<T, TElement, TEntity>(this IRuleBuilder<T, int> ruleBuilder, string propertyName, string message = null)
        {
            message = NotEmptyWithMessageBase<TEntity>(propertyName, message);

            return ruleBuilder.NotEmpty().WithMessage(message);
        }

        public static IRuleBuilderOptions<T, short> NotEmptyWithMessage<T, TElement, TEntity>(this IRuleBuilder<T, short> ruleBuilder, string propertyName, string message = null)
        {
            message = NotEmptyWithMessageBase<TEntity>(propertyName, message);

            return ruleBuilder.NotEmpty().WithMessage(message);
        }

        public static IRuleBuilderOptions<T, decimal?> NotEmptyWithMessage<T, TElement, TEntity>(this IRuleBuilder<T, decimal?> ruleBuilder, string propertyName, 
                                                                                                    string message = null)
        {
            message = NotEmptyWithMessageBase<TEntity>(propertyName, message);

            return ruleBuilder.NotEmpty().WithMessage(message);
        }

        private static string NotEmptyWithMessageBase<TEntity>(string propertyName, string message = null)
        {
            if (message is null)
            {
                message = FormatNotEmptyMessage(typeof(TEntity), propertyName);
            }

            return message;
        }

        private static string FormatNotEmptyMessage(Type type, string propertyName)
        {
            string message = string.Format(ValidationConstant.NOT_EMPTY_ERROR_MESSAGE,$"{type.Name} {propertyName}");

            return message;
        }
    }
}