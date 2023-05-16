namespace VSGBulgariaMarketplace.Application.Models.Exceptions
{
    using System;
    using System.Net;

    internal class HttpException : Exception
    {
        public HttpException(string exceptionMessage, HttpStatusCode statusCode)
            : base(exceptionMessage)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }
    }
}
