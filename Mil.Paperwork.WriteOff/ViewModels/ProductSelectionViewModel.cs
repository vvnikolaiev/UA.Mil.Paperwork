using Mil.Paperwork.Infrastructure.MVVM;
using System.Collections.ObjectModel;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class ProductSelectionViewModel : ObservableItem
    {
        private readonly IDataService _dataService;

        private IList<ProductDTO> _loadedProducts;
        private ObservableCollection<ProductDTO> _products;

        public ObservableCollection<ProductDTO> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public ProductSelectionViewModel(IDataService dataService)
        {
            _dataService = dataService;

            UpdateProductsCollection();
        }

        public void UpdateProductsCollection(IEnumerable<AssetDismantlingViewModel> assetDismantlings = null)
        {
            _loadedProducts = LoadProductData();

            UpdateMergedProductsCollection(assetDismantlings);
        }

        public void UpdateMergedProductsCollection(IEnumerable<AssetDismantlingViewModel> assetDismantlings = null)
        {
            var products = new List<ProductDTO>();

            var excludedItems = assetDismantlings?
                .Where(x => x != null && x.IsValid)
                .SelectMany(x => x.Components
                            .Where(c => c != null && x.IsValid && c.Exclude)
                            .Select(c => c.ToProductDTO()))
                .Distinct(new ProductComparer());

            if (excludedItems != null)
            {
                products.AddRange(excludedItems);
            }

            products.AddRange(_loadedProducts);

            Products = [.. products];
        }

        private IList<ProductDTO> LoadProductData()
        {
            return _dataService.LoadProductsData();
        }
    }
}
