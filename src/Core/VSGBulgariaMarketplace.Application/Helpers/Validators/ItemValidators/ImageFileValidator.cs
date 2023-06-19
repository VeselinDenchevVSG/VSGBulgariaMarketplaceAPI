namespace VSGBulgariaMarketplace.Application.Helpers.Validators.ItemValidators
{
    using FluentValidation;

    using Microsoft.AspNetCore.Http;

    using VSGBulgariaMarketplace.Application.Constants;

    public class ImageFileValidator : AbstractValidator<IFormFile>
    {
        private static readonly string[] allowedExtensions = new string[]
        {
            ValidationConstant.JPG_FILE_EXTENSION,
            ValidationConstant.JPEG_FILE_EXTENSION,
            ValidationConstant.PNG_FILE_EXTENSION,
            ValidationConstant.GIF_FILE_EXTENSION,
            ValidationConstant.WEBP_FILE_EXTENSION,
            ValidationConstant.APNG_FILE_EXTENSION,
            ValidationConstant.AVIF_FILE_EXTENSION
        };

        public ImageFileValidator()
        {
            RuleFor(f => f.Length).NotEmpty().WithMessage(ValidationConstant.FILE_CAN_NOT_BE_EMPTY_ERROR_MESSAGE)
                                    .LessThanOrEqualTo(ValidationConstant.BYTES_IN_MB * ValidationConstant.MAX_FILE_SIZE_IN_MB)
                                    .WithMessage(string.Format(ValidationConstant.FILE_MUST_BE_LESS_THAN_MAX_FILE_SIZE_ERROR_MESSAGE, ValidationConstant.MAX_FILE_SIZE_IN_MB));

            RuleFor(f => f.FileName).Must(HaveAllowedExtension)
                                                .WithMessage(string.Format(ValidationConstant.FILE_MUST_HAVE_ALLOWED_EXTENSION_ERROR_MESSAGE, 
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