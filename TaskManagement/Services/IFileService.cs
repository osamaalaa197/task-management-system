namespace TaskManagement.Services
{
    public interface IFileService
    {
        bool AllowFile(IFormFile? file);
        Task<bool> AllowFileExtension(IFormFile? file);
        string UploadFile(int taskId,IFormFile file);
    }
}