using Mil.MVVM.Common;
using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Factories;
using Mil.Paperwork.UI.ViewModels.Assets;
using Mil.Paperwork.UI.ViewModels.Controls;
using Mil.Paperwork.UI.ViewModels.Dictionaries;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Reports
{
    internal class AssetsTableViewModel : ObservableItem
    {
        private readonly IAssetFactory _assetFactory;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private WriteOffAssetViewModel? _selectedAsset;
        
        public ProductSelectionViewModel ProductsSelector { get; }

        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }

        public ObservableCollection<WriteOffAssetViewModel> AssetsCollection { get; set; }

        public WriteOffAssetViewModel? SelectedAsset
        {
            get => _selectedAsset;
            set => SetProperty(ref _selectedAsset, value);
        }

        public IDelegateCommand ClearTableCommand { get; }
        public IDelegateCommand AddRowCommand { get; }
        public IDelegateCommand RemoveRowCommand { get; }

        public AssetsTableViewModel(IDataService dataService, IDialogService dialogService) 
            : this(new DummyAssetFactory(), dataService, dialogService)
        { 
        }

        public AssetsTableViewModel(
            IAssetFactory assetFactory,
            IDataService dataService,
            IDialogService dialogService)
        {
            _assetFactory = assetFactory;
            _dataService = dataService;
            _dialogService = dialogService;

            ProductsSelector = new ProductSelectionViewModel(dataService);
            AssetsCollection = [];

            MeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(x => new MeasurementUnitViewModel(x))];

            ClearTableCommand = new DelegateCommand(ClearTable);
            AddRowCommand = new DelegateCommand(AddRow);
            RemoveRowCommand = new DelegateCommand(RemoveRowExecute);
        }
        private async void ClearTable()
        {
            var dlgResult = await _dialogService.ShowMessageAsync("Are you sure you want to clear the table?", "Confirmation", DialogButtons.YesNo);
            if (dlgResult == DialogResult.Yes)
            {
                AssetsCollection.Clear();
            }
        }

        private void AddRow()
        {
            var asset = _assetFactory.CreateAssetViewModel();
            AssetsCollection.Add(asset);

            SelectedAsset = asset;
        }

        private void RemoveRowExecute()
        {
            if (SelectedAsset != null && AssetsCollection != null)
            {
                AssetsCollection.Remove(SelectedAsset);

                SelectedAsset = AssetsCollection.FirstOrDefault();
            }
        }

        public void LoadAssets(IEnumerable<IAssetInfo> assets)
        {
            AssetsCollection.Clear();
            foreach (var assetInfo in assets)
            {
                var vm = _assetFactory.CreateAssetViewModel();
                vm.LoadFrom(assetInfo);
                AssetsCollection.Add(vm);
            }

            SelectedAsset = AssetsCollection.FirstOrDefault();
        }
    }
}
