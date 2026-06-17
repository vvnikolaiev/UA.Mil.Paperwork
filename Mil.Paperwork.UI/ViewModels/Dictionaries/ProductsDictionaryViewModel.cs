using Mil.Paperwork.Common.Enums;
using Mil.MVVM.Common;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.ViewModels.Ribbon;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Dictionaries
{
    internal class ProductsDictionaryViewModel : SettingsTabViewModel, ISilentRefreshable
    {
        private const string GroupRecords = "Записи";
        private const string GroupData = "Дані";
        private const string GroupExchange = "Обмін";

        private const string CaptionAdd = "Додати";
        private const string CaptionRemove = "Видалити";
        private const string CaptionSave = "Зберегти";
        private const string CaptionRefresh = "Оновити";
        private const string CaptionImport = "Імпорт";
        private const string CaptionExportJson = "Екс. JSON";
        private const string CaptionExportExcel = "Екс. Excel";

        private const string AutomationIdAdd = "ProductsDictionary_AddAction";
        private const string AutomationIdRemove = "ProductsDictionary_RemoveAction";
        private const string AutomationIdSave = "ProductsDictionary_SaveAction";
        private const string AutomationIdRefresh = "ProductsDictionary_RefreshAction";
        private const string AutomationIdImport = "ProductsDictionary_ImportAction";
        private const string AutomationIdExportJson = "ProductsDictionary_ExportJsonAction";
        private const string AutomationIdExportExcel = "ProductsDictionary_ExportExcelAction";

        private readonly IDataService _dataService;
        private readonly IExportService _exportService;
        private readonly IImportService _importService;
        private readonly IDialogService _dialogService;

        private ProductViewModel _selectedProduct;
        private IDelegateCommand _exportJsonCommand;
        private IDelegateCommand _exportExcelCommand;

        public ObservableCollection<ProductViewModel> Products { get; }
        public ObservableCollection<ExportType> ExportTypes { get; private set; }
        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }

        public override string Header => "Довідник майна";

        public ProductViewModel SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public IDelegateCommand AddItemCommand { get; }
        public IDelegateCommand RemoveItemCommand { get; }
        public IDelegateCommand SaveCommand { get; }
        public IDelegateCommand ImportCommand { get; }
        public IDelegateCommand<ExportType> ExportDataCommand { get; }
        public IDelegateCommand RefreshCommand { get; }

        private IDelegateCommand ExportJsonCommand =>
            _exportJsonCommand ??= new DelegateCommand(() => ExportRawDataCommandExecute(ExportType.Json));

        private IDelegateCommand ExportExcelCommand =>
            _exportExcelCommand ??= new DelegateCommand(() => ExportRawDataCommandExecute(ExportType.Excel));

        public ProductsDictionaryViewModel(
            IDataService dataService,
            IExportService exportService,
            IImportService importService,
            IDialogService dialogService) : base(dialogService)
        {
            _dataService = dataService;
            _exportService = exportService;
            _importService = importService;
            _dialogService = dialogService;

            Products = [.. GetProductsData()];
            ExportTypes = [.. EnumHelper.GetValues<ExportType>()];
            MeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(x => new MeasurementUnitViewModel(x))];

            AddItemCommand = new DelegateCommand(AddItemCommandExecute);
            RemoveItemCommand = new DelegateCommand(RemoveItemCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            ImportCommand = new DelegateCommand(ImportCommandExecute);
            ExportDataCommand = new DelegateCommand<ExportType>(ExportRawDataCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
        }

        protected override IList<RibbonGroupViewModel> BuildRibbonGroups()
        {
            var groups = new List<RibbonGroupViewModel>
            {
                new RibbonGroupViewModel(GroupRecords, new[]
                {
                    new RibbonActionViewModel(CaptionAdd, RibbonIconKeys.Add, AddItemCommand, AutomationIdAdd),
                    new RibbonActionViewModel(CaptionRemove, RibbonIconKeys.Remove, RemoveItemCommand, AutomationIdRemove, isDestructive: true)
                }),
                new RibbonGroupViewModel(GroupData, new[]
                {
                    new RibbonActionViewModel(CaptionSave, RibbonIconKeys.Save, SaveCommand, AutomationIdSave),
                    new RibbonActionViewModel(CaptionRefresh, RibbonIconKeys.Refresh, RefreshCommand, AutomationIdRefresh)
                }),
                new RibbonGroupViewModel(GroupExchange, new[]
                {
                    new RibbonActionViewModel(CaptionImport, RibbonIconKeys.Import, ImportCommand, AutomationIdImport),
                    new RibbonActionViewModel(CaptionExportJson, RibbonIconKeys.Export, ExportJsonCommand, AutomationIdExportJson),
                    new RibbonActionViewModel(CaptionExportExcel, RibbonIconKeys.Export, ExportExcelCommand, AutomationIdExportExcel)
                })
            };
            return groups;
        }

        public void SilentRefresh()
        {
            ReloadProductsData();
        }

        private ProductViewModel[] GetProductsData()
        {
            var products = _dataService.LoadProductsData();
            var productViewModels = products.Select(x => new ProductViewModel(x));
            var result = productViewModels.ToArray();
            return result;
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

        private void RemoveItemCommandExecute()
        {
            if (SelectedProduct != null && Products.Contains(SelectedProduct))
            {
                Products.Remove(SelectedProduct);
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
    }
}