using Mil.Paperwork.Infrastructure.Helpers;
using System.IO;
namespace Mil.Paperwork.Infrastructure.Services
{
    internal class FileStorageService : IFileStorageService
    {
        public void SaveFile(string path, byte[] fileContent)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var fileInfo = new FileInfo(path);

                if (!fileInfo.Directory?.Exists ?? false)
                {
                    Directory.CreateDirectory(path);
                }

                if (fileInfo.Exists)
                {
                    path = GetUniqueFileName(path);
                }


                File.WriteAllBytes(path, fileContent);
            }
        }

        public T? ReadJsonFile<T>(string fileName, string directory = null)
        {
            directory ??= GetLocalDirectoryName();
            var filePath = Path.Combine(directory, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file '{fileName}' was not found.", fileName);
            }

            var jsonContent = File.ReadAllText(filePath);
            return JsonHelper.ReadJson<T>(jsonContent);
        }

        public void WriteJsonToFile<T>(T obj, string fileName, string directory = null)
        {
            directory ??= GetLocalDirectoryName();
            var filePath = Path.Combine(directory, fileName);

            WriteJsonToFile(obj, filePath);
        }

        public void WriteJsonToFile<T>(T obj, string filePath)
        {
            var jsonContent = JsonHelper.WriteJson(obj);
            File.WriteAllText(filePath, jsonContent);
        }

        private string GetLocalDirectoryName()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return baseDirectory;
        }

        private string GetUniqueFileName(string fullPath)
        {
            var directory = Path.GetDirectoryName(fullPath) ?? string.Empty;
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullPath);
            var extension = Path.GetExtension(fullPath);

            for (int i = 1; i <= 1000; i++)
            {
                var newFileName = $"{fileNameWithoutExtension} ({i}){extension}";
                var newFilePath = Path.Combine(directory, newFileName);

                if (!File.Exists(newFilePath))
                {
                    return newFilePath;
                }
            }

            throw new IOException("No unique file name available after 1000 attempts.");
        }
    }
}
