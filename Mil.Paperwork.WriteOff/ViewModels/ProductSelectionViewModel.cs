using Mil.Paperwork.Common.MVVM;
using System.Collections.ObjectModel;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.ViewModels.Reports;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class ProductSelectionViewModel : ObservableItem
    {
        private readonly IDataService _dataService;

        private IList<ProductDTO> _loadedProducts;
        private ProductDTO _selectedProduct;
        private readonly ObservableCollection<ProductDTO> _products;

        public ProductDTO SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public ObservableCollection<ProductDTO> Products
        {
            get => _products;
        }

        public ProductSelectionViewModel(IDataService dataService)
        {
            _dataService = dataService;

            _products = new ObservableCollection<ProductDTO>();

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

            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }

        private IList<ProductDTO> LoadProductData()
        {
            return _dataService.LoadProductsData();
        }
    }
}
