using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using System;

namespace Mil.Paperwork.UI.ViewModels.Dashboard
{
    internal class ReportCardViewModel : ObservableItem
    {
        private const string IconKeyDefault = "IconDocument";
        private const string AutomationIdFormat = "Dashboard_ReportCard_{0}";
        private const string FavoriteToggleAutomationIdFormat = "Dashboard_ReportCard_{0}_FavoriteToggle";

        private bool _isFavorite;
        private readonly Action<ReportType, bool> _onFavoriteChanged;

        public ReportType ReportType { get; }

        public string Title { get; }

        public string IconKey { get; }

        public string AutomationId { get; }

        public string FavoriteToggleAutomationId { get; }

        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                if (SetProperty(ref _isFavorite, value))
                {
                    _onFavoriteChanged(ReportType, value);
                }
            }
        }

        public IDelegateCommand CreateCommand { get; }

        public ReportCardViewModel(ReportType reportType, bool isFavorite, Action<ReportType> onCreate, Action<ReportType, bool> onFavoriteChanged)
        {
            ReportType = reportType;
            Title = reportType.GetDescription();
            IconKey = IconKeyDefault;
            AutomationId = string.Format(AutomationIdFormat, reportType);
            FavoriteToggleAutomationId = string.Format(FavoriteToggleAutomationIdFormat, reportType);
            _isFavorite = isFavorite;
            _onFavoriteChanged = onFavoriteChanged;

            CreateCommand = new DelegateCommand(() => onCreate(reportType));
        }
    }
}
