namespace VSGBulgariaMarketplace.Application.Helpers.Validators.ItemValidators
{
    using FluentValidation;

    using Microsoft.AspNetCore.Http;

    using static VSGBulgariaMarketplace.Application.Constants.ValidationConstant;

    public class ImageFileValidator : AbstractValidator<IFormFile>
    {
        private static readonly string[] allowedExtensions = new string[]
        {
            JPG_FILE_EXTENSION,
            JPEG_FILE_EXTENSION,
            PNG_FILE_EXTENSION,
            GIF_FILE_EXTENSION,
            WEBP_FILE_EXTENSION,
            APNG_FILE_EXTENSION,
            AVIF_FILE_EXTENSION
        };

        public ImageFileValidator()
        {
            RuleFor(f => f.Length).NotEmpty().WithMessage(FILE_CAN_NOT_BE_EMPTY_ERROR_MESSAGE)
                                    .LessThanOrEqualTo(BYTES_IN_MB * MAX_FILE_SIZE_IN_MB)
                                    .WithMessage(string.Format(FILE_MUST_BE_LESS_THAN_MAX_FILE_SIZE_ERROR_MESSAGE, MAX_FILE_SIZE_IN_MB));

            RuleFor(f => f.FileName).Must(HaveAllowedExtension)
                                                .WithMessage(string.Format(FILE_MUST_HAVE_ALLOWED_EXTENSION_ERROR_MESSAGE, 
                                                                        string.Join(", ", allowedExtensions)));
        }

        private bool HaveAllowedExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            bool haveAllowedExtension = allowedExtensions.Contains(extension);

            return haveAllowedExtension;
        }
    }
}