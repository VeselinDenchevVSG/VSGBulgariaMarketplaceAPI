namespace VSGBulgariaMarketplace.Application.Helpers.Validators
{
    using FluentValidation;

    using Microsoft.AspNetCore.Http;

    public class ImageFileValidator : AbstractValidator<IFormFile>
    {
        private const int MAX_FILE_SIZE_IN_MB = 5;

        private static readonly string[] allowedExtensions = new string[]
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".svg",
            ".webp",
            ".apng",
            ".avif"
        };

        public ImageFileValidator()
        {
            RuleFor(x => x.Length).NotEmpty().WithMessage("File cannot be empty")
                                    .LessThanOrEqualTo(1024 * 1024 * MAX_FILE_SIZE_IN_MB).WithMessage($"File size must be less than {MAX_FILE_SIZE_IN_MB} MB");

            RuleFor(x => x.FileName).Must(HaveAllowedExtension).WithMessage($"File must have {string.Join(", ", allowedExtensions)} extension");
        }

        private bool HaveAllowedExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            bool haveAllowedExtension = allowedExtensions.Contains(extension);

            return haveAllowedExtension;
        }
    }
}
