using BookMyDoctor.Server.DTOs;
using CloudinaryDotNet.Actions;

namespace BookMyDoctor.Server.Extentions
{
    public static class CloudinaryExtensions
    {
        public static FileUploadResponseDTO ToFileUploadResponse(this ImageUploadResult result, string message = "File uploaded successfully")
        {
            return new FileUploadResponseDTO
            {
                Success = result.Error == null,
                PublicId = result.PublicId,
                Url = result.Url?.ToString(),
                SecureUrl = result.SecureUrl?.ToString(),
                FileSize = result.Bytes,
                Format = result.Format,
                Message = result.Error?.Message ?? message
            };
        }

        public static FileUploadResponseDTO ToFileUploadResponse(this RawUploadResult result, string message = "File uploaded successfully")
        {
            return new FileUploadResponseDTO
            {
                Success = result.Error == null,
                PublicId = result.PublicId,
                Url = result.Url?.ToString(),
                SecureUrl = result.SecureUrl?.ToString(),
                FileSize = result.Bytes,
                Format = result.Format,
                Message = result.Error?.Message ?? message
            };
        }

        public static bool IsValidImageFile(this IFormFile file)
        {
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            return allowedTypes.Contains(file.ContentType.ToLower());
        }

        public static bool IsValidDocumentFile(this IFormFile file)
        {
            var allowedTypes = new[] { "application/pdf", "image/jpeg", "image/jpg", "image/png" };
            return allowedTypes.Contains(file.ContentType.ToLower());
        }

        public static bool IsValidPdfFile(this IFormFile file)
        {
            return file.ContentType.ToLower() == "application/pdf";
        }
    }
}