namespace VSGBulgariaMarketplace.Application.Models.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string? message)
            : base(message)
        {
        }
    }
}