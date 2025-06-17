using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.DataModels;
using Mil.Paperwork.WriteOff.Enums;
using Mil.Paperwork.WriteOff.ViewModels.Tabs;

namespace Mil.Paperwork.WriteOff.ViewModels.Dictionaries
{
    internal class ProductsDictionaryViewModel : ISettingsTabViewModel
    {
        private readonly IDataService _dataService;
        private readonly IExportService _exportService;

        public ObservableCollection<ProductViewModel> Products { get; }
        public ObservableCollection<EnumItemDataModel<ExportType>> ExportTypes { get; private set; }
        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }

        public string Header => "Довідник майна";
        public bool IsClosed { get; private set; }

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public ICommand AddItemCommand { get; }
        public ICommand<ProductViewModel> RemoveItemCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand<ExportType> ExportDataCommand { get; }
        public ICommand ExportTableDataCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CloseCommand { get; }

        public ProductsDictionaryViewModel(IDataService dataService, IExportService exportService)
        {
            _dataService = dataService;
            _exportService = exportService;

            Products = [.. GetProductsData()];
            FillExportTypesCollection();
            MeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(x => new MeasurementUnitViewModel(x))];

            AddItemCommand = new DelegateCommand(AddItemCommandExecute);
            RemoveItemCommand = new DelegateCommand<ProductViewModel>(RemoveItemCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            ImportCommand = new DelegateCommand(ImportCommandExecute, () => false); // Disabled for now
            ExportDataCommand = new DelegateCommand<ExportType>(ExportRawDataCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
        }

        private ProductViewModel[] GetProductsData()
        {
            var products = _dataService.LoadProductsData();
            var productViewModels = products.Select(x => new ProductViewModel(x));
            return productViewModels.ToArray();
        }

        private void FillExportTypesCollection()
        {
            var types = EnumHelper.GetValuesWithDescriptions<ExportType>().Select(x => new EnumItemDataModel<ExportType>(x.Value, x.Description));
            ExportTypes = [.. types];
        }

        private void ReloadProductsData()
        {
            Products.Clear();
            var products = GetProductsData();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        }

        private void AddItemCommandExecute()
        {
            var newProduct = new ProductViewModel();
            Products.Add(newProduct);
        }

        private void RemoveItemCommandExecute(ProductViewModel product)
        {
            if (product != null && Products.Contains(product))
            {
                Products.Remove(product);
            }
        }

        private void SaveCommandExecute()
        {
            var products = Products.Select(vm => vm.ToProductDTO()).ToArray();
            _dataService.SaveProductsData(products);
        }

        private void ImportCommandExecute()
        {
            // To be implemented
        }

        private void ExportRawDataCommandExecute(ExportType exportType)
        {
            var folderDialog = new OpenFolderDialog();

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                var products = Products.Select(p => p.ToProductDTO());

                var result = exportType switch
                {
                    ExportType.Json => _exportService.TryExportToJson(products, folderName, ExportHelper.PRODUCTS_FILE_NAME_JSON_FORMAT),
                    ExportType.Excel => _exportService.TryExportToExcel(products, folderName, ExportHelper.PRODUCTS_FILE_NAME_XLSX_FORMAT),
                    _ => throw new ArgumentOutOfRangeException(nameof(exportType), exportType, null)
                };

                string message;
                if (result)
                {
                    message = $"Дані експортовано успішно. Каталог:\r\n{folderName}";
                }
                else
                {
                    message = $"Помилка експорту данних.";
                }

                MessageBox.Show(message);
            }
        }

        private void RefreshCommandExecute()
        {
            var result = MessageBox.Show("Ви впевнені що бажаєте перезавантажити список?", "Підтвердження", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ReloadProductsData();
            }
        }

        private void CloseCommandExecute()
        {
            var result = MessageBox.Show("Are you sure you want to close this tab?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                TabCloseRequested.Invoke(this, this);
                IsClosed = true;
            }
        }
    }
}