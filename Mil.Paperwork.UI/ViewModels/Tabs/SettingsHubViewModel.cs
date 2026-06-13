using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Enums;
using Mil.Paperwork.UI.ViewModels.Ribbon;
using System.Collections.Generic;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal class SettingsHubViewModel : BaseTabViewModel
    {
        private const string SectionTitleReportConfig = "Конфігурація звітів";
        private const string SectionTitleCommissions = "Комісії";

        private readonly ReportConfigViewModel _reportConfig;
        private readonly CommissionsConfigViewModel _commissionsConfig;

        private SettingsSectionItem _selectedSectionItem;

        public override string Header => "Налаштування";

        public IReadOnlyList<SettingsSectionItem> SectionItems { get; }

        public SettingsSectionItem SelectedSectionItem
        {
            get => _selectedSectionItem;
            set
            {
                if (SetProperty(ref _selectedSectionItem, value))
                {
                    OnPropertyChanged(nameof(ActiveSectionContent));
                    OnPropertyChanged(nameof(RibbonGroups));
                }
            }
        }

        public object ActiveSectionContent
        {
            get
            {
                var section = _selectedSectionItem?.Section;
                var content = section switch
                {
                    SettingsSection.ReportConfig => (object)_reportConfig,
                    SettingsSection.Commissions => _commissionsConfig,
                    _ => _reportConfig
                };
                return content;
            }
        }

        public override IList<RibbonGroupViewModel> RibbonGroups
        {
            get
            {
                var section = _selectedSectionItem?.Section;
                if (section == SettingsSection.Commissions)
                {
                    return _commissionsConfig.RibbonGroups;
                }

                return _reportConfig.RibbonGroups;
            }
        }

        public SettingsHubViewModel(
            IReportDataService reportDataService,
            IExportService exportService,
            IImportService importService,
            IDialogService dialogService) : base(dialogService)
        {
            _reportConfig = new ReportConfigViewModel(reportDataService, exportService, importService, dialogService);
            _commissionsConfig = new CommissionsConfigViewModel(reportDataService, exportService, importService, dialogService);

            SectionItems = new List<SettingsSectionItem>
            {
                new SettingsSectionItem(SectionTitleReportConfig, SettingsSection.ReportConfig),
                new SettingsSectionItem(SectionTitleCommissions, SettingsSection.Commissions)
            };

            _selectedSectionItem = SectionItems.FirstOrDefault(x => x.Section == SettingsSection.ReportConfig);
        }

        public void SelectReportType(ReportType reportType)
        {
            SelectedSectionItem = SectionItems.FirstOrDefault(x => x.Section == SettingsSection.ReportConfig);
            _reportConfig.SelectReportType(reportType);
        }
    }
}
