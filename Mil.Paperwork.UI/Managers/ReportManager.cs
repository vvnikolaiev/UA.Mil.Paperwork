using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mil.Paperwork.UI.Managers
{
    public class ReportManager
    {
        private readonly IDialogService _dialogService;
        private readonly IReportHistoryService _reportHistoryService;
        private readonly IReportService<ICommonWriteOffReportData> _qualityStateReportService;
        private readonly IReportService<ICommonWriteOffReportData> _writeOffActReportService;
        private readonly IReportService<IResidualValueReportData> _residualValueReportService;
        private readonly IReportService<ITechnicalStateReportData> _technicalStateReportService;
        private readonly IReportService<IInitialTechnicalStateReportData> _initialTechnicalStateReportService;
        private readonly IReportService<IAssetValuationReportData> _valuationReportService;
        private readonly IReportService<IDismantlingReportData> _dismantlingReportService;
        private readonly CommissioningActService _commissioningActService;
        private readonly IReportService<IInvoceReportData> _invoiceReportService;
        private readonly IReportService<IHandoverReportData> _handover23ReportService;
        private readonly IReportService<IWriteOffPackageReportData> _writeOffReportsPackageService;
        private readonly IReportService<IWriteOffOrderReportData> _writeOffOrderReportService;
        private readonly IReportService<IEASReportData> _easReportService;

        public ReportManager(
            QualityStateReportService qualityStateReportService,
            TechnicalStateReportService technicalStateReportService,
            WriteOffActReportService writeOffActReportService,
            WriteOffReportPackageService writeOffReportsPackageService,
            ResidualValueReportService residualValueService,
            AssetValuationReportService valuationReportService,
            AssetDismantlingReportService dismantlingReportService,
            CommissioningActService commissioningActService,
            InvoiceReportService invoiceReportService,
            Handover23ReportService handover23ReportService,
            WriteOffOrderReportService writeOffOrderReportService,
            EASReportService easReportService,
            IReportHistoryService reportHistoryService,
            IDialogService dialogService)
        {
            _dialogService = dialogService;
            _reportHistoryService = reportHistoryService;

            _qualityStateReportService = qualityStateReportService;
            _technicalStateReportService = technicalStateReportService;
            _writeOffActReportService = writeOffActReportService;
            _writeOffReportsPackageService = writeOffReportsPackageService;
            _initialTechnicalStateReportService = technicalStateReportService;
            _residualValueReportService = residualValueService;
            _valuationReportService = valuationReportService;
            _dismantlingReportService = dismantlingReportService;
            _commissioningActService = commissioningActService;
            _invoiceReportService = invoiceReportService;
            _handover23ReportService = handover23ReportService;
            _writeOffOrderReportService = writeOffOrderReportService;
            _easReportService = easReportService;
        }

        public async void GenerateWriteOffReport(ObsoleteWriteOffReportData reportData)
        {
            try
            {
                var qualityStateReportResult = _qualityStateReportService.TryGenerateReport(reportData);
                var technicalStateReportResult = _technicalStateReportService.TryGenerateReport(reportData);
                var residualValueReportResult = _residualValueReportService.TryGenerateReport(reportData);
                var assetValuationReportResult = _valuationReportService.TryGenerateReport(reportData);
                var dismantlingReportResult = _dismantlingReportService.TryGenerateReport(reportData);

                string qualityStateReportResultStatus, technicalStateReportResultStatus, residualValueReportResultStatus, assetValuationReportResultStatus, dismantlingReportResultStatus;

                qualityStateReportResultStatus = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.QualityStateReportName, qualityStateReportResult);
                technicalStateReportResultStatus = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.TechnicalStateReportName, technicalStateReportResult);
                residualValueReportResultStatus = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.ResidualValueReportName, residualValueReportResult);
                assetValuationReportResultStatus = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.ValuationReportName, assetValuationReportResult);
                dismantlingReportResultStatus = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.DismantlingReportName, dismantlingReportResult);

                var message = $"{residualValueReportResultStatus}\n{technicalStateReportResultStatus}\n{qualityStateReportResultStatus}\n{assetValuationReportResultStatus}\n{dismantlingReportResultStatus}";

                await _dialogService.ShowMessageAsync(message);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації звітів: {ex.Message}");
            }
        }

        public async void GenerateResidualValueReport(IResidualValueReportData reportData, Guid? historyEntryId = null)
        {
            await RunReportAsync(_residualValueReportService, reportData, TextFormatHelper.ResidualValueReportName,
                "Помилка генерації звіту", ReportType.ResidualValueReport, historyEntryId);
        }

        public async void GenerateInitialTechnicalStateReport(IInitialTechnicalStateReportData reportData, Guid? historyEntryId = null)
        {
            await RunReportAsync(_initialTechnicalStateReportService, reportData, TextFormatHelper.InitialTechnicalStateReportName,
                "Помилка генерації звіту", ReportType.TechnicalStateReport, historyEntryId);
        }

        public async void GenerateTechnicalStateReport(ITechnicalStateReportData reportData, Guid? historyEntryId = null)
        {
            await RunReportAsync(_technicalStateReportService, reportData, TextFormatHelper.TechnicalStateReportName,
                "Помилка генерації звіту", ReportType.TechnicalStateReport, historyEntryId);
        }

        public async void GenerateQualityStateReport(ICommonWriteOffReportData reportData)
        {
            await RunReportAsync(_qualityStateReportService, reportData, TextFormatHelper.QualityStateReportName, "Помилка генерації звіту");
        }

        public async void GenerateWriteOffActReport(ICommonWriteOffReportData reportData)
        {
            await RunReportAsync(_writeOffActReportService, reportData, TextFormatHelper.WriteOffActReportName, "Помилка генерації звіту");
        }

        public async void GenerateWriteOffPackage(IWriteOffPackageReportData reportData, Guid? historyEntryId = null)
        {
            await RunReportAsync(_writeOffReportsPackageService, reportData, TextFormatHelper.WriteOffPackageName,
                "Помилка генерації пакету", ReportType.WriteOffPackage, historyEntryId);
        }

        public async void GenerateValuationReport(IAssetValuationReportData reportData, Guid? historyEntryId = null)
        {
            await RunReportAsync(_valuationReportService, reportData, TextFormatHelper.ValuationReportName,
                "Помилка генерації звіту", ReportType.AssetValuationReport, historyEntryId);
        }

        public async void GenerateDismantlingReport(IDismantlingReportData reportData, Guid? historyEntryId = null)
        {
            try
            {
                var assetDismantlingReportResult = _dismantlingReportService.TryGenerateReport(reportData);
                var assetValuationReportResult = _valuationReportService.TryGenerateReport(reportData);

                var combinedFiles = assetDismantlingReportResult.OutputFiles.Concat(assetValuationReportResult.OutputFiles).ToList();
                var combinedResult = ReportGenerationResult.FromResult(assetDismantlingReportResult.Success, combinedFiles);
                TrySaveGeneratedToHistory(ReportType.AssetDismantlingReport, reportData, combinedResult, historyEntryId);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.DismantlingReportName, assetDismantlingReportResult);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації звіту: {ex.Message}");
            }
        }

        public async void GenerateCommissioningAct(ICommissioningActReportData reportData, Guid? historyEntryId = null)
        {
            await RunReportAsync(_commissioningActService, reportData, TextFormatHelper.CommisioninaActName,
                "Помилка генерації акту", ReportType.CommissioningAct, historyEntryId);
        }

        public async void GenerateCommissioningAct(IList<ICommissioningActReportData> reportData)
        {
            await RunReportAsync(_commissioningActService, reportData, TextFormatHelper.CommisioninaActName, "Помилка генерації акту");
        }

        public async void GenerateInvoice(IInvoceReportData reportData, Guid? historyEntryId = null)
        {
            await RunReportAsync(_invoiceReportService, reportData, TextFormatHelper.InvoiceName,
                "Помилка генерації накладної", ReportType.Invoice, historyEntryId);
        }

        public async void GenerateHandover23Act(IHandoverReportData reportData, Guid? historyEntryId = null)
        {
            await RunReportAsync(_handover23ReportService, reportData, TextFormatHelper.Handover23Name,
                "Помилка генерації акту", ReportType.Handover23Act, historyEntryId);
        }

        public async void GenerateWriteOffOrder(IWriteOffOrderReportData reportData, Guid? historyEntryId = null)
        {
            await RunReportAsync(_writeOffOrderReportService, reportData, "Наказ про списання",
                "Помилка генерації наказу", ReportType.WriteOffOrder, historyEntryId);
        }

        public async void GenerateEAS(IEASReportData reportData, Guid? historyEntryId = null)
        {
            await RunReportAsync(_easReportService, reportData, "Єдиний акт списання",
                "Помилка генерації акту", ReportType.EAS, historyEntryId);
        }

        private async Task RunReportAsync<TData>(
            IReportService<TData> service,
            TData reportData,
            string displayName,
            string errorPrefix,
            ReportType reportType,
            Guid? historyEntryId)
            where TData : IReportData
        {
            try
            {
                var result = service.TryGenerateReport(reportData);

                TrySaveGeneratedToHistory(reportType, reportData, result, historyEntryId);

                var status = TextFormatHelper.GetReportStatusMessage(displayName, result);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"{errorPrefix}: {ex.Message}");
            }
        }

        private async Task RunReportAsync<TData>(
            IReportService<TData> service,
            TData reportData,
            string displayName,
            string errorPrefix)
        {
            try
            {
                var result = service.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(displayName, result);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"{errorPrefix}: {ex.Message}");
            }
        }

        private void TrySaveGeneratedToHistory(ReportType reportType, IReportData reportData, ReportGenerationResult result, Guid? historyEntryId)
        {
            if (result?.Success != true)
            {
                return;
            }

            try
            {
                _reportHistoryService.SaveGenerated(reportType, reportData, result.OutputFiles, historyEntryId);
            }
            catch (Exception)
            {
            }
        }
    }
}
