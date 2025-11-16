using Mil.Paperwork.Infrastructure.Enums;
using System;

namespace Mil.Paperwork.Infrastructure.Services
{
    public interface IDialogService
    {
        DialogResult ShowMessage(string message, string caption = "", DialogButtons buttons = DialogButtons.OK, DialogIcon icon = DialogIcon.None);
        /// <summary>
        /// Pick a single file. Returns the full path or null if canceled.
        /// </summary>
        bool TryPickFile(out string filePath, string filter = "", string title = "");
        /// <summary>
        /// Pick a folder. Returns the selected folder path or null if canceled.
        /// </summary>
        bool TryPickFolder(out string path);
    }
}