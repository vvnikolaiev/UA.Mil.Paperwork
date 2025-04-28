using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Helpers;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.Memento;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    public class AssetDismantlingViewModel : AssetValuationViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;

        private AssetDismantlingViewModelMemento _memento;

        private string _registrationNumber;
        private string _documentNumber;
        private string _dismantlingReason;
        private string _nomenclatureCode;
        private string _finalReportReasonText;

        private ProductDTO _selectedProduct;
        private ObservableCollection<ProductDTO> _products;


        public override string Header => "Акт розукомплектації";

        public string RegistrationNumber
        {
            get => _registrationNumber;
            set => SetProperty(ref _registrationNumber, value);
        }

        public string DocumentNumber
        {
            get => _documentNumber;
            set => SetProperty(ref _documentNumber, value);
        }

        public string DismantlingReason
        {
            get => _dismantlingReason;
            set => SetProperty(ref _dismantlingReason, value);
        }

        public string NomenclatureCode
        {
            get => _nomenclatureCode;
            set => SetProperty(ref _nomenclatureCode, value);
        }

        public string FinalReportReasonText
        {
            get => _finalReportReasonText;
            set => SetProperty(ref _finalReportReasonText, value);
        }

        public int ItemsToExcludeCount => Components.Count(x => x.Exclude);

        public ProductDTO SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public ObservableCollection<ProductDTO> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public ICommand ProductNameChangedCommand { get; }
        public ICommand ProductSelectedCommand { get; }

        public AssetDismantlingViewModel(ReportManager reportManager, IDataService dataService, INavigationService navigationService)
            : base(reportManager, dataService, navigationService)
        {
            _reportManager = reportManager;
            _dataService = dataService;

            UpdateProductsCollection();

            ProductSelectedCommand = new DelegateCommand(ProductSelectedExecute);
            ProductNameChangedCommand = new DelegateCommand(ProductNameChangedCommandExecute);
            //SaveState();
        }

        #region Memento

        public override void SaveState()
        {
            _memento = new AssetDismantlingViewModelMemento
            {
                IsValid = IsValid,
                Name = Name,
                RegistrationNumber = _registrationNumber,
                DocumentNumber = _documentNumber,
                NomenclatureCode = _nomenclatureCode,
                DismantlingReason = _dismantlingReason,
                Price = Price,
                SerialNumber = SerialNumber,
                Description = Description,
                ValuationDate = ValuationDate,
                Components = [.. Components.Select(c => new AssetValuationItemViewModelMemento
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

        public override void RestoreState()
        {
            if (_memento == null) return;

            IsValid = _memento.IsValid;
            Name = _memento.Name;
            RegistrationNumber = _memento.RegistrationNumber;
            DocumentNumber = _memento.DocumentNumber;
            NomenclatureCode = _memento.NomenclatureCode;
            DismantlingReason = _memento.DismantlingReason;
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

            // subscribe events Components
            foreach (var component in Components)
            {
                component.AssetComponentExcludedChanged -= OnAssetComponentExcludedChanged;
            }

            ProductNameChangedCommandExecute();

            //this.SelectedValuationTemplate = _memento.SelectedValuationTemplate;
            //this.ValuationDataTemplates = new ObservableCollection<AssetValuationData>(_memento.ValuationDataTemplates);
        }

        #endregion


        internal AssetDismantlingData ToAssetDismantlingData()
        {
            CalculatePrices();

            var assetDismantlingData = new AssetDismantlingData
            {
                Name = Name,
                RegistrationNumber = _registrationNumber,
                DocumentNumber = _documentNumber,
                NomenclatureCode = _nomenclatureCode,
                Reason = _finalReportReasonText,
                Price = Price,
                SerialNumber = SerialNumber,
                ValuationDate = ValuationDate,
                Description = Description,
                AssetComponents = GetAssetComponents()
            };

            return assetDismantlingData;
        }

        protected override void ValidateData()
        {
            base.ValidateData();
            OnPropertyChanged(nameof(ItemsToExcludeCount));
        }

        protected override void ApplyTemplateData(IAssetValuationData valuationDataTemplate)
        {
            // ?? do we need this when we have a combobox?

            //if (IsDismantling)
            //{
            //    // take name, price, date, etc.
            //    Name = valuationDataTemplate.Name;
            //    TotalPrice = valuationDataTemplate.TotalPrice;
            //    DataSource = valuationDataTemplate.Description;
            //    SerialNumber = valuationDataTemplate.SerialNumber;
            //    ValuationDate = valuationDataTemplate.ValuationDate;
            //}
            Description = valuationDataTemplate.Description;
            ValuationDate = valuationDataTemplate.ValuationDate;

            base.ApplyTemplateData(valuationDataTemplate);
        }

        protected override void GenerateReport(string folderName)
        {
            var dismantlingData = ToAssetDismantlingData();
            var reportData = new DismantlingReportData
            {
                DestinationFolder = folderName,
                Dismantlings = new List<AssetDismantlingData> { dismantlingData }
            };

            _dataService.SaveValuationData([dismantlingData]);
            _reportManager.GenerateDismantlingReport(reportData);
        }

        protected override void AddComponent(AssetValuationItemViewModel component)
        {
            component.AssetComponentExcludedChanged += OnAssetComponentExcludedChanged;
            base.AddComponent(component);
        }

        protected override void ClearComponents()
        {
            foreach (var component in Components)
            {
                component.AssetComponentExcludedChanged -= OnAssetComponentExcludedChanged;
            }

            base.ClearComponents();
        }

        private void OnAssetComponentExcludedChanged(object sender, EventArgs args)
        {
            ProductNameChangedCommandExecute();
        }

        private void UpdateProductsCollection()
        {
            Products = [.. _dataService.LoadProductsData()];
        }

        private void ProductSelectedExecute()
        {
            FillProductDetails();
        }

        private void ProductNameChangedCommandExecute()
        {
            var excludedComponents = Components.Where(x => x.Exclude).Select(x => x.Name).ToArray();
            if (excludedComponents.Any())
            {
                var fullName = ReportHelper.GetFullAssetName(Name, SerialNumber);
                var extendedText = TextFormatHelper.GetDismantlingDescriptionText(excludedComponents, fullName, _dismantlingReason);
                FinalReportReasonText = extendedText;
            }
        }

        private void FillProductDetails()
        {
            if (SelectedProduct != null)
            {
                Name = SelectedProduct.Name;
                ShortName = SelectedProduct.ShortName;
                NomenclatureCode = SelectedProduct.NomenclatureCode;
                Price = SelectedProduct.Price;
                MeasurementUnit = SelectedProduct.MeasurementUnit;
            }
            else
            {
                OnPropertyChanged(nameof(Name));
            }
        }
    }
}