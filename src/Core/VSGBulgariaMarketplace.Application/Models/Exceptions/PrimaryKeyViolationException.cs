namespace VSGBulgariaMarketplace.Application.Models.Exceptions
{
    using System;

    public class PrimaryKeyViolationException : Exception
    {
        public PrimaryKeyViolationException(string? message)
            : base(message)
        {
        }
    }
}
