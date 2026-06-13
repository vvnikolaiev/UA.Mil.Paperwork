using Mil.MVVM.Common;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.ViewModels.Ribbon;
using System;
using System.Collections.Generic;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal abstract class BaseReportTabViewModel : BaseTabViewModel, IReportTabViewModel
    {
        private const string DraftSavedMessage = "Чернетку збережено.";
        private const string DraftSaveFailedMessageFormat = "Не вдалося зберегти чернетку: {0}";
        private const string DraftStatusTextFormat = "Чернетку збережено о {0}";
        private const string DraftStatusTimeFormat = "HH:mm";

        private const string GroupTitleDocument = "Документ";
        private const string GroupTitleTable = "Таблиця";
        private const string GroupTitleReport = "Звіт";

        private const string CaptionGenerate = "Сформувати";
        private const string CaptionSaveDraft = "Чернетка";
        private const string CaptionAddRow = "Додати рядок";
        private const string CaptionRemoveRow = "Видалити рядок";
        private const string CaptionImport = "Імпорт";
        private const string CaptionClear = "Очистити";
        private const string CaptionParameters = "Параметри";
        private const string CaptionFolder = "Тека";

        private const string IconKeyGenerate = "IconGenerateDocument";
        private const string IconKeySaveDraft = "IconSaveDraft";
        private const string IconKeyAddRow = "IconAddTableRow";
        private const string IconKeyRemoveRow = "IconRemoveTableRow";
        private const string IconKeyImport = "IconImport";
        private const string IconKeyClear = "IconClearTable";
        private const string IconKeyParameters = "IconParameters";
        private const string IconKeyFolder = "IconFolderOpen";

        private readonly IReportHistoryService _reportHistoryService;
        private readonly IDialogService _dialogService;

        private bool _isDirty;
        private bool _suspendDirtyTracking = true;
        private DateTime? _lastDraftSavedAt;

        public event EventHandler<ReportType> OpenReportSettingsRequested;

        public override bool IsDirty => _isDirty;

        public DateTime? LastDraftSavedAt => _lastDraftSavedAt;

        public string DraftStatusText
        {
            get
            {
                if (!_lastDraftSavedAt.HasValue)
                {
                    return string.Empty;
                }
                var result = string.Format(DraftStatusTextFormat, _lastDraftSavedAt.Value.ToString(DraftStatusTimeFormat));
                return result;
            }
        }

        public Guid? HistoryEntryId { get; internal set; }

        public IDelegateCommand SaveDraftCommand { get; }

        protected abstract ReportType HistoryReportType { get; }

        public BaseReportTabViewModel(IReportHistoryService reportHistoryService, IDialogService dialogService) : base(dialogService)
        {
            _reportHistoryService = reportHistoryService;
            _dialogService = dialogService;

            SaveDraftCommand = new DelegateCommand(SaveDraftCommandExecute);
        }

        protected abstract IReportData BuildReportData();

        protected Guid EnsureHistoryEntryId()
        {
            HistoryEntryId ??= Guid.NewGuid();
            return HistoryEntryId.Value;
        }

        protected void OpenSettings(ReportType reportType)
        {
            OpenReportSettingsRequested?.Invoke(this, reportType);
        }

        protected void ResumeDirtyTracking()
        {
            _suspendDirtyTracking = false;
            ClearDirty();
        }

        protected void WithDirtyTrackingSuspended(Action action)
        {
            var wasSuspended = _suspendDirtyTracking;
            _suspendDirtyTracking = true;
            try
            {
                action();
            }
            finally
            {
                _suspendDirtyTracking = wasSuspended;
            }
        }

        protected void ResetDirtyState()
        {
            ClearDirty();
        }

        protected override void NotifyValueChanged(object? value, string propertyName)
        {
            base.NotifyValueChanged(value, propertyName);

            if (!_suspendDirtyTracking && ShouldTrackDirtyForProperty(propertyName))
            {
                MarkDirty();
            }
        }

        protected RibbonGroupViewModel CreateDocumentGroup(IDelegateCommand generateCommand)
        {
            var actions = new List<RibbonActionViewModel>
            {
                new RibbonActionViewModel(CaptionGenerate, IconKeyGenerate, generateCommand),
                new RibbonActionViewModel(CaptionSaveDraft, IconKeySaveDraft, SaveDraftCommand)
            };
            var group = new RibbonGroupViewModel(GroupTitleDocument, actions);
            return group;
        }

        protected RibbonGroupViewModel CreateReportGroup(IDelegateCommand? configCommand, IDelegateCommand? folderCommand)
        {
            var actions = new List<RibbonActionViewModel>();
            if (configCommand != null)
            {
                actions.Add(new RibbonActionViewModel(CaptionParameters, IconKeyParameters, configCommand));
            }
            if (folderCommand != null)
            {
                actions.Add(new RibbonActionViewModel(CaptionFolder, IconKeyFolder, folderCommand));
            }
            var group = new RibbonGroupViewModel(GroupTitleReport, actions);
            return group;
        }

        protected RibbonGroupViewModel CreateTableGroup(
            IDelegateCommand? addRowCommand,
            IDelegateCommand? removeRowCommand,
            IDelegateCommand? importCommand,
            IDelegateCommand? clearCommand)
        {
            var actions = new List<RibbonActionViewModel>();
            if (addRowCommand != null)
            {
                actions.Add(new RibbonActionViewModel(CaptionAddRow, IconKeyAddRow, addRowCommand));
            }
            if (removeRowCommand != null)
            {
                actions.Add(new RibbonActionViewModel(CaptionRemoveRow, IconKeyRemoveRow, removeRowCommand, isDestructive: true));
            }
            if (importCommand != null)
            {
                actions.Add(new RibbonActionViewModel(CaptionImport, IconKeyImport, importCommand));
            }
            if (clearCommand != null)
            {
                actions.Add(new RibbonActionViewModel(CaptionClear, IconKeyClear, clearCommand, isDestructive: true));
            }
            var group = new RibbonGroupViewModel(GroupTitleTable, actions);
            return group;
        }

        private static bool ShouldTrackDirtyForProperty(string propertyName)
        {
            var result = propertyName != nameof(IsDirty)
                && propertyName != nameof(LastDraftSavedAt)
                && propertyName != nameof(DraftStatusText);
            return result;
        }

        private void MarkDirty()
        {
            if (!_isDirty)
            {
                _isDirty = true;
                OnPropertyChanged(nameof(IsDirty));
            }
        }

        private void ClearDirty()
        {
            _isDirty = false;
            _lastDraftSavedAt = null;
            OnPropertyChanged(nameof(IsDirty));
            OnPropertyChanged(nameof(LastDraftSavedAt));
            OnPropertyChanged(nameof(DraftStatusText));
        }

        private async void SaveDraftCommandExecute()
        {
            try
            {
                var reportData = BuildReportData();
                HistoryEntryId = _reportHistoryService.SaveDraft(HistoryReportType, reportData, HistoryEntryId);

                _isDirty = false;
                _lastDraftSavedAt = DateTime.Now;
                OnPropertyChanged(nameof(IsDirty));
                OnPropertyChanged(nameof(LastDraftSavedAt));
                OnPropertyChanged(nameof(DraftStatusText));

                await _dialogService.ShowMessageAsync(DraftSavedMessage);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync(string.Format(DraftSaveFailedMessageFormat, ex.Message));
            }
        }
    }
}
