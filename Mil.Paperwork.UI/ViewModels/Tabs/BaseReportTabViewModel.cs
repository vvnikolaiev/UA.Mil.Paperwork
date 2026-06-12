using Mil.MVVM.Common;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using System;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal abstract class BaseReportTabViewModel : BaseTabViewModel, IReportTabViewModel
    {
        private const string DraftSavedMessage = "Чернетку збережено.";
        private const string DraftSaveFailedMessageFormat = "Не вдалося зберегти чернетку: {0}";

        private readonly IReportHistoryService _reportHistoryService;
        private readonly IDialogService _dialogService;

        public event EventHandler<ReportType> OpenReportSettingsRequested;

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

        private async void SaveDraftCommandExecute()
        {
            try
            {
                var reportData = BuildReportData();
                HistoryEntryId = _reportHistoryService.SaveDraft(HistoryReportType, reportData, HistoryEntryId);

                await _dialogService.ShowMessageAsync(DraftSavedMessage);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync(string.Format(DraftSaveFailedMessageFormat, ex.Message));
            }
        }
    }
}
