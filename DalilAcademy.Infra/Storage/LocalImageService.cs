using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Storage;
using Microsoft.AspNetCore.Hosting;

namespace DalilAcademy.Infra.Storage
{
    public class LocalImageService(IWebHostEnvironment env) : IImageService
    {
        private const long MaxBytes = 2 * 1024 * 1024;   // 2 MB
        private static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        private string ImagesFolder =>
            Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "images");

        public async Task<string> SaveCourseImageAsync(
            Stream content, string fileName, long length, CancellationToken ct = default)
        {
            if (length == 0)
                throw new BadRequestException("The file is empty.");

            if (length > MaxBytes)
                throw new BadRequestException("The file is too large. Maximum size is 2 MB.");

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new BadRequestException("Only .jpg, .jpeg, .png and .webp files are allowed.");

            Directory.CreateDirectory(ImagesFolder);

            // اسم عشوائي: بيمنع تعارض الأسماء ومحاولات ../
            var newName = $"{Guid.NewGuid()}{extension}";

            await using var fileStream = File.Create(Path.Combine(ImagesFolder, newName));
            await content.CopyToAsync(fileStream, ct);

            return $"/images/{newName}";
        }

        public void DeleteCourseImage(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;

            // Path.GetFileName بيمنع أي محاولة تطلع برّا مجلد الصور
            var path = Path.Combine(ImagesFolder, Path.GetFileName(imageUrl));
            if (File.Exists(path)) File.Delete(path);
        }
    }
}