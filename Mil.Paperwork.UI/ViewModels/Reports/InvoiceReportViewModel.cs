using Mil.MVVM.Common;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Helpers;
using Mil.Paperwork.UI.Managers;
using Mil.Paperwork.UI.ViewModels.Assets;
using Mil.Paperwork.UI.ViewModels.Controls;
using Mil.Paperwork.UI.ViewModels.Dictionaries;
using Mil.Paperwork.UI.ViewModels.Ribbon;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Mil.Paperwork.UI.ViewModels.Reports
{
    internal class InvoiceReportViewModel : BaseReportTabViewModel, IReportDataLoadable<IInvoceReportData>
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

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
            set
            {
                if (SetProperty(ref _dateCreated, value))
                {
                    UpdateDueDate();
                }
            }
        }

        public int ValidDays
        {
            get => _validDays;
            set
            {
                if (SetProperty(ref _validDays, value))
                {
                    UpdateDueDate();
                }
            }
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

        public IDelegateCommand AddRowCommand { get; }
        public IDelegateCommand RemoveRowCommand { get; }
        public IDelegateCommand GenerateReportCommand { get; }
        public IDelegateCommand OpenConfigurationCommand { get; }

        protected override ReportType HistoryReportType => ReportType.Invoice;

        protected override string AutomationIdPrefix => "Invoice";

        public InvoiceReportViewModel(
            ReportManager reportManager,
            IDataService dataService,
            IReportHistoryService reportHistoryService,
            IDialogService dialogService) : base(reportHistoryService, dialogService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
            _dialogService = dialogService;

            ProductsSelector = new ProductSelectionViewModel(dataService);
            MeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(x => new MeasurementUnitViewModel(x))];
            People = [.. _dataService.LoadPeopleData().Select(x => new PersonViewModel(x))];
            AssetsCollection = [];

            AssetAcceptance = new AssetAccetpanceViewModel(dataService);

            GenerateReportCommand = new DelegateCommand(GenerateReportCommandExecute);
            OpenConfigurationCommand = new DelegateCommand(OpenConfigurationCommandExecute);

            AddRowCommand = new DelegateCommand(AddRow);
            RemoveRowCommand = new DelegateCommand(RemoveRowExecute);
            ResumeDirtyTracking();
        }

        protected override IList<RibbonGroupViewModel> BuildRibbonGroups()
        {
            var documentGroup = CreateDocumentGroup(GenerateReportCommand);
            var tableGroup = CreateTableGroup(AddRowCommand, RemoveRowCommand, null, null);
            var reportGroup = CreateReportGroup(OpenConfigurationCommand, null);
            var groups = new List<RibbonGroupViewModel> { documentGroup, tableGroup, reportGroup };
            return groups;
        }

        private void UpdateDueDate()
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

        private async void GenerateReportCommandExecute()
        {
            // Validate required fields
            if (GetIsDataValid() == false)
            {
                await _dialogService.ShowMessageAsync("Please fill all required fields.", "Validation", icon: DialogIcon.Warning);
                return;
            }

            if (_dialogService.TryPickFolder(out var folderName))
            {
                await GenerateReport(folderName);
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

        protected override IReportData BuildReportData()
        {
            var reportData = new InvoceReportData
            {
                DocumentNumber = DocumentNumber,
                Assets = [.. AssetsCollection.Select(x => x.ToAssetInfo())],
                Recipient = AssetAcceptance.GetAcceptedDTO(),
                Transmitter = AssetAcceptance.GetHandedDTO(),
                Reason = Reason,
                DateCreated = DateCreated,
                DueDate = DueDate,
            };

            return reportData;
        }

        protected async Task GenerateReport(string folderName)
        {
            var reportData = (InvoceReportData)BuildReportData();
            reportData.DestinationFolder = folderName;

            _dataService.AlterPeople([reportData.Recipient, reportData.Transmitter]);

            var result = await RunReportAsync(TextFormatHelper.InvoiceName, "Помилка генерації накладної",
                () => _reportManager.GenerateInvoice(reportData));
            await RecordGeneratedAsync(result);

            ResetDirtyState();
        }

        public void LoadReportData(IInvoceReportData data)
        {
            WithDirtyTrackingSuspended(() =>
            {
                DocumentNumber = data.DocumentNumber;
                DateCreated = data.DateCreated;
                DueDate = data.DueDate;
                Reason = data.Reason;

                AssetsCollection.Clear();
                foreach (var assetInfo in data.Assets ?? [])
                {
                    AssetsCollection.Add(InvoiceAssetViewModel.FromAssetInfo(assetInfo));
                }

                SelectedAsset = AssetsCollection.FirstOrDefault();

                AssetAcceptance.LoadFrom(data.Recipient, data.Transmitter);
            });
        }

        private void OpenConfigurationCommandExecute()
        {
            OpenSettings(ReportType.Invoice);
        }
    }
}