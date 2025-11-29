using Mil.Paperwork.Common.Enums;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Managers;
using Mil.Paperwork.UI.Memento;
using Mil.Paperwork.UI.ViewModels.Controls;
using Mil.Paperwork.UI.ViewModels.Dictionaries;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Reports
{
    internal class AssetValuationViewModel : BaseReportTabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly IImportService _importService;
        private readonly IDialogService _dialogService;
        private AssetValuationViewModelMemento _memento;

        private bool _isValid;
        private string _name;
        private string _shortName;
        private string _measurementUnit;
        private decimal _price;
        private string _serialNumber;
        private string _nomenclatureCode;
        private ProductDTO _selectedProduct;

        private ObservableCollection<AssetValuationItemViewModel> _components = [];

        private string _description;
        private DateTime _valuationDate = new DateTime(2023, 01, 01);

        private IAssetValuationData _selectedValuationTemplate;
        private ObservableCollection<IAssetValuationData> _valuationDataTemplates = [];

        public override string Header => "Акт оцінки";

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string MeasurementUnit
        {
            get => _measurementUnit;
            set => SetProperty(ref _measurementUnit, value);
        }

        public DateTime ValuationDate
        {
            get => _valuationDate;
            set => SetProperty(ref _valuationDate, value);
        }

        public bool IsValid
        {
            get => _isValid;
            set => SetProperty(ref _isValid, value);
        }

        public virtual string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string ShortName
        {
            get => _shortName;
            set => SetProperty(ref _shortName, value);
        }

        public virtual string SerialNumber
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public string NomenclatureCode
        {
            get => _nomenclatureCode;
            set => SetProperty(ref _nomenclatureCode, value);
        }
        public ProductDTO SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    ProductSelector.SelectedProduct = value;
                    FillProductDetails();
                }
            }
        }

        public ProductSelectionViewModel ProductSelector { get; }

        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }

        public virtual ObservableCollection<AssetValuationItemViewModel> Components
        {
            get => _components;
            set => SetProperty(ref _components, value);
        }

        public int ItemsCount => Components.Count;

        public IAssetValuationData SelectedValuationTemplate
        {
            get => _selectedValuationTemplate;
            set => SetProperty(ref _selectedValuationTemplate, value);
        }

        public ObservableCollection<IAssetValuationData> ValuationDataTemplates
        {
            get => _valuationDataTemplates;
            set => SetProperty(ref _valuationDataTemplates, value);
        }

        public IDelegateCommand ApplyValuationTemplateCommand { get; }
        public IDelegateCommand AddRowCommand { get; }
        public IDelegateCommand ImportRowsCommand { get; }
        public IDelegateCommand ClearCommand { get; }
        public IDelegateCommand OkCommand { get; }
        public IDelegateCommand CancelCommand { get; }
        public IDelegateCommand CloseTabCommand { get; }
        public IDelegateCommand GenerateReportCommand { get; }
        public IDelegateCommand OpenConfigurationCommand { get; }

        public AssetValuationViewModel(
            ReportManager reportManager, 
            IDataService dataService, 
            IImportService importService,
            IDialogService dialogService) : base(dialogService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
            _importService = importService;
            _dialogService = dialogService;

            UpdateValuationTemplatesCollection();

            ProductSelector = new ProductSelectionViewModel(dataService);
            MeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(x => new MeasurementUnitViewModel(x))];

            ApplyValuationTemplateCommand = new DelegateCommand(ApplyValuationTemplateExecute);
            AddRowCommand = new DelegateCommand(AddRowExecute);
            ImportRowsCommand = new DelegateCommand(ImportRowsCommandExecute);
            ClearCommand = new DelegateCommand(ClearTableExecute);
            OkCommand = new DelegateCommand(OKCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);
            CloseTabCommand = new DelegateCommand(CloseTabCommandExecute);
            GenerateReportCommand = new DelegateCommand(GenerateReportCommandExecute);
            OpenConfigurationCommand = new DelegateCommand(OpenConfigurationCommandExecute);

            SaveState();
        }

        internal AssetValuationData ToAssetValuationData()
        {
            CalculatePrices();

            var assetValuationData = new AssetValuationData
            {
                Name = Name,
                Price = Price,
                SerialNumber = SerialNumber,
                ValuationDate = ValuationDate,
                Description = Description,
                AssetComponents = GetAssetComponents()
            };

            return assetValuationData;
        }

        protected virtual void ValidateData()
        {
            var isValid = true;

            isValid &= !string.IsNullOrEmpty(Name);
            isValid &= Components.Any(x => x.IsValid);
            isValid &= Price > 0;

            IsValid = isValid;
        }

        protected virtual void ApplyTemplateData(IAssetValuationData valuationDataTemplate)
        {
            FillAssetComponentsTable(valuationDataTemplate.AssetComponents);
        }

        protected virtual void AddComponent(AssetValuationItemViewModel component)
        {
            Components.Add(component);
        }

        protected virtual void ClearComponents()
        {
            Components.Clear();
        }

        protected List<AssetComponent> GetAssetComponents()
        {
            var components = Components
                .Where(x => x.IsValid)
                .Select(x => x.ToAssetComponent())
                .ToList();

            return components;
        }

        private void FillProductDetails()
        {
            var product = ProductSelector?.SelectedProduct;

            if (product != null)
            {
                Name = product.Name;
                ShortName = product.ShortName;
                NomenclatureCode = product.NomenclatureCode;
                Price = product.Price;
                MeasurementUnit = product.MeasurementUnit;
            }
            else
            {
                OnPropertyChanged(nameof(Name));
            }
        }

        private void FillAssetComponentsTable(IList<AssetComponent> assetComponents)
        {
            var items = assetComponents.Select(x => new AssetValuationItemViewModel(x));
            ClearComponents();
            foreach (var item in items)
            {
                AddComponent(item);
            }
        }

        private void UpdateValuationTemplatesCollection()
        {
            ValuationDataTemplates = [.. _dataService.LoadValuationData()];
        }

        private async void ApplyValuationTemplateExecute()
        {
            if (SelectedValuationTemplate == null || SelectedValuationTemplate.AssetComponentsCount == 0)
            {
                return;
            }

            var dlgResult = await _dialogService.ShowMessageAsync("Are you sure you want to replace all the table items?", "Confirmation", DialogButtons.YesNo);
            if (dlgResult == DialogResult.Yes)
            {
                ApplyTemplateData(SelectedValuationTemplate);
            }
        }

        private void AddRowExecute()
        {
            AddComponent(new AssetValuationItemViewModel());
        }

        private async void ImportRowsCommandExecute()
        {
            var importViewModel = new ImportViewModel(_importService, _dataService, _dialogService);
            importViewModel.SetImportType(ImportType.Valuation);

            await _dialogService.OpenImportWindow(importViewModel);

            if (importViewModel.IsValid)
            {
                var assetComponents = importViewModel.ImportDataResult.Rows.Cast<AssetComponent>().ToList();
                FillAssetComponentsTable(assetComponents);
            }
        }

        private async void ClearTableExecute()
        {
            var dlgResult = await _dialogService.ShowMessageAsync("Are you sure you want to clear the table?", "Confirmation", DialogButtons.YesNo);
            if (dlgResult == DialogResult.Yes)
            {
                ClearComponents();
            }
        }

        private void GenerateReportCommandExecute()
        {
            ValidateData();
            if (IsValid)
            {
                // TODO: folderDialog.InitialDirectory = ;
                if (_dialogService.TryPickFolder(out var folderName))
                {
                    GenerateReport(folderName);
                }
            }
        }

        private void OpenConfigurationCommandExecute()
        {
            OpenSettings(ReportType.AssetValuationReport);
        }

        protected virtual void GenerateReport(string folderName)
        {
            var assetValuationData = ToAssetValuationData();
            var reportData = new AssetValuationReportData
            {
                DestinationFolder = folderName,
                ValuationData = [assetValuationData]
            };

            _dataService.SaveValuationData([assetValuationData]);
            _reportManager.GenerateValuationReport(reportData);
        }

        private void OKCommandExecute()
        {
            CalculatePrices();
            ValidateData();
            SaveState();
            CloseWindow();
        }

        private void CancelCommandExecute()
        {
            // there will be a bug when User closes the window with "X" button.
            RestoreState();
            CloseWindow();
        }

        private void CloseTabCommandExecute()
        {
            Close();
        }

        protected void CalculatePrices()
        {
            if (Components.Count == 0)
            {
                return;
            }

            var sumLeft = Price;
            var zeroPriceItems = new List<AssetValuationItemViewModel>();


            foreach (var item in Components)
            {
                if (item.IsValid)
                {
                    sumLeft -= item.Price * item.Quantity;
                    if (item.Price == 0)
                    {
                        zeroPriceItems.Add(item);
                    }
                }
            }

            if (zeroPriceItems.Count > 0)
            {
                var priceOfLeftItems = sumLeft / zeroPriceItems.Sum(x => x.Quantity);
                zeroPriceItems.ForEach(x => x.Price = priceOfLeftItems);
            }
            else
            {
                var mainComponent = Components.First();
                mainComponent.Price += sumLeft;
            }
        }

        private void CloseWindow()
        {
            //_navigationService.CloseWindow(this);
        }

        #region Memento

        public virtual void SaveState()
        {
            _memento = new AssetValuationViewModelMemento
            {
                IsValid = _isValid,
                Name = _name,
                Price = _price,
                SerialNumber = _serialNumber,
                Description = _description,
                ValuationDate = _valuationDate,
                Components = [.. _components.Select(c => new AssetValuationItemViewModelMemento
                {
                    Name = c.Name,
                    NomenclatureCode = c.NomenclatureCode,
                    MeasurementUnit = c.MeasurementUnit,
                    Quantity = c.Quantity,
                    Price = c.Price,
                    Exclude = c.Exclude
                })]
                //SelectedValuationTemplate = this.SelectedValuationTemplate,
                //ValuationDataTemplates = new ObservableCollection<AssetValuationData>(this.ValuationDataTemplates)
            };
        }

        public virtual void RestoreState()
        {
            if (_memento == null) return;

            IsValid = _memento.IsValid;
            Name = _memento.Name;
            Price = _memento.Price;
            SerialNumber = _memento.SerialNumber;
            Description = _memento.Description;
            ValuationDate = _memento.ValuationDate;
            Components = [.. _memento.Components.Select(m => new AssetValuationItemViewModel
                {
                    Name = m.Name,
                    NomenclatureCode = m.NomenclatureCode,
                    MeasurementUnit = m.MeasurementUnit,
                    Quantity = m.Quantity,
                    Price = m.Price,
                    Exclude = m.Exclude
                })];
            //this.SelectedValuationTemplate = _memento.SelectedValuationTemplate;
            //this.ValuationDataTemplates = new ObservableCollection<AssetValuationData>(_memento.ValuationDataTemplates);
        }

        #endregion
    }
}