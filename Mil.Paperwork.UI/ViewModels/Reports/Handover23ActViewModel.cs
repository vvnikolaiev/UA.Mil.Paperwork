using Mil.MVVM.Common;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Managers;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Reports
{
    internal class Handover23ActViewModel : BaseReportTabViewModel, IReportDataLoadable<IHandoverReportData>
    {
        private const string HeaderText = "Додаток №23";

        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IReportDataService _reportDataService;

        private string _documentNumber;
        private DateTimeOffset _documentDate = DateTime.Now;
        private DateTime? _dateStart = DateTime.Now;
        private DateTime? _dateEnd = DateTime.Now;
        private string _supplierName; // ТОВАРИСТВО З ОБМЕЖЕНОЮ ВІДПОВІДАЛЬНІСТЮ "ЛВТ ІНЖЖИНІРИНГ"
        private string _receiverName; // військова частина А4682
        private string _reasonDocumentName;
        private string _reasonDocumentNumber;
        private DateTime _reasonDocumentDate = DateTime.Now;

        private AssetAccetpanceViewModel _assetAcceptance;
        private AssetsTableViewModel _assetsTable;

        public override string Header => !string.IsNullOrEmpty(_documentNumber) ? $"{HeaderText} ({_documentNumber})" : HeaderText;

        [Required(ErrorMessage = "Друже, треба заповнити")]
        public string DocumentNumber
        {
            get => _documentNumber;
            set
            {
                if (SetProperty(ref _documentNumber, value))
                {
                    OnPropertyChanged(nameof(Header));
                }
            }
        }

        public DateTimeOffset DocumentDate
        {
            get => _documentDate;
            set => SetProperty(ref _documentDate, value);
        }

        public DateTime? DateStart
        {
            get => _dateStart;
            set => SetProperty(ref _dateStart, value);
        }

        public DateTime? DateEnd
        {
            get => _dateEnd;
            set => SetProperty(ref _dateEnd, value);
        }

        public string SupplierName
        {
            get => _supplierName;
            set => SetProperty(ref _supplierName, value);
        }

        public string ReceiverName
        {
            get => _receiverName;
            set => SetProperty(ref _receiverName, value);
        }

        [Required(ErrorMessage = "Нє, ну принаймні це треба заповнювати")]
        public string ReasonDocumentName
        {
            get => _reasonDocumentName;
            set => SetProperty(ref _reasonDocumentName, value);
        }

        public string ReasonDocumentNumber
        {
            get => _reasonDocumentNumber;
            set => SetProperty(ref _reasonDocumentNumber, value);
        }

        public DateTime ReasonDocumentDate
        {
            get => _reasonDocumentDate;
            set => SetProperty(ref _reasonDocumentDate, value);
        }

        public AssetAccetpanceViewModel AssetAcceptance
        {
            get => _assetAcceptance;
            set => SetProperty(ref _assetAcceptance, value);
        }

        public AssetsTableViewModel AssetsTable
        {
            get => _assetsTable;
            set => SetProperty(ref _assetsTable, value);
        }

        public IDelegateCommand GenerateReportCommand { get; }
        public IDelegateCommand OpenConfigurationCommand { get; }

        protected override ReportType HistoryReportType => ReportType.Handover23Act;

        public Handover23ActViewModel(
            ReportManager reportManager,
            IDataService dataService,
            IReportHistoryService reportHistoryService,
            IDialogService dialogService)
            : base(reportHistoryService, dialogService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
            _dialogService = dialogService;

            AssetsTable = new AssetsTableViewModel(dataService, dialogService);
            AssetAcceptance = new AssetAccetpanceViewModel(dataService);

            GenerateReportCommand = new DelegateCommand(GenerateReportCommandExecute);
            OpenConfigurationCommand = new DelegateCommand(OpenConfigurationCommandExecute);
        }

        private void FillDefaultValues()
        {
        }

        private async void GenerateReportCommandExecute()
        {
            // Validate required fields
            if (GetIsDataValid() == false)
            {
                await _dialogService.ShowMessageAsync("Заповніть всі обов'язкові поля для генерації", "Помилка валідації", icon: DialogIcon.Warning);
                return;
            }

            if (_dialogService.TryPickFolder(out var folderName))
            {
                GenerateReport(folderName);
            }
        }

        private bool GetIsDataValid()
        {
            return true; // TODO: implement validation logic
            //var isTransmitterValid = AssetAcceptance.GetIsTransmitterValid();
            //var isReceiverValid = AssetAcceptance.GetIsReceiverValid();

            //var isValid =
            //    !string.IsNullOrWhiteSpace(DocumentNumber) &&
            //    AssetsCollection.Count > 0 &&
            //    isTransmitterValid &&
            //    isReceiverValid;

            //return isValid;
        }

        protected override IReportData BuildReportData()
        {
            var assets = AssetsTable.AssetsCollection.Select(x => x.ToAssetInfo());
            var reportData = new HandoverReportData
            {
                Assets = [.. assets],
                DocumentNumber = DocumentNumber,
                DocumentDate = DocumentDate.Date,

                DateStart = DateStart,
                DateEnd = DateEnd,

                Supplier = SupplierName,
                Receiver = ReceiverName,

                PersonResponsible = AssetAcceptance.GetHandedDTO(),
                PersonReceiver = AssetAcceptance.GetAcceptedDTO(),

                ReasonDocumentName = ReasonDocumentName,
                ReasonDocumentNumber = ReasonDocumentNumber,
                ReasonDocumentDate = ReasonDocumentDate,
            };

            return reportData;
        }

        protected void GenerateReport(string folderName)
        {
            var reportData = (HandoverReportData)BuildReportData();
            reportData.DestinationFolder = folderName;

            _dataService.AlterPeople([reportData.PersonResponsible, reportData.PersonReceiver]);

            // Generate report
            _reportManager.GenerateHandover23Act(reportData, EnsureHistoryEntryId());
        }

        public void LoadReportData(IHandoverReportData data)
        {
            DocumentNumber = data.DocumentNumber;
            DocumentDate = data.DocumentDate;
            DateStart = data.DateStart;
            DateEnd = data.DateEnd;
            SupplierName = data.Supplier;
            ReceiverName = data.Receiver;
            ReasonDocumentName = data.ReasonDocumentName;
            ReasonDocumentNumber = data.ReasonDocumentNumber;
            ReasonDocumentDate = data.ReasonDocumentDate;

            AssetsTable.LoadAssets(data.Assets ?? []);
            AssetAcceptance.LoadFrom(data.PersonReceiver, data.PersonResponsible);
        }

        private void OpenConfigurationCommandExecute()
        {
            OpenSettings(ReportType.Handover23Act);
        }
    }
}