using System.ComponentModel.DataAnnotations;

namespace Server.Models.Extensions;

public class AllowedImageFormatAttribute : ValidationAttribute
{
    private static readonly string[] _extensions = [
        ".png",
        ".jpeg",
        ".jpg",
    ];

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success!;

        IFormFile? file = (IFormFile) value;

        if (file is null)
            return new ValidationResult("The provided value is not a valid file.");

        var extension = Path.GetExtension(file.FileName);

        if (!_extensions.Contains(extension.ToLower()))
        {
            return new ValidationResult("Your image's filetype is not valid.");
        }

        if (file.Length <= 0)
            return new ValidationResult("The file is empty.");

        if (file.Length > 5 * 1024 * 1024)
            return new ValidationResult("The file is too large. Maximum size is 5 MB.");

        return ValidationResult.Success!;
    }
}
