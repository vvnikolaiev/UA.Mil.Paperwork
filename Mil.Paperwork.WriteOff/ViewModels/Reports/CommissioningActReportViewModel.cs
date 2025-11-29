using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.ViewModels.Dictionaries;
using Mil.Paperwork.WriteOff.ViewModels.Tabs;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels.Reports
{
    internal class CommissioningActReportViewModel : BaseReportTabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly IDialogService _dialogService;
        private string _productName;
        private string _shortName;
        private decimal _price;
        private int _warrantyPeriodMonths;
        private int _resourceYears;
        private int _yearManufactured;
        private string _measurementUnitName;
        private MeasurementUnitViewModel _measurementUnit;
        private string _assetState;
        private AssetAccetpanceViewModel _assetAcceptance;

        private int _count = 1;
        private string _countText = string.Empty;
        private string _documentNumber;
        private DateTime _documentDate = DateTime.Now.Date;
        private string _commissioningLocation;
        private string _shortCharacteristic;
        private string _assetCompliance;
        private string _completionState;
        private string _testResults;
        private string _otherInfo;
        private string _conclusion;
        private string _attachedDocumentation;

        private bool _isTechnicalStateActCreationChecked;
        private string _tsDocNumber;
        private string _tsRegNumber;

        public override string Header => "Акт введення в експлуатацію";

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        public string ShortName
        {
            get => _shortName;
            set => SetProperty(ref _shortName, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public string MeasurementUnitName
        {
            get => _measurementUnitName;
            set => SetProperty(ref _measurementUnitName, value);
        }

        public MeasurementUnitViewModel MeasurementUnit
        {
            get => _measurementUnit;
            set => SetProperty(ref _measurementUnit, value);
        }

        public AssetAccetpanceViewModel AssetAcceptance
        {
            get => _assetAcceptance;
            set => SetProperty(ref _assetAcceptance, value);
        }

        public int WarrantyPeriodMonths
        {
            get => _warrantyPeriodMonths;
            set => SetProperty(ref _warrantyPeriodMonths, value);
        }

        public int ResourceYears
        {
            get => _resourceYears;
            set => SetProperty(ref _resourceYears, value);
        }

        public int YearManufactured
        {
            get => _yearManufactured;
            set => SetProperty(ref _yearManufactured, value);
        }

        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        public string CountText
        {
            get => _countText;
            set => SetProperty(ref _countText, value);
        }

        public string DocumentNumber
        {
            get => _documentNumber;
            set => SetProperty(ref _documentNumber, value);
        }

        public DateTime DocumentDate
        {
            get => _documentDate;
            set => SetProperty(ref _documentDate, value);
        }

        public string CommissioningLocation
        {
            get => _commissioningLocation;
            set => SetProperty(ref _commissioningLocation, value);
        }

        public string ShortCharacteristic
        {
            get => _shortCharacteristic;
            set => SetProperty(ref _shortCharacteristic, value);
        }

        public string AssetCompliance
        {
            get => _assetCompliance;
            set => SetProperty(ref _assetCompliance, value);
        }

        public string CompletionState
        {
            get => _completionState;
            set => SetProperty(ref _completionState, value);
        }

        public string TestResults
        {
            get => _testResults;
            set => SetProperty(ref _testResults, value);
        }

        public string OtherInfo
        {
            get => _otherInfo;
            set => SetProperty(ref _otherInfo, value);
        }

        public string Conclusion
        {
            get => _conclusion;
            set => SetProperty(ref _conclusion, value);
        }

        public string AttachedDocumentation
        {
            get => _attachedDocumentation;
            set => SetProperty(ref _attachedDocumentation, value);
        }

        public bool IsTechnicalStateActCreationChecked
        {
            get => _isTechnicalStateActCreationChecked;
            set => SetProperty(ref _isTechnicalStateActCreationChecked, value);
        }

        public string TSDocNumber
        {
            get => _tsDocNumber;
            set => SetProperty(ref _tsDocNumber, value);
        }

        public string TSRegNumber
        {
            get => _tsRegNumber;
            set => SetProperty(ref _tsRegNumber, value);
        }

        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }

        public ObservableCollection<ProductIdentification> ProductIdentifiers { get; } = [];

        public ProductSelectionViewModel ProductSelector { get; }

        public ICommand ProductSelectedCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand CloseTabCommand { get; }
        public ICommand OpenConfigurationCommand { get; }

        public CommissioningActReportViewModel(
            ReportManager reportManager,
            IDataService dataService,
            IReportDataService reportDataService,
            IDialogService dialogService) : base(dialogService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
            _reportDataService = reportDataService;
            _dialogService = dialogService;

            ProductSelector = new ProductSelectionViewModel(dataService);
            MeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(x => new MeasurementUnitViewModel(x))];

            AssetAcceptance = new AssetAccetpanceViewModel(dataService);

            ProductSelectedCommand = new DelegateCommand(ProductSelectedExecute);
            GenerateReportCommand = new DelegateCommand(GenerateReportCommandExecute);
            CloseTabCommand = new DelegateCommand(CloseTabCommandExecute);
            OpenConfigurationCommand = new DelegateCommand(OpenConfigurationCommandExecute);

            ProductIdentifiers.CollectionChanged += OnProductIdentifiersCollectionChanged;

            PropertyChanged += OnActPropertyChanged;

            FillReportDefaults();
        }

        private void OnProductIdentifiersCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (ProductIdentifiers.Count > 1)
            {
                Count = ProductIdentifiers.Count;
            }
        }

        private void OnActPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Count) || e.PropertyName == nameof(MeasurementUnit))
            {
                GenerateCountText();
            }
        }

        private void ProductSelectedExecute()
        {
            FillProductDetails();
        }

        private void FillProductDetails()
        {
            var product = ProductSelector?.SelectedProduct;

            if (product != null)
            {
                ProductName = product.Name;
                ShortName = product.ShortName;
                Price = product.Price;
                MeasurementUnitName = product.MeasurementUnit;
                WarrantyPeriodMonths = product.WarrantyPeriodMonths;
                YearManufactured = product.YearManufactured;
            }
            else
            {
                OnPropertyChanged(nameof(ProductName));
            }
        }

        private void FillReportDefaults()
        {
            _assetState = "прид.";
            AssetCompliance = "відповідає";
            CompletionState = "не потрібна";
            Conclusion = "ввести в експлуатацію";

            var reportParameters = _reportDataService.GetReportParametersDictionary(ReportType.CommissioningAct);

            CommissioningLocation = reportParameters.GetValueOrDefault(CommissioningActHelper.FIELD_COMMISSIONED_LOCATIONN, string.Empty);
        }


        private void GenerateReportCommandExecute()
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(DocumentNumber) || Count <= 0)
            {
                _dialogService.ShowMessage("Please fill all required fields.", "Validation", icon: DialogIcon.Warning);
                return;
            }

            if (_dialogService.TryPickFolder(out var folderName))
            {
                GenerateReport(folderName);
            }
        }

        protected virtual void GenerateReport(string folderName)
        {
            var product = new ProductDTO()
            {
                Name = ProductName,
                Price = Price,
                ShortName = ShortName,
                MeasurementUnit = MeasurementUnitName,
                ResourceYears = ResourceYears,
                WarrantyPeriodMonths = WarrantyPeriodMonths,
                YearManufactured = YearManufactured,
            };

            var identifiers = ProductIdentifiers.Cast<IProductIdentification>().ToList();

            var personAccepted = _assetAcceptance.GetReceiverDTO();
            var personHanded = _assetAcceptance.GetTransmitterDTO();

            var reportData = new CommissioningActReportData
            {
                DocumentNumber = DocumentNumber,
                DocumentDate = DocumentDate,
                Asset = product,
                DestinationFolder = folderName,
                AssetIds = identifiers,
                Count = Count,
                CountText = CountText,
                AssetState = _assetState,
                CommissioningLocation = CommissioningLocation,
                ShortCharacteristic = ShortCharacteristic,
                AssetCompliance = AssetCompliance,
                CompletionState = CompletionState,
                TestResults = TestResults,
                OtherInfo = OtherInfo,
                Conclusion = Conclusion,
                AttachedDocumentation = AttachedDocumentation,
                PersonAccepted = personAccepted,
                PersonHanded = personHanded
            };

            _dataService.AlterPeople([personAccepted, personHanded]);

            _reportManager.GenerateCommissioningAct(reportData);

            if (IsTechnicalStateActCreationChecked)
            {
                var asset = new AssetInfo(product)
                {
                    StartDate = DocumentDate,
                    SerialNumber = string.Join(", ", identifiers?.Select(x => x.SerialNumber) ?? []),
                    Count = Count,
                    InitialCategory = 1,
                    TSRegisterNumber = TSRegNumber,
                    TSDocumentNumber = TSDocNumber
                };

                var tsReportData = new InitialTechnicalStateReportData
                {
                    EventType = EventType.Accounted,
                    Assets = [asset],
                    DestinationFolder = folderName,
                    PersonAccepted = personAccepted,
                    PersonHanded = personHanded,
                    
                };

                _reportManager.GenerateInitialTechnicalStateReport(tsReportData);
            }

        }

        private void CloseTabCommandExecute()
        {
            Close();
        }

        private void OpenConfigurationCommandExecute()
        {
            OpenSettings(ReportType.CommissioningAct);
        }

        private void GenerateCountText()
        {
            var result = ReportHelper.GenerateItemsCountText(_count, _measurementUnit?.ToDTO());
            CountText = result;
        }
    }
}