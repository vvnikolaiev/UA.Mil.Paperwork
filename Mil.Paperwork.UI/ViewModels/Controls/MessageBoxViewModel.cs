using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.UI.Helpers;
using System;

namespace Mil.Paperwork.UI.ViewModels.Controls
{
    public class MessageBoxViewModel : ObservableItem, IWindowViewModel<DialogResult>
    {
        public string Title { get; }
        public string Message { get; }

        public string OkText => "ОК";
        public string CancelText => "Відмінити";
        public string YesText => "Так";
        public string NoText => "Ні";

        public bool IsTitleEmpty => String.IsNullOrEmpty(Title);
        public bool IsOkVisible => Buttons == DialogButtons.OK || Buttons == DialogButtons.OKCancel;
        public bool IsCancelVisible => Buttons == DialogButtons.OKCancel || Buttons == DialogButtons.YesNoCancel;
        public bool IsYesVisible => Buttons == DialogButtons.YesNo || Buttons == DialogButtons.YesNoCancel;
        public bool IsNoVisible => Buttons == DialogButtons.YesNo || Buttons == DialogButtons.YesNoCancel;

        public bool HasIcon => Icon != DialogIcon.None;
        public string IconKey => GetIconKey(Icon);

        public DialogButtons Buttons { get; }
        public DialogIcon Icon { get; }

        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand YesCommand { get; }
        public DelegateCommand NoCommand { get; }


        // Event used by window to close with a result
        public event Action<DialogResult>? RequestClose;

        public MessageBoxViewModel(string message, string title = "", DialogButtons buttons = DialogButtons.OK, DialogIcon icon = DialogIcon.None)
        {
            Message = message ?? string.Empty;
            Title = title ?? string.Empty;
            Buttons = buttons;
            Icon = icon;


            OkCommand = new DelegateCommand(() => Close(DialogResult.Ok));
            CancelCommand = new DelegateCommand(() => Close(DialogResult.Cancel));
            YesCommand = new DelegateCommand(() => Close(DialogResult.Yes));
            NoCommand = new DelegateCommand(() => Close(DialogResult.No));
        }

        private void Close(DialogResult r)
        {
            RequestClose?.Invoke(r);
        }

        private string GetIconKey(DialogIcon icon)
        {
            var result = icon switch
            {
                DialogIcon.Information => IconKeys.DialogInfo,
                DialogIcon.Warning => IconKeys.DialogWarning,
                DialogIcon.Error => IconKeys.DialogError,
                DialogIcon.Question => IconKeys.DialogInfo,
                _ => string.Empty
            };
            return result;
        }
    }
}