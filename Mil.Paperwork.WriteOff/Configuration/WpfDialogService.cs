using Microsoft.Win32;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
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
                case DialogButtons.YesNoCancel:
                    mbButtons = MessageBoxButton.YesNoCancel;
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
                case DialogIcon.Question:
                    mbIcon = MessageBoxImage.Question;
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

        public Task<DialogResult> ShowMessageAsync(string message, string caption = "", DialogButtons buttons = DialogButtons.OK, DialogIcon icon = DialogIcon.None)
        {
            var tcs = new TaskCompletionSource<DialogResult>();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    var res = ShowMessage(message, caption, buttons, icon);
                    tcs.SetResult(res);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));

            return tcs.Task;
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

        public Task<(bool Success, string? Path)> TryPickFileAsync(string filter = "", string title = "")
        {
            var tcs = new TaskCompletionSource<(bool, string?)>();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    var ok = TryPickFile(out var path, filter, title);
                    tcs.SetResult((ok, path));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));

            return tcs.Task;
        }

        public bool TryPickFolder(out string path)
        {
            var dlg = new OpenFolderDialog();

            var result = dlg.ShowDialog() ?? false;
            path = result ? dlg.FolderName : null;

            return result;
        }

        public Task<(bool Success, string? Path)> TryPickFolderAsync()
        {
            var tcs = new TaskCompletionSource<(bool, string?)>();

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    var ok = TryPickFolder(out var p);
                    tcs.SetResult((ok, p));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));

            return tcs.Task;
        }

        public Task OpenImportWindow(object viewModel = null, bool isDialog = true)
        {
            return Task.CompletedTask;
        }
    }
}
