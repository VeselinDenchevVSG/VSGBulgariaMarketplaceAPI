namespace VSGBulgariaMarketplace.Application.Helpers.Validators.Helpers
{
    using VSGBulgariaMarketplace.Application.Services.HelpServices;

    internal class EnumValidationHelper
    {
        internal static bool BeValid<T>(string name) where T : Enum
        {
            try
            {
                EnumService.GetEnumValueFromDisplayName<T>(name);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
