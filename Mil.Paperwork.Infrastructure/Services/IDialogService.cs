using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Infrastructure.Services
{
    public interface IDialogService
    {
        DialogResult ShowMessage(string message, string caption = "", DialogButtons buttons = DialogButtons.OK, DialogIcon icon = DialogIcon.Information);

        /// <summary>
        /// Async variant of ShowMessage to support UI frameworks that require async dialogs.
        /// </summary>
        Task<DialogResult> ShowMessageAsync(string message, string caption = "", DialogButtons buttons = DialogButtons.OK, DialogIcon icon = DialogIcon.Information);

        /// <summary>
        /// Pick a single file. Returns the full path or null if canceled.
        /// </summary>
        bool TryPickFile(out string filePath, string filter = "", string title = "");

        /// <summary>
        /// Async variant of TryPickFile.
        /// </summary>
        Task<(bool Success, string? Path)> TryPickFileAsync(string filter = "", string title = "");

        /// <summary>
        /// Pick a folder. Returns the selected folder path or null if canceled.
        /// </summary>
        bool TryPickFolder(out string path);

        /// <summary>
        /// Async variant of TryPickFolder.
        /// </summary>
        Task<(bool Success, string? Path)> TryPickFolderAsync();

        Task OpenImportWindow(object viewModel = null, bool isDialog = true);

    }
}