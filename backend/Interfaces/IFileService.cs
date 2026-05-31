namespace NzolaNet.API.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file, string folder);
        void Delete(string filePath);
    }
}
