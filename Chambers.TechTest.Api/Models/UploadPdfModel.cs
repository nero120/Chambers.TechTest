using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Chambers.TechTest.Api.Models
{
    public class UploadPdfModel : IValidatableObject
    {
        [Required(ErrorMessage = "A file must be provided")]
        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var file = ((UploadPdfModel)validationContext.ObjectInstance).File;

            if (!FileIsPdf(file))
            {
                yield return new ValidationResult("File must be a PDF", new[] { nameof(UploadPdfModel) });
            }

            if (FileSizeLimitExceeded(file))
            {
                yield return new ValidationResult("File size must be less than 5MB", new[] { nameof(UploadPdfModel) });
            }
        }

        protected bool FileIsPdf(IFormFile file)
        {
            return file.ContentType.Equals("application/pdf") && Path.GetExtension(file.FileName).ToUpper().Equals(".PDF");
        }

        protected bool FileSizeLimitExceeded(IFormFile file)
        {
            return file.Length > Constants.MaxUploadFileSize;
        }
    }
}
