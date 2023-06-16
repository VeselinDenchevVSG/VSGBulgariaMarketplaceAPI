namespace VSGBulgariaMarketplace.Application.Models.Exceptions
{
    internal class ErrorModel
    {
        public ErrorModel(int code, string errorMessage)
        {
            this.Code = code;
            this.ErrorMessage = errorMessage;
        }

        public int Code { get; set; }

        public string ErrorMessage { get; set; }
    }
}
