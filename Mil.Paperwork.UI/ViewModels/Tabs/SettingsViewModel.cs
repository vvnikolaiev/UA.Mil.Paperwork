using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.Services;
using System;
using System.Collections.ObjectModel;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal class SettingsViewModel : ObservableItem, ISettingsTabViewModel
    {
        private readonly IReportDataService _reportDataService;

        private AssetType _selectedAssetType;
        public string Header => "Settings";

        public bool IsClosed { get; private set; }

        public AssetType SelectedAssetType
        {
            get => _selectedAssetType;
            set => SetProperty(ref _selectedAssetType, value);
        }

        public ObservableCollection<AssetType> AssetTypes { get; private set; }

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public IDelegateCommand SaveSettingsCommand { get; }
        public IDelegateCommand CloseTabCommand { get; }

        public SettingsViewModel(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;

            SelectedAssetType = reportDataService.GetAssetType();
            FillAssetTypesCollection();

            CloseTabCommand = new DelegateCommand(CloseTabCommandExecute);
            SaveSettingsCommand = new DelegateCommand(SaveSettingsCommandExecute);
        }

        private void FillAssetTypesCollection()
        {
            AssetTypes = [.. EnumHelper.GetValues<AssetType>()];
        }

        private void CloseTabCommandExecute()
        {
            TabCloseRequested.Invoke(this, this);
            IsClosed = true;
        }

        private void SaveSettingsCommandExecute()
        {
            //_reportDataService.SetAssetType(_selectedAssetType);
        }
    }
}
