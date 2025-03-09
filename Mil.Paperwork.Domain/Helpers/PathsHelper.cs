using System.IO;
using System.Text.RegularExpressions;
namespace Mil.Paperwork.Domain.Helpers
{
    internal static class PathsHelper
    {
        public const string TEMPLATES_DIRECTORY = "Templates";

        public static string GetTemplatePath(string templateName)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDirectory, TEMPLATES_DIRECTORY, templateName);
        }

        public static string GetDestinationPath(string destinationFolder, DateTime reportDate)
        {
            var folderName = $"{reportDate:yyyy.MM.dd} Рапорт";
            var path = Path.Combine(destinationFolder, folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var invalidCharsPattern = new string(invalidChars);
            var regex = new Regex($"[{Regex.Escape(invalidCharsPattern)}]");

            var sanitizedFileName = regex.Replace(fileName, "_");
            return sanitizedFileName;
        }
    }
}
