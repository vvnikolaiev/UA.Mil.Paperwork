using Microsoft.Win32;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.ViewModels.Dictionaries;
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
        private int _yearManufactured;
        private string _measurementUnitName;
        private MeasurementUnitViewModel _measurementUnit;
        private string _assetState;

        private string _serialNumber;
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

        private PersonViewModel _selectedPersonAccepted;
        private PersonViewModel _selectedPersonHanded;
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

        public int WarrantyPeriodMonths
        {
            get => _warrantyPeriodMonths;
            set => SetProperty(ref _warrantyPeriodMonths, value);
        }

        public int YearManufactured
        {
            get => _yearManufactured;
            set => SetProperty(ref _yearManufactured, value);
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

        public PersonViewModel SelectedPersonAccepted
        {
            get => _selectedPersonAccepted;
            set => SetProperty(ref _selectedPersonAccepted, value);
        }

        public PersonViewModel SelectedPersonHanded
        {
            get => _selectedPersonHanded;
            set => SetProperty(ref _selectedPersonHanded, value);
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

        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }

        public ObservableCollection<PersonViewModel> People { get; }

        public ObservableCollection<ProductIdentification> ProductIdentifiers { get; } = [];

        public ProductSelectionViewModel ProductSelector { get; }

        public ICommand ProductSelectedCommand { get; }
        public ICommand PersonAccSelectedCommand { get; }
        public ICommand PersonHanSelectedCommand { get; }
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
            MeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(x => new MeasurementUnitViewModel(x))];
            People = [.. _dataService.LoadPeopleData().Select(x => new PersonViewModel(x))];

            ProductSelectedCommand = new DelegateCommand(ProductSelectedExecute);
            PersonAccSelectedCommand = new DelegateCommand(PersonAccSelectedExecute);
            PersonHanSelectedCommand = new DelegateCommand(PersonHanSelectedExecute);
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

        private void PersonAccSelectedExecute()
        {
            var person = SelectedPersonAccepted;

            if (person != null)
            {
                PersonAcceptedName = person.FullName;
                PersonAcceptedPosition = person.Position;
                PersonAcceptedRank = person.Rank;
            }
            else
            {
                OnPropertyChanged(nameof(PersonAcceptedName));
            }
        }

        private void PersonHanSelectedExecute()
        {
            var person = SelectedPersonHanded;

            if (person != null)
            {
                PersonHandedName = person.FullName;
                PersonHandedPosition = person.Position;
                PersonHandedRank = person.Rank;
            }
            else
            {
                OnPropertyChanged(nameof(PersonHandedName));
            }
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
                MeasurementUnit = MeasurementUnitName,
                WarrantyPeriodMonths = WarrantyPeriodMonths,
                YearManufactured = YearManufactured,
            };

            var identifiers = ProductIdentifiers.Cast<IProductIdentification>().ToList();

            var personAccepted = new PersonDTO(_personAcceptedName, _personAcceptedPosition, _personAcceptedRank);
            var personHanded = new PersonDTO(_personHandedName, _personHandedPosition, _personHandedRank);

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
            var countText = ReportHelper.ConvertNumberToWords(_count, _measurementUnit.Gender);
            var result = $"{_count} ({countText}) {_measurementUnit.Name}";
            CountText = result;
        }
    }
}