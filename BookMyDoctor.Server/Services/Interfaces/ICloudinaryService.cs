using CloudinaryDotNet.Actions;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadProfilePhotoAsync(IFormFile file, string userId);
        Task<RawUploadResult> UploadDocumentAsync(IFormFile file, string userId, string documentType);
        Task<RawUploadResult> UploadInvoicePdfAsync(IFormFile file, string invoiceId);
        Task<DeletionResult> DeleteFileAsync(string publicId);
        string GetOptimizedUrl(string publicId, int width = 300, int height = 300);
    }
}