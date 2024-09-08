using TaskManagement.Core.Consts;

namespace TaskManagement.Services
{
    public class FileService : IFileService
    {
        public bool AllowFile(IFormFile? file) => file is not { Length: > FileConst.MaxFileSize };

        public async Task<bool> AllowFileExtension(IFormFile? file)
        {
            if (file is null) return true;

            var allowedHeaders = FileHeaders.GetHeaders(Path.GetExtension(file.FileName));

            if (allowedHeaders.Length.Equals(0)) return false;

            using var checkerStream = new MemoryStream();
            await file.CopyToAsync(checkerStream);
            var logoBytes = checkerStream.ToArray();

            return allowedHeaders.SequenceEqual(logoBytes.Take(allowedHeaders.Length).ToArray());
        }

        public string UploadFile(int taskId, IFormFile file)
        {
            string fileName = taskId + Path.GetExtension(file.FileName);
            string filePath = @"wwwroot\TaskAttachment\" + fileName;
            var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return filePath;
        }
    }
}
