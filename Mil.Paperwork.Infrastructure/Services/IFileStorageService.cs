namespace Mil.Paperwork.Infrastructure.Services
{
    public interface IFileStorageService
    {
        void SaveFile(string path, byte[] fileBytes);

        T ReadJsonFile<T>(string fileName, string directory = null);

        void WriteJsonToFile<T>(T obj, string fileName, string directory = null);

        void WriteJsonToFile<T>(T obj, string filePath);
    }
}
