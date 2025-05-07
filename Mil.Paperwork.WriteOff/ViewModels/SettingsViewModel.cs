using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.WriteOff.DataModels;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class SettingsViewModel : ObservableItem, ITabViewModel
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

        public ObservableCollection<AssetTypeDataModel> AssetTypes { get; private set; }

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public ICommand SaveSettingsCommand { get; }
        public ICommand CloseTabCommand { get; }

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
            var assetTypes = EnumHelper.GetValuesWithDescriptions<AssetType>().Select(x => new AssetTypeDataModel(x.Value, x.Description));
            AssetTypes = [.. assetTypes];
        }

        private void CloseTabCommandExecute()
        {
            TabCloseRequested.Invoke(this, this);
            IsClosed = true;
        }

        private void SaveSettingsCommandExecute()
        {
            _reportDataService.SetAssetType(_selectedAssetType);
        }
    }
}
