using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Factories;
using Mil.Paperwork.WriteOff.ViewModels.Dictionaries;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels.Reports
{
    internal class AssetsTableViewModel : ObservableItem
    {
        private readonly IAssetFactory _assetFactory;
        private readonly IDataService _dataService;

        private WriteOffAssetViewModel? _selectedAsset;
        
        public ProductSelectionViewModel ProductsSelector { get; }

        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }

        public ObservableCollection<WriteOffAssetViewModel> AssetsCollection { get; set; }

        public WriteOffAssetViewModel? SelectedAsset
        {
            get => _selectedAsset;
            set => SetProperty(ref _selectedAsset, value);
        }

        public ICommand ClearTableCommand { get; }
        public ICommand AddRowCommand { get; }
        public ICommand RemoveRowCommand { get; }

        public AssetsTableViewModel(
            IAssetFactory assetFactory,
            IDataService dataService)
        {
            _assetFactory = assetFactory;
            _dataService = dataService;

            ProductsSelector = new ProductSelectionViewModel(dataService);
            AssetsCollection = new ObservableCollection<WriteOffAssetViewModel>();

            MeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(x => new MeasurementUnitViewModel(x))];

            ClearTableCommand = new DelegateCommand(ClearTable);
            AddRowCommand = new DelegateCommand(AddRow);
            RemoveRowCommand = new DelegateCommand(RemoveRowExecute);
        }
        private void ClearTable()
        {
            if (MessageBox.Show("Are you sure you want to clear the table?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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

        public void Refresh()
        {
            var assetsWithProdId = AssetsCollection.Select(x => new { Asset = x, x.SelectedProductId }).ToList();

            // restore selected values
            var products = ProductsSelector.Products
                .GroupBy(x => x.AlmostUniqueID)
                .ToDictionary(g => g.Key, g => g.Last());

            foreach (var item in assetsWithProdId)
            {
                // If asset.SelectedProductId is not in the new Products, set it to null or a default value
                var selectedValue = item.SelectedProductId;
                if (selectedValue != null)
                {
                    products.TryGetValue(selectedValue, out var product);

                    item.Asset.SelectedProductId = product?.AlmostUniqueID;
                }
            }
        }
    }
}
