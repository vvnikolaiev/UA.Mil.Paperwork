using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.ViewModels.Controls;
using Mil.Paperwork.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mil.Paperwork.UI.Services
{
    internal class AvaloniaDialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;

        public AvaloniaDialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OpenImportWindow(object viewModel = null, bool isDialog = true)
        {
            var window = _serviceProvider.GetRequiredService<ImportDialogWindow>();

            window.DataContext = viewModel;

            if (isDialog)
            {
                var owner = GetCurrentWindow();
                await window.ShowDialog(owner);
            }
            else
            {
                window.Show();
            }
        }

        public DialogResult ShowMessage(string message, string caption = "", DialogButtons buttons = DialogButtons.OK, DialogIcon icon = DialogIcon.Information)
        {
            // Call async variant but avoid deadlock by ensuring async internals do not capture SynchronizationContext.
            return ShowMessageAsync(message, caption, buttons, icon).GetAwaiter().GetResult();
        }

        public async Task<DialogResult> ShowMessageAsync(string message, string caption = "", DialogButtons buttons = DialogButtons.OK, DialogIcon icon = DialogIcon.Information)
        {
            var vm = new MessageBoxViewModel(message, caption, buttons, icon);
            var dialog = new MessageBoxWindow
            {
                DataContext = vm
            };


            // Determine owner window so the dialog can be shown modally.
            var owner = GetCurrentWindow();

            var result = await dialog.ShowDialog<DialogResult>(owner);
            return result;
        }
        //TODO: REFACTOR!!!!!!
        public bool TryPickFile(out string filePath, string filter = "", string title = "")
        {
            var tuple = TryPickFileAsync(filter, title).GetAwaiter().GetResult();
            filePath = tuple.Path;
            return tuple.Success;
        }

        public async Task<(bool Success, string? Path)> TryPickFileAsync(string filter = "", string title = "")
        {
            var result = false;
            string? filePath = null;

            var owner = GetCurrentWindow();
            if (owner != null)
            {
                var options = new FilePickerOpenOptions
                {
                    Title = title ?? string.Empty,
                    AllowMultiple = false
                };

                // Map classic filter like "JSON Files|*.json;*.other|All Files|*.*" to FilePickerFileType(s)
                var fileTypes = GetFilesFilter(filter);
                if (fileTypes != null && fileTypes.Count > 0)
                {
                    options.FileTypeFilter = fileTypes;
                }

                var pickResult = await owner.StorageProvider.OpenFilePickerAsync(options).ConfigureAwait(false);
                result = TryGetPathResult(pickResult, out filePath);
            }

            return (result, filePath);
        }

        public bool TryPickFolder(out string path)
        {
            var tuple = TryPickFolderAsync().GetAwaiter().GetResult();
            path = tuple.Path ?? string.Empty;
            return tuple.Success;
        }

        public async Task<(bool Success, string? Path)> TryPickFolderAsync()
        {
            var result = false;
            string? folderPath = null;

            var owner = GetCurrentWindow();
            if (owner != null)
            {
                var options = new FolderPickerOpenOptions
                {
                    Title = string.Empty
                };

                var folders = await owner.StorageProvider.OpenFolderPickerAsync(options).ConfigureAwait(false);

                result = TryGetPathResult(folders, out folderPath);
            }

            return (result, folderPath);
        }

        private bool TryGetPathResult(IReadOnlyList<IStorageItem> itemsList, out string? path)
        {
            var result = itemsList != null && itemsList.Count > 0;
            path = null;

            if (result)
            {
                var firstItem = itemsList[0];
                var localPath = firstItem.TryGetLocalPath();

                path = !string.IsNullOrEmpty(localPath) ? localPath : firstItem.Name;
            }

            return result;
        }

        private List<FilePickerFileType>? GetFilesFilter(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return null;

            var parts = filter.Split('|');
            if (parts.Length < 2)
                return null;

            var result = new List<FilePickerFileType>();

            // Parse pairs: name | patterns ; name2 | patterns2 ...
            for (int i = 0; i + 1 < parts.Length; i += 2)
            {
                var name = parts[i].Trim();
                var patternPart = parts[i + 1];

                var patterns = patternPart
                    .Split(';')
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToArray();

                var fileType = new FilePickerFileType(string.IsNullOrEmpty(name) ? "Files" : name)
                {
                    Patterns = patterns.Length > 0 ? patterns : ["*.*"]
                };

                result.Add(fileType);
            }

            return result.Count > 0 ? result : null;
        }

        private Window? GetCurrentWindow()
        {
            var app = Avalonia.Application.Current;
            if (app?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var currentWindow = desktop.Windows.FirstOrDefault(w => w.IsActive) ?? desktop.MainWindow;

                return currentWindow;
            }

            return null;
        }
    }
}
