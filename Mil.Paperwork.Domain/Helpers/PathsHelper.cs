using Mil.Paperwork.Domain.DataModels.Assets;
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

        public static string GetDetailedFileName(IAssetInfo asset, string fileNameFormat)
        {
            var number = String.IsNullOrEmpty(asset.TSDocumentNumber) ? asset.TSRegisterNumber : asset.TSDocumentNumber;
            var fullNumber = String.IsNullOrEmpty(asset.SerialNumber) ? number : $"{number},{asset.SerialNumber}";

            var name = String.IsNullOrEmpty(asset.ShortName) ? fullNumber : $"{asset.ShortName} {fullNumber}";

            var fileName = String.Format(fileNameFormat, name);

            return fileName;
        }
    }
}
