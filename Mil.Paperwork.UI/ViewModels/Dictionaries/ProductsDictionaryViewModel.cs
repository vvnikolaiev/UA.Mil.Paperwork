using Mil.Paperwork.Common.Enums;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Dictionaries
{
    internal class ProductsDictionaryViewModel : ISettingsTabViewModel
    {
        private readonly IDataService _dataService;
        private readonly IExportService _exportService;
        private readonly IImportService _importService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<ProductViewModel> Products { get; }
        public ObservableCollection<ExportType> ExportTypes { get; private set; }
        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }

        public string Header => "Довідник майна";
        public bool IsClosed { get; private set; }

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public IDelegateCommand AddItemCommand { get; }
        public IDelegateCommand<ProductViewModel> RemoveItemCommand { get; }
        public IDelegateCommand SaveCommand { get; }
        public IDelegateCommand ImportCommand { get; }
        public IDelegateCommand<ExportType> ExportDataCommand { get; }
        public IDelegateCommand ExportTableDataCommand { get; }
        public IDelegateCommand RefreshCommand { get; }
        public IDelegateCommand CloseCommand { get; }

        public ProductsDictionaryViewModel(
            IDataService dataService, 
            IExportService exportService, 
            IImportService importService,
            IDialogService dialogService)
        {
            _dataService = dataService;
            _exportService = exportService;
            _importService = importService;
            _dialogService = dialogService;

            Products = [.. GetProductsData()];
            ExportTypes = [.. EnumHelper.GetValues<ExportType>()];
            MeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(x => new MeasurementUnitViewModel(x))];

            AddItemCommand = new DelegateCommand(AddItemCommandExecute);
            RemoveItemCommand = new DelegateCommand<ProductViewModel>(RemoveItemCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            ImportCommand = new DelegateCommand(ImportCommandExecute);
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

        private async void ImportCommandExecute()
        {
            var importViewModel = new ImportViewModel(_importService, _dataService, _dialogService);
            importViewModel.SetImportType(ImportType.Products);

            await _dialogService.OpenImportWindow(importViewModel);

            if (importViewModel.IsValid)
            {
                ReloadProductsData();
            }
        }

        private async void ExportRawDataCommandExecute(ExportType exportType)
        {
            if (_dialogService.TryPickFolder(out var folderName))
            {
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


                await _dialogService.ShowMessageAsync(message);
            }
        }

        private async void RefreshCommandExecute()
        {
            var result = await _dialogService.ShowMessageAsync("Ви впевнені що бажаєте перезавантажити список?", "Підтвердження", DialogButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                ReloadProductsData();
            }
        }

        private async void CloseCommandExecute()
        {
            var result = await _dialogService.ShowMessageAsync("Are you sure you want to close this tab?", "Confirmation", DialogButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                TabCloseRequested.Invoke(this, this);
                IsClosed = true;
            }
        }
    }
}