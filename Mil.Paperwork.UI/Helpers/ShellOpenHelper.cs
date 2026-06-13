using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Mil.Paperwork.UI.Helpers
{
    internal static class ShellOpenHelper
    {
        private const string ErrorCaption = "Помилка";
        private const string PathNotFoundMessageFormat = "Файл або теку не знайдено:\n{0}";
        private const string OpenPathErrorMessageFormat = "Не вдалося відкрити:\n{0}";

        public static async Task OpenFilesAsync(IList<string> filePaths, IDialogService dialogService)
        {
            string pathToOpen;
            bool pathExists;
            if (filePaths.Count == 1)
            {
                pathToOpen = filePaths[0];
                pathExists = File.Exists(pathToOpen);
            }
            else
            {
                pathToOpen = Path.GetDirectoryName(filePaths[0]) ?? filePaths[0];
                pathExists = Directory.Exists(pathToOpen);
            }

            if (!pathExists)
            {
                var notFoundMessage = string.Format(PathNotFoundMessageFormat, pathToOpen);
                await dialogService.ShowMessageAsync(notFoundMessage, ErrorCaption, DialogButtons.OK, DialogIcon.Error);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(pathToOpen) { UseShellExecute = true });
            }
            catch (Exception)
            {
                var errorMessage = string.Format(OpenPathErrorMessageFormat, pathToOpen);
                await dialogService.ShowMessageAsync(errorMessage, ErrorCaption, DialogButtons.OK, DialogIcon.Error);
            }
        }

        public static async Task OpenFolderAsync(string firstFilePath, IDialogService dialogService)
        {
            var folderPath = Path.GetDirectoryName(firstFilePath) ?? firstFilePath;

            if (!Directory.Exists(folderPath))
            {
                var notFoundMessage = string.Format(PathNotFoundMessageFormat, folderPath);
                await dialogService.ShowMessageAsync(notFoundMessage, ErrorCaption, DialogButtons.OK, DialogIcon.Error);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(folderPath) { UseShellExecute = true });
            }
            catch (Exception)
            {
                var errorMessage = string.Format(OpenPathErrorMessageFormat, folderPath);
                await dialogService.ShowMessageAsync(errorMessage, ErrorCaption, DialogButtons.OK, DialogIcon.Error);
            }
        }
    }
}
