using Microsoft.Win32;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using System.Collections.Generic;
using System.Windows;

namespace Mil.Paperwork.WriteOff.Configuration
{
    internal class WpfDialogService : IDialogService
    {
        public DialogResult ShowMessage(string message, string caption = "", DialogButtons buttons = DialogButtons.OK, DialogIcon icon = DialogIcon.None)
        {
            var mbButtons = MessageBoxButton.OK;
            switch (buttons)
            {
                case DialogButtons.OK:
                    mbButtons = MessageBoxButton.OK;
                    break;
                case DialogButtons.OKCancel:
                    mbButtons = MessageBoxButton.OKCancel;
                    break;
                case DialogButtons.YesNo:
                    mbButtons = MessageBoxButton.YesNo;
                    break;
            }

            var mbIcon = MessageBoxImage.None;
            switch (icon)
            {
                case DialogIcon.Information:
                    mbIcon = MessageBoxImage.Information;
                    break;
                case DialogIcon.Warning:
                    mbIcon = MessageBoxImage.Warning;
                    break;
                case DialogIcon.Error:
                    mbIcon = MessageBoxImage.Error;
                    break;
            }

            var result = MessageBox.Show(message, caption ?? string.Empty, mbButtons, mbIcon);

            return result switch
            {
                MessageBoxResult.OK => DialogResult.Ok,
                MessageBoxResult.Cancel => DialogResult.Cancel,
                MessageBoxResult.Yes => DialogResult.Yes,
                MessageBoxResult.No => DialogResult.No,
                _ => DialogResult.None
            };
        }

        public bool TryPickFile(out string filePath, string filter = "", string title = "")
        {
            var dlg = new OpenFileDialog
            {
                Filter = filter ?? string.Empty,
                Title = title ?? string.Empty
            };

            var result = dlg.ShowDialog() ?? false;
            filePath = result ? dlg.FileName : null;

            return result;
        }

        public bool TryPickFolder(out string path)
        {
            var dlg = new OpenFolderDialog();

            var result = dlg.ShowDialog() ?? false;
            path = result ? dlg.FolderName : null;

            return result;
        }
    }
}
