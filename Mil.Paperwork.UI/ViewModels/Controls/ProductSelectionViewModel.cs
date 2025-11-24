using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mil.Paperwork.UI.ViewModels.Controls
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

        public void UpdateProductsCollection(IEnumerable<ProductDTO?> itemsToExclude = null)
        {
            _loadedProducts = LoadProductData();

            UpdateMergedProductsCollection(itemsToExclude);
        }

        public void UpdateMergedProductsCollection(IEnumerable<ProductDTO?> itemsToExclude = null)
        {
            var products = new List<ProductDTO>();

            if (itemsToExclude != null)
            {
                products.AddRange(itemsToExclude);
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
