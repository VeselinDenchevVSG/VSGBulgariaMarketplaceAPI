namespace VSGBulgariaMarketplace.Application.Models.Exceptions
{
    public class ItemAlreadyExistsException : Exception
    {
        public ItemAlreadyExistsException(string? message)
            : base(message)
        {
        }
    }
}
