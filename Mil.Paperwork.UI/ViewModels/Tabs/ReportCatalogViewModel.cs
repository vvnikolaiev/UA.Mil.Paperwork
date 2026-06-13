using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.UI.ViewModels.Reports;
using System;
using System.Collections.Generic;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal class ReportCatalogViewModel : ObservableItem, ITabViewModel
    {
        private const string TabHeader = "Створити звіт";

        public event EventHandler<ITabViewModel> TabCloseRequested;
        public event EventHandler<ReportType> ReportCreationRequested;

        public string Header => TabHeader;

        public bool IsDirty => false;

        public List<ReportItemViewModel> DocumentTypes { get; }

        public IDelegateCommand<ReportType> CreateReportCommand { get; }

        public IDelegateCommand CloseTabCommand { get; }

        public ReportCatalogViewModel()
        {
            DocumentTypes = [.. GetAllReportTypes()];

            CreateReportCommand = new DelegateCommand<ReportType>(CreateReportCommandExecute);
            CloseTabCommand = new DelegateCommand(CloseTabCommandExecute);
        }

        private static IList<ReportItemViewModel> GetAllReportTypes()
        {
            var reportTypes = new List<ReportItemViewModel>()
            {
                new(ReportType.WriteOffOrder),
                new(ReportType.ResidualValueReport),
                new(ReportType.WriteOffPackage),
                new(ReportType.AssetValuationReport),
                new(ReportType.AssetDismantlingReport),
                new(ReportType.TechnicalStateReport),
                new(ReportType.CommissioningAct),
                new(ReportType.Invoice),
                new(ReportType.Handover23Act),
            };

            return reportTypes;
        }

        private void CreateReportCommandExecute(ReportType reportType)
        {
            ReportCreationRequested?.Invoke(this, reportType);
        }

        private void CloseTabCommandExecute()
        {
            TabCloseRequested?.Invoke(this, this);
        }
    }
}
