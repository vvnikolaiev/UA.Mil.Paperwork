using Microsoft.Win32;
using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.ViewModels.Tabs;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels.Reports
{
    internal class CommissioningActReportViewModel : BaseReportTabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private string _productName;
        private string _shortName;
        private decimal _price;
        private int _warrantyPeriodMonths;
        private string _measurementUnit;
        private string _assetState;

        private string _serialNumber;
        private int _count = 1;
        private string _countText = string.Empty;
        private string _documentNumber;
        private string _commissioningLocation;
        private string _shortCharacteristic;
        private string _assetCompliance;
        private string _completionState;
        private string _testResults;
        private string _otherInfo;
        private string _conclusion;
        private string _attachedDocumentation;

        private string _personAcceptedName;
        private string _personAcceptedRank;
        private string _personAcceptedPosition;
        private string _personHandedName;
        private string _personHandedRank;
        private string _personHandedPosition;

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

        public string MeasurementUnit
        {
            get => _measurementUnit;
            set => SetProperty(ref _measurementUnit, value);
        }

        public int WarrantyPeriodMonths
        {
            get => _warrantyPeriodMonths;
            set => SetProperty(ref _warrantyPeriodMonths, value);
        }

        public string SerialNumber
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
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

        public string PersonAcceptedName
        {
            get => _personAcceptedName;
            set => SetProperty(ref _personAcceptedName, value);
        }

        public string PersonAcceptedRank
        {
            get => _personAcceptedRank;
            set => SetProperty(ref _personAcceptedRank, value);
        }

        public string PersonAcceptedPosition
        {
            get => _personAcceptedPosition;
            set => SetProperty(ref _personAcceptedPosition, value);
        }

        public string PersonHandedName
        {
            get => _personHandedName;
            set => SetProperty(ref _personHandedName, value);
        }

        public string PersonHandedRank
        {
            get => _personHandedRank;
            set => SetProperty(ref _personHandedRank, value);
        }

        public string PersonHandedPosition
        {
            get => _personHandedPosition;
            set => SetProperty(ref _personHandedPosition, value);
        }

        public ObservableCollection<ProductIdentification> ProductIdentifiers { get; } = [];
            
        public ProductSelectionViewModel ProductSelector { get; }

        public ICommand ProductSelectedCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand CloseTabCommand { get; }
        public ICommand OpenConfigurationCommand { get; }

        public CommissioningActReportViewModel(
            ReportManager reportManager,
            IDataService dataService,
            IReportDataService reportDataService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
            _reportDataService = reportDataService;

            ProductSelector = new ProductSelectionViewModel(dataService);

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
                MeasurementUnit = product.MeasurementUnit;
                WarrantyPeriodMonths = product.WarrantyPeriodMonths;
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
            
            PersonAcceptedName = reportParameters.GetValueOrDefault(CommissioningActHelper.FIELD_ACCEPTED_PERSON_NAME, string.Empty);
            PersonAcceptedPosition = reportParameters.GetValueOrDefault(CommissioningActHelper.FIELD_ACCEPTED_PERSON_POSITION, string.Empty);
            PersonAcceptedRank = reportParameters.GetValueOrDefault(CommissioningActHelper.FIELD_ACCEPTED_PERSON_RANK, string.Empty);
            PersonHandedName = reportParameters.GetValueOrDefault(CommissioningActHelper.FIELD_HANDED_PERSON_NAME, string.Empty);
            PersonHandedPosition = reportParameters.GetValueOrDefault(CommissioningActHelper.FIELD_HANDED_PERSON_POSITION, string.Empty);
            PersonHandedRank = reportParameters.GetValueOrDefault(CommissioningActHelper.FIELD_HANDED_PERSON_RANK, string.Empty);

            CommissioningLocation = reportParameters.GetValueOrDefault(CommissioningActHelper.FIELD_COMMISSIONED_LOCATIONN, string.Empty);
        }


        private void GenerateReportCommandExecute()
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(DocumentNumber) || Count <= 0)
            {
                MessageBox.Show("Please fill all required fields.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var folderDialog = new OpenFolderDialog();
            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
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
                MeasurementUnit = MeasurementUnit,
                WarrantyPeriodMonths = WarrantyPeriodMonths
            };

            var identifiers = ProductIdentifiers.Cast<IProductIdentification>().ToList();

            var reportData = new CommissioningActReportData
            {
                DocumentNumber = DocumentNumber,
                Asset = product,
                DestinationFolder = folderName,
                AssetIds = identifiers,
                //SerialNumber = SerialNumber,
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
                PersonAccepted = new Person(_personAcceptedName, _personAcceptedPosition, _personAcceptedRank),
                PersonHanded = new Person(_personHandedName, _personAcceptedPosition, _personHandedRank)
            };

            // Save data if needed
            // _dataService.SaveCommissioningActData(reportData);

            // Generate report
            _reportManager.GenerateCommissioningAct(reportData);
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
            // TODO: CREATE Enum/Dictionary of measurement units!!!

            var countText = ReportHelper.ConvertNumberToWords(_count, NounGender.Masculine);
            var result = $"{_count} ({countText}) {_measurementUnit}";
            CountText = result;
        }
    }
}