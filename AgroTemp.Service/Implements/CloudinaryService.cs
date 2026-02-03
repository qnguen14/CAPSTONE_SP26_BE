using AgroTemp.Service.Config.ApiModels;
using AgroTemp.Service.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace AgroTemp.Service.Implements
{
    public class CloudinaryService(IServiceProvider serviceProvider) : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary = new Cloudinary(CloudinarySetting.Instance.CloudinaryUrl);

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            Console.WriteLine("Uploading image to Cloudinary...");
            ImageValidate(file);
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Quality(80).Crop("fit"),
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
        }

        public async Task<List<string>> UploadMultipleImagesAsync(IFormFileCollection files)
        {
            Console.WriteLine($"Uploading {files.Count} images to Cloudinary");

            if (files == null || files.Count == 0)
            {
                throw new Exception("No files provided for upload.");
            }

            var uploadTasks = new List<Task<string>>();

            foreach (var file in files)
            {
                uploadTasks.Add(UploadImageAsync(file));
            }

            try
            {
                var results = await Task.WhenAll(uploadTasks);
                Console.WriteLine($"Successfully uploaded {results.Length} images");
                return results.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading multiple images: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<string> UploadVideoAsync(IFormFile file)
        {
            Console.WriteLine("Uploading video to Cloudinary");
            VideoValidate(file);

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Quality(80)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
        }

        public async Task<List<string>> UploadMultipleVideosAsync(IFormFileCollection files)
        {
            Console.WriteLine($"Uploading {files.Count} videos to Cloudinary");

            if (files == null || files.Count == 0)
            {
                throw new Exception("No files provided for upload.");
            }

            var uploadTasks = new List<Task<string>>();

            foreach (var file in files)
            {
                uploadTasks.Add(UploadVideoAsync(file));
            }

            try
            {
                var results = await Task.WhenAll(uploadTasks);
                Console.WriteLine($"Successfully uploaded {results.Length} videos");
                return results.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading multiple videos: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<string> UploadRawFileAsync(IFormFile file)
        {
            Console.WriteLine("Uploading raw file to Cloudinary");
            RawFileValidate(file);
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
        }

        public async Task<List<string>> UploadMultipleRawFilesAsync(IFormFileCollection files)
        {
            Console.WriteLine($"Uploading {files.Count} raw files to Cloudinary");

            if (files == null || files.Count == 0)
            {
                throw new Exception("No files provided for upload.");
            }

            var uploadTasks = new List<Task<string>>();

            foreach (var file in files)
            {
                uploadTasks.Add(UploadRawFileAsync(file));
            }

            try
            {
                var results = await Task.WhenAll(uploadTasks);
                Console.WriteLine($"Successfully uploaded {results.Length} raw files");
                return results.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading multiple raw files: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task DeleteAsync(string url)
        {
            Console.WriteLine($"Deleting file from Cloudinary. Url: {url}");

            var (publicId, resourceType) = UrlValidateAndDetectType(url);

            var deleteParams = new DeletionParams(publicId);

            if (resourceType == "video")
            {
                deleteParams.ResourceType = ResourceType.Video;
            }
            else if (resourceType == "raw")
            {
                deleteParams.ResourceType = ResourceType.Raw;
                deleteParams.Invalidate = true; // Invalidate cache for raw files
            }
            else
            {
                deleteParams.ResourceType = ResourceType.Image;
            }

            var result = await _cloudinary.DestroyAsync(deleteParams);
            if (result.Result != "ok")
            {
                Console.WriteLine($"Failed to delete {resourceType} file. Result: {result.Result}");
                throw new Exception($"Failed to delete {resourceType} file: {result.Result}");
            }
            Console.WriteLine($"Successfully deleted {resourceType} file");
        }

        public async Task DeleteMultipleAsync(IEnumerable<string> urls)
        {
            Console.WriteLine($"Deleting multiple files from Cloudinary. Count: {urls.Count()}");
            var deleteTasks = urls.Select(url => DeleteAsync(url));

            try
            {
                await Task.WhenAll(deleteTasks);
                Console.WriteLine($"Successfully deleted {urls.Count()} files");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting multiple files: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        private void RawFileValidate(IFormFile file)
        {
            // Allowed extensions
            var validExtensions = new[]
        {
            ".txt", ".doc", ".docx", ".pdf", ".xlsx", ".xls", ".ppt", ".pptx",
            ".rtf", ".odt", ".ods", ".odp", ".csv", ".xml", ".json", ".zip",
            ".rar", ".7z", ".tar", ".gz", ".sql", ".log", ".md", ".yaml", ".yml"
        };

            // Null check
            if (file == null || file.Length == 0)
            {
                throw new Exception("File is null or empty.");
            }

            // Get file extension and validate
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!validExtensions.Contains(fileExtension))
            {
                throw new Exception("Invalid document file type. Allowed types are: " + string.Join(", ", validExtensions));
            }
            // Check file size (max 50MB)
            if (file.Length > 50 * 1024 * 1024)
            {
                throw new Exception("File size exceeds the 20MB limit.");
            }
        }

        private void VideoValidate(IFormFile file)
        {
            // Allowewd extensions
            var validExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm", ".mkv", ".m4v" };

            // Null check
            if (file == null || file.Length == 0)
            {
                throw new Exception("File is null or empty.");
            }

            // Get file extension and validate
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!validExtensions.Contains(fileExtension))
            {
                throw new Exception("Invalid video file type. Allowed types are: " + string.Join(", ", validExtensions));
            }

            // Check file size (max 50MB)
            if (file.Length > 50 * 1024 * 1024)
            {
                throw new Exception("File size exceeds the 50MB limit.");
            }
        }

        private void ImageValidate(IFormFile file)
        {
            // Allowed extensions
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".jfif" };

            // Null check
            if (file == null || file.Length == 0)
            {
                throw new Exception("File is null or empty.");
            }

            // Get file extension and validate
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!validExtensions.Contains(fileExtension))
            {
                throw new Exception("Invalid image file type. Allowed types are: " + string.Join(", ", validExtensions));
            }

            // Check file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                throw new Exception("File size exceeds the 5MB limit.");
            }
        }

        private (string publicId, string resourceType) UrlValidateAndDetectType(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("URL is null or empty.");
            }

            // check if this is a valid URL
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new Exception("Invalid URL format.");
            }

            var uri = new Uri(url);

            var host = "res.cloudinary.com";
            if (!uri.Host.Equals(host, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Host required: {host}");
            }

            var cloudName = CloudinarySetting.Instance.CloudinaryUrl.Split("@").Last();
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 2 || !segments[0].Equals(cloudName, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"CloudName required: {cloudName}");
            }

            // Detect resource type from URL path
            if (segments.Length < 4 || segments[2] != "upload")
            {
                throw new Exception("Invalid Cloudinary URL format.");
            }

            var resourceType = segments[1].ToLower();
            if (resourceType != "image" && resourceType != "video" && resourceType != "raw")
            {
                throw new Exception("Unsupported resource type in URL.");
            }

            string publicId;

            if (resourceType == "raw")
            {
                // For raw files, keep publicId with extension
                var publicIdWithExtension = segments[^1];
                publicId = Path.GetFileName(publicIdWithExtension);

                var isValidPublicId = !string.IsNullOrEmpty(publicId) &&
                                      Regex.IsMatch(publicId, @"^[a-zA-Z0-9_\-\.]+$");

                if (!isValidPublicId)
                {
                    throw new Exception("Invalid public ID format in URL.");
                }
            }
            else
            {
                // For images and videos, extract public ID without extension
                var publicIdWithExtension = segments[^1];
                publicId = Path.GetFileNameWithoutExtension(publicIdWithExtension);

                var isValidPublicId = !string.IsNullOrEmpty(publicId) &&
                                      Regex.IsMatch(publicId, @"^[a-zA-Z0-9_\-]+$");

                if (!isValidPublicId)
                {
                    throw new Exception("Invalid public ID format in URL.");
                }
            }

            return (publicId, resourceType);
        }
    }
}
