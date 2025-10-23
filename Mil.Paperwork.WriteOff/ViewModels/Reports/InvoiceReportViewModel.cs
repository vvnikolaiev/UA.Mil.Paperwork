using Microsoft.Win32;
using Mil.Paperwork.Domain.DataModels.ReportData;
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
    internal class InvoiceReportViewModel : BaseReportTabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;

        private int _validDays = 10;
        private string _documentNumber;
        private DateTime _dateCreated = DateTime.Now;
        private DateTime _dueDate;
        private string _reason;
        private string _operationType;

        private AssetAccetpanceViewModel _assetAcceptance;

        private InvoiceAssetViewModel _selectedAsset;

        public ProductSelectionViewModel ProductsSelector { get; }

        public override string Header => "Накладна (вимога)";

        public string DocumentNumber
        {
            get => _documentNumber;
            set => SetProperty(ref _documentNumber, value);
        }

        public DateTime DateCreated
        {
            get => _dateCreated;
            set => SetProperty(ref _dateCreated, value);
        }

        public int ValidDays
        {
            get => _validDays;
            set => SetProperty(ref _validDays, value);
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        public string OperationType
        {
            get => _operationType;
            set => SetProperty(ref _operationType, value);
        }

        public AssetAccetpanceViewModel AssetAcceptance
        {
            get => _assetAcceptance;
            set => SetProperty(ref _assetAcceptance, value);
        }

        public InvoiceAssetViewModel SelectedAsset
        {
            get => _selectedAsset;
            set => SetProperty(ref _selectedAsset, value);
        }

        public ObservableCollection<InvoiceAssetViewModel> AssetsCollection { get; set; }

        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }

        public ObservableCollection<PersonViewModel> People { get; }

        public ICommand UpdateDueDateCommand { get; }
        public ICommand AddRowCommand { get; }
        public ICommand RemoveRowCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand CloseTabCommand { get; }
        public ICommand OpenConfigurationCommand { get; }

        public InvoiceReportViewModel(ReportManager reportManager, IDataService dataService)
        {
            _reportManager = reportManager;
            _dataService = dataService;

            ProductsSelector = new ProductSelectionViewModel(dataService);
            MeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(x => new MeasurementUnitViewModel(x))];
            People = [.. _dataService.LoadPeopleData().Select(x => new PersonViewModel(x))];
            AssetsCollection = [];

            AssetAcceptance = new AssetAccetpanceViewModel(dataService);
            
            UpdateDueDateCommand = new DelegateCommand(UpdateDueDateCommandExecute);

            GenerateReportCommand = new DelegateCommand(GenerateReportCommandExecute);
            CloseTabCommand = new DelegateCommand(CloseTabCommandExecute);
            OpenConfigurationCommand = new DelegateCommand(OpenConfigurationCommandExecute);

            AddRowCommand = new DelegateCommand(AddRow);
            RemoveRowCommand = new DelegateCommand(RemoveRowExecute);
        }

        private void UpdateDueDateCommandExecute()
        {
            DueDate = DateCreated.AddDays(ValidDays);
        }

        private void AddRow()
        {
            var asset = new InvoiceAssetViewModel();
            AssetsCollection.Add(asset);

            SelectedAsset = asset;
        }

        private void RemoveRowExecute()
        {
            if (SelectedAsset != null && AssetsCollection != null)
            {
                AssetsCollection.Remove(SelectedAsset);

                SelectedAsset = AssetsCollection.FirstOrDefault();
            }
        }

        private void GenerateReportCommandExecute()
        {
            // Validate required fields
            if (GetIsDataValid() == false)
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

        private bool GetIsDataValid()
        {
            var isTransmitterValid = AssetAcceptance.GetIsTransmitterValid();
            var isReceiverValid = AssetAcceptance.GetIsReceiverValid();

            var isValid =
                !string.IsNullOrWhiteSpace(DocumentNumber) &&
                AssetsCollection.Count > 0 &&
                isTransmitterValid &&
                isReceiverValid;

            return isValid;
        }

        protected void GenerateReport(string folderName)
        {
            var personRecipient = AssetAcceptance.GetReceiverDTO();
            var personTransmitter = AssetAcceptance.GetTransmitterDTO();

            var reportData = new InvoceReportData
            {
                DocumentNumber = DocumentNumber,
                Assets = [.. AssetsCollection.Select(x => x.ToAssetInfo())],
                Recipient = personRecipient,
                Transmitter = personTransmitter,
                Reason = Reason,
                DateCreated = DateCreated,
                DueDate = DueDate,

                DestinationFolder = folderName,
            };

            _dataService.AlterPeople([personRecipient, personTransmitter]);

            // Generate report
            _reportManager.GenerateInvoice(reportData);
        }

        private void CloseTabCommandExecute()
        {
            Close();
        }

        private void OpenConfigurationCommandExecute()
        {
            OpenSettings(ReportType.Invoice);
        }
    }
}