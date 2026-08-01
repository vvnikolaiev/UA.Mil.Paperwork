using Mil.Paperwork.DataAccess.Conversions;
using Mil.Paperwork.DataAccess.Mappers;
using Mil.Paperwork.DataAccess.Repositories;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Managers;
using Mil.Paperwork.UI.ViewModels.Reports;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System;
using System.Collections.Generic;

namespace Mil.Paperwork.UI.Factories
{
    internal class ReportTabFactory
    {
        private readonly ReportManager _reportManager;
        private readonly IAssetFactory _assetFactory;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly IReportHistoryService _reportHistoryService;
        private readonly IReportHistoryRepository _reportHistoryRepository;
        private readonly ReportConversionRegistry _conversionRegistry;
        private readonly IImportService _importService;
        private readonly IDialogService _dialogService;

        public event EventHandler<ReportType>? OpenReportSettingsRequested;

        public ReportTabFactory(
            ReportManager reportManager,
            IAssetFactory assetFactory,
            IDataService dataService,
            IReportDataService reportDataService,
            IReportHistoryService reportHistoryService,
            IReportHistoryRepository reportHistoryRepository,
            ReportConversionRegistry conversionRegistry,
            IImportService importService,
            IDialogService dialogService)
        {
            _reportManager = reportManager;
            _assetFactory = assetFactory;
            _dataService = dataService;
            _reportDataService = reportDataService;
            _reportHistoryService = reportHistoryService;
            _reportHistoryRepository = reportHistoryRepository;
            _conversionRegistry = conversionRegistry;
            _importService = importService;
            _dialogService = dialogService;
        }

        public IReportTabViewModel? Create(ReportType reportType)
        {
            var createdTab = CreateReportTab(reportType);

            if (createdTab != null)
            {
                createdTab.OpenReportSettingsRequested += OnOpenReportSettingsRequested;
            }

            return createdTab;
        }

        public IReportTabViewModel? CreateFromHistoryEntry(Guid entryId)
        {
            var entry = _reportHistoryRepository.GetEntry(entryId);
            if (entry == null)
            {
                return null;
            }

            var reportData = ReportSnapshotMapper.ToReportData(entry.Snapshot);
            var createdTab = Create(entry.ReportType);
            if (createdTab == null)
            {
                return null;
            }

            LoadReportDataIntoTab(createdTab, reportData);

            if (createdTab is BaseReportTabViewModel baseTab)
            {
                baseTab.HistoryEntryId = entry.Id;
            }

            return createdTab;
        }

        public IReadOnlyList<IReportTabViewModel> CreateFromConversion(Guid entryId, ReportType targetType)
        {
            var createdTabs = new List<IReportTabViewModel>();

            var entry = _reportHistoryRepository.GetEntry(entryId);
            if (entry?.Snapshot == null)
            {
                return createdTabs;
            }

            var sourceData = ReportSnapshotMapper.ToReportData(entry.Snapshot);
            var convertedDataItems = _conversionRegistry.Convert(entry.ReportType, targetType, sourceData);

            foreach (var convertedData in convertedDataItems)
            {
                var createdTab = Create(targetType);
                if (createdTab == null)
                {
                    continue;
                }

                LoadReportDataIntoTab(createdTab, convertedData);
                createdTabs.Add(createdTab);
            }

            return createdTabs;
        }

        private IReportTabViewModel? CreateReportTab(ReportType documentType)
        {
            IReportTabViewModel? createdTab;
            switch (documentType)
            {
                case ReportType.ResidualValueReport:
                    createdTab = new ResidualValueReportViewModel(_reportManager, _assetFactory, _dataService, _reportDataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.AssetValuationReport:
                    createdTab = new AssetValuationViewModel(_reportManager, _dataService, _importService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.AssetDismantlingReport:
                    createdTab = new AssetDismantlingViewModel(_reportManager, _dataService, _importService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.TechnicalStateReport:
                    createdTab = new AssetInitialTechnicalStateViewModel(_reportManager, _assetFactory, _dataService, _reportDataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.WriteOffPackage:
                    createdTab = new AssetTechnicalStateViewModel(_reportManager, _assetFactory, _dataService, _reportDataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.Invoice:
                    createdTab = new InvoiceReportViewModel(_reportManager, _dataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.CommissioningAct:
                    createdTab = new CommissioningActReportViewModel(_reportManager, _dataService, _reportDataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.Handover23Act:
                    createdTab = new Handover23ActViewModel(_reportManager, _dataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.WriteOffOrder:
                    createdTab = new WriteOffOrderViewModel(_reportManager, _dataService, _reportDataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.EAS:
                    createdTab = new EASViewModel(_reportManager, _dataService, _reportDataService, _reportHistoryService, _dialogService, _importService);
                    break;
                default:
                    createdTab = null;
                    break;
            }

            return createdTab;
        }

        private static void LoadReportDataIntoTab(IReportTabViewModel tab, IReportData reportData)
        {
            switch (reportData)
            {
                case IInvoceReportData invoiceData when tab is InvoiceReportViewModel invoiceVm:
                    invoiceVm.LoadReportData(invoiceData);
                    break;
                case ICommissioningActReportData commActData when tab is CommissioningActReportViewModel commActVm:
                    commActVm.LoadReportData(commActData);
                    break;
                case IInitialTechnicalStateReportData initTsData when tab is AssetInitialTechnicalStateViewModel initTsVm:
                    initTsVm.LoadReportData(initTsData);
                    break;
                case IResidualValueReportData rvData when tab is ResidualValueReportViewModel rvVm:
                    rvVm.LoadReportData(rvData);
                    break;
                case IWriteOffPackageReportData wopData when tab is AssetTechnicalStateViewModel wopVm:
                    wopVm.LoadReportData(wopData);
                    break;
                case IDismantlingReportData dismantlingData when tab is AssetDismantlingViewModel dismantlingVm:
                    dismantlingVm.LoadReportData(dismantlingData);
                    break;
                case IAssetValuationReportData valuationData when tab is AssetValuationViewModel valuationVm:
                    valuationVm.LoadReportData(valuationData);
                    break;
                case IHandoverReportData handoverData when tab is Handover23ActViewModel handoverVm:
                    handoverVm.LoadReportData(handoverData);
                    break;
                case IWriteOffOrderReportData writeOffOrderData when tab is WriteOffOrderViewModel writeOffOrderVm:
                    writeOffOrderVm.LoadReportData(writeOffOrderData);
                    break;
                case IEASReportData easData when tab is EASViewModel easVm:
                    easVm.LoadReportData(easData);
                    break;
            }
        }

        private void OnOpenReportSettingsRequested(object? sender, ReportType reportType)
        {
            OpenReportSettingsRequested?.Invoke(this, reportType);
        }
    }
}
