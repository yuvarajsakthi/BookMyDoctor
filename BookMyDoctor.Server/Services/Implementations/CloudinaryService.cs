using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using BookMyDoctor.Server.Services.Interfaces;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
            if (string.IsNullOrEmpty(cloudinaryUrl))
                throw new InvalidOperationException("CLOUDINARY_URL environment variable is not set");

            _cloudinary = new Cloudinary(cloudinaryUrl);
        }

        public async Task<ImageUploadResult> UploadProfilePhotoAsync(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                throw new ArgumentException("Only JPEG, PNG, and WebP images are allowed");

            if (file.Length > 5 * 1024 * 1024) // 5MB limit
                throw new ArgumentException("File size must be less than 5MB");

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"profile_photos/{userId}_{Guid.NewGuid()}",
                Folder = "bookmydoctor/profiles",
                Transformation = new Transformation()
                    .Width(400).Height(400)
                    .Crop("fill")
                    .Quality("auto")
                    .FetchFormat("auto"),
                Overwrite = false
            };

            return await _cloudinary.UploadAsync(uploadParams);
        }

        public async Task<RawUploadResult> UploadDocumentAsync(IFormFile file, string userId, string documentType)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            var allowedTypes = new[] { "application/pdf", "image/jpeg", "image/jpg", "image/png" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                throw new ArgumentException("Only PDF and image files are allowed");

            if (file.Length > 10 * 1024 * 1024) // 10MB limit
                throw new ArgumentException("File size must be less than 10MB");

            using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"documents/{userId}_{documentType}_{Guid.NewGuid()}",
                Folder = "bookmydoctor/documents",
                Overwrite = false
            };

            return await _cloudinary.UploadAsync(uploadParams);
        }

        public async Task<RawUploadResult> UploadInvoicePdfAsync(IFormFile file, string invoiceId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required");

            if (file.ContentType.ToLower() != "application/pdf")
                throw new ArgumentException("Only PDF files are allowed for invoices");

            if (file.Length > 5 * 1024 * 1024) // 5MB limit
                throw new ArgumentException("File size must be less than 5MB");

            using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"invoices/invoice_{invoiceId}_{DateTime.UtcNow:yyyyMMdd}",
                Folder = "bookmydoctor/invoices",
                Overwrite = true // Allow overwriting invoices
            };

            return await _cloudinary.UploadAsync(uploadParams);
        }

        public async Task<DeletionResult> DeleteFileAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                throw new ArgumentException("Public ID is required");

            var deletionParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deletionParams);
        }

        public string GetOptimizedUrl(string publicId, int width = 300, int height = 300)
        {
            if (string.IsNullOrEmpty(publicId))
                return string.Empty;

            return _cloudinary.Api.UrlImgUp
                .Transform(new Transformation()
                    .Width(width).Height(height)
                    .Crop("fill")
                    .Quality("auto")
                    .FetchFormat("auto"))
                .BuildUrl(publicId);
        }
    }
}