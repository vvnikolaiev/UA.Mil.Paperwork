using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Enums;
using Mil.Paperwork.UI.ViewModels.Ribbon;
using System.Collections.Generic;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal class SettingsHubViewModel : BaseTabViewModel
    {
        private const string SectionTitleGeneral = "Загальні";
        private const string SectionTitleReportConfig = "Конфігурація звітів";
        private const string SectionTitleCommissions = "Комісії";

        private readonly SettingsViewModel _generalSettings;
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
                var section = _selectedSectionItem?.Section ?? SettingsSection.General;
                var content = section switch
                {
                    SettingsSection.ReportConfig => (object)_reportConfig,
                    SettingsSection.Commissions => _commissionsConfig,
                    _ => _generalSettings
                };
                return content;
            }
        }

        public override IList<RibbonGroupViewModel> RibbonGroups
        {
            get
            {
                var section = _selectedSectionItem?.Section ?? SettingsSection.General;
                if (section == SettingsSection.ReportConfig)
                {
                    return _reportConfig.RibbonGroups;
                }

                if (section == SettingsSection.Commissions)
                {
                    return _commissionsConfig.RibbonGroups;
                }

                return new List<RibbonGroupViewModel>();
            }
        }

        public SettingsHubViewModel(
            IReportDataService reportDataService,
            IExportService exportService,
            IImportService importService,
            IDialogService dialogService) : base(dialogService)
        {
            _generalSettings = new SettingsViewModel(reportDataService);
            _reportConfig = new ReportConfigViewModel(reportDataService, exportService, importService, dialogService);
            _commissionsConfig = new CommissionsConfigViewModel(reportDataService, exportService, importService, dialogService);

            SectionItems = new List<SettingsSectionItem>
            {
                new SettingsSectionItem(SectionTitleGeneral, SettingsSection.General),
                new SettingsSectionItem(SectionTitleReportConfig, SettingsSection.ReportConfig),
                new SettingsSectionItem(SectionTitleCommissions, SettingsSection.Commissions)
            };

            _selectedSectionItem = SectionItems[0];
        }

        public void SelectReportType(ReportType reportType)
        {
            SelectedSectionItem = SectionItems[1];
            _reportConfig.SelectReportType(reportType);
        }
    }
}
