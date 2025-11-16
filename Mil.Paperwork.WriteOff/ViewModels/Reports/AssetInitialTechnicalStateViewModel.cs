using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.WriteOff.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Factories;
using Mil.Paperwork.WriteOff.Helpers;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.ViewModels.Dictionaries;
using Mil.Paperwork.WriteOff.ViewModels.Tabs;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels.Reports
{
    internal class AssetInitialTechnicalStateViewModel : BaseReportTabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly IDialogService _dialogService;
        private AssetsTableViewModel _assetsTable;
        private AssetAccetpanceViewModel _assetAcceptance;
        private EventType _eventType;

        private DateTime _operationDate = DateTime.Now;
        private bool _isInvoiceNeeded;
        private bool _isInvoceInvertedExchange;
        private string _invoiceNumber;
        private string _commissioningActNumbers;
        private string _commissioningLocation;
        private bool _isCommissioningActNeeded;

        public override string Header => "Тех. стан (№7)";

        public DateTime OperationDate
        {
            get => _operationDate;
            set => SetProperty(ref _operationDate, value);
        }

        public EventType EventType
        {
            get => _eventType;
            set => SetProperty(ref _eventType, value);
        }

        public bool IsInvoiceNeeded
        {
            get => _isInvoiceNeeded;
            set => SetProperty(ref _isInvoiceNeeded, value);
        }

        public bool IsInvoceInvertedExchange
        {
            get => _isInvoceInvertedExchange;
            set => SetProperty(ref _isInvoceInvertedExchange, value);
        }

        public string InvoiceNumber
        {
            get => _invoiceNumber;
            set => SetProperty(ref _invoiceNumber, value);
        }

        public string CommissioningLocation
        {
            get => _commissioningLocation;
            set => SetProperty(ref _commissioningLocation, value);
        }

        public string CommissioningActNumbers
        {
            get => _commissioningActNumbers;
            set => SetProperty(ref _commissioningActNumbers, value);
        }

        public bool IsCommissioningActNeeded
        {
            get => _isCommissioningActNeeded;
            set => SetProperty(ref _isCommissioningActNeeded, value);
        }

        public AssetsTableViewModel AssetsTable
        {
            get => _assetsTable;
            set => SetProperty(ref _assetsTable, value);
        }

        public AssetAccetpanceViewModel AssetAcceptance
        {
            get => _assetAcceptance;
            set => SetProperty(ref _assetAcceptance, value);
        }

        public ObservableCollection<EventType> EventTypes { get; private set; }

        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }

        public ICommand GenerateReportCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand OpenConfigurationCommand { get; }

        public AssetInitialTechnicalStateViewModel(
            ReportManager reportManager,
            IAssetFactory assetFactory,
            IDataService dataService,
            IReportDataService reportDataService,
            IDialogService dialogService) : base(dialogService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
            _reportDataService = reportDataService;
            _dialogService = dialogService;

            AssetsTable = new AssetsTableViewModel(assetFactory, dataService, dialogService);
            AssetAcceptance = new AssetAccetpanceViewModel(dataService);

            FillReportDefaults();
            FillAssetTypesCollection();
            MeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(x => new MeasurementUnitViewModel(x))];

            GenerateReportCommand = new DelegateCommand(GenerateReport);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
            OpenConfigurationCommand = new DelegateCommand(OpenConfigurationCommandExecute);
        }

        private void GenerateReport()
        {
            if (_dialogService.TryPickFolder(out var folderName))
            {
                var assets = AssetsTable.AssetsCollection.Select(x => x.ToAssetInfo(EventType)).ToArray();

                GenerateReport(assets, folderName);

                var productInfos = assets.Select(DTOConvertionHelper.ConvertToProductDTO).ToList();
                _dataService.AlterProductsData(productInfos);
            }
        }

        protected virtual void GenerateReport(IEnumerable<IAssetInfo> assets, string destinationFolder)
        {

            var personAccepted = AssetAcceptance.GetReceiverDTO();
            var personHanded = AssetAcceptance.GetTransmitterDTO();

            var reportData = new InitialTechnicalStateReportData
            {
                EventType = _eventType,
                Assets = [.. assets],
                DestinationFolder = destinationFolder,
                PersonAccepted = personAccepted,
                PersonHanded = personHanded
            };

            _dataService.AlterPeople([personAccepted, personHanded]);

            _reportManager.GenerateInitialTechnicalStateReport(reportData);

            GenerateInvoiceReport(assets, destinationFolder, personAccepted, personHanded);

            GenerateComissioningActReport(assets, destinationFolder, personAccepted, personHanded);
        }

        private void GenerateComissioningActReport(IEnumerable<IAssetInfo> assets, string destinationFolder, PersonDTO personAccepted, PersonDTO personHanded)
        {
            if (IsCommissioningActNeeded)
            {
                var assetsArray = assets.ToArray();

                List<ICommissioningActReportData> commissioningActDataList = [];

                var docNums = CommissioningActNumbers?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];

                var assetCount = assetsArray.Length;

                for (int i = 0; i < assetsArray.Length; i++)
                {
                    var commActDocumentNum = docNums.Length > i ? docNums[i] : 
                                             docNums.Length > 0 ? docNums[0] :
                                             string.Empty;

                    var asset = assetsArray[i];

                    var serialNumbers = asset.SerialNumber?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
                    var identifiers = serialNumbers.Select(x => new ProductIdentification() { SerialNumber = x });

                    var reportData = new CommissioningActReportData
                    {
                        DocumentNumber = commActDocumentNum,
                        DocumentDate = OperationDate,
                        Asset = asset,
                        DestinationFolder = destinationFolder,
                        AssetIds = [.. identifiers],
                        Count = asset.Count,
                        CountText = ReportHelper.GenerateItemsCountText(asset.Count, null),
                        CommissioningLocation = CommissioningLocation,
                        PersonAccepted = personAccepted,
                        PersonHanded = personHanded
                    };

                    commissioningActDataList.Add(reportData);
                }


                _reportManager.GenerateCommissioningAct(commissioningActDataList);
            }
        }

        private void GenerateInvoiceReport(IEnumerable<IAssetInfo> assets, string destinationFolder, PersonDTO personAccepted, PersonDTO personHanded)
        {
            if (IsInvoiceNeeded)
            {
                var invoiceReportData = new InvoceReportData
                {
                    DocumentNumber = InvoiceNumber,
                    Assets = [.. assets],
                    Recipient = IsInvoceInvertedExchange ? personHanded : personAccepted,
                    Transmitter = IsInvoceInvertedExchange ? personAccepted : personHanded,
                    Reason = string.Empty,
                    DateCreated = OperationDate,
                    DueDate = OperationDate.AddDays(10),

                    DestinationFolder = destinationFolder,
                };

                _reportManager.GenerateInvoice(invoiceReportData);
            }
        }

        private void FillAssetTypesCollection()
        {
            EventTypes = [.. EnumHelper.GetValues<EventType>()];
        }

        private void FillReportDefaults()
        {
            var reportParameters = _reportDataService.GetReportParametersDictionary(ReportType.CommissioningAct);

            CommissioningLocation = reportParameters.GetValueOrDefault(CommissioningActHelper.FIELD_COMMISSIONED_LOCATIONN, string.Empty);
            //_assetState = "прид.";
            //AssetCompliance = "відповідає";
            //CompletionState = "не потрібна";
            //Conclusion = "ввести в експлуатацію";
        }

        private void CloseCommandExecute()
        {
            Close();
        }

        private void OpenConfigurationCommandExecute()
        {
            OpenSettings(ReportType.TechnicalStateReport);
        }
    }
}
