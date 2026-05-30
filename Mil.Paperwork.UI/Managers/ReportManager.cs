using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Helpers;
using System;
using System.Collections.Generic;

namespace Mil.Paperwork.UI.Managers
{
    public class ReportManager
    {
        private readonly IDialogService _dialogService;
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
            IDialogService dialogService)
        {
            _dialogService = dialogService;

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

        public async void GenerateResidualValueReport(IResidualValueReportData reportData)
        {
            try
            {
                var residualValueReportResult = _residualValueReportService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.ResidualValueReportName, residualValueReportResult);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації звіту: {ex.Message}");
            }
        }

        public async void GenerateInitialTechnicalStateReport(IInitialTechnicalStateReportData reportData)
        {
            try
            {
                var technicalStateReportResult = _initialTechnicalStateReportService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.InitialTechnicalStateReportName, technicalStateReportResult);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації звіту: {ex.Message}");
            }
        }

        public async void GenerateTechnicalStateReport(ITechnicalStateReportData reportData)
        {
            try
            {
                var technicalStateReportResult = _technicalStateReportService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.TechnicalStateReportName, technicalStateReportResult);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації звіту: {ex.Message}");
            }
        }

        public async void GenerateQualityStateReport(ICommonWriteOffReportData reportData)
        {
            try
            {
                var qualityStateReportResult = _qualityStateReportService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.QualityStateReportName, qualityStateReportResult);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації звіту: {ex.Message}");
            }
        }

        public async void GenerateWriteOffActReport(ICommonWriteOffReportData reportData)
        {
            try
            {
                var writeOffActResult = _writeOffActReportService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.WriteOffActReportName, writeOffActResult);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації звіту: {ex.Message}");
            }
        }

        public async void GenerateWriteOffPackage(IWriteOffPackageReportData reportData)
        {
            try
            {
                var result = _writeOffReportsPackageService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.WriteOffPackageName, result);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації пакету: {ex.Message}");
            }
        }


        public async void GenerateValuationReport(IAssetValuationReportData reportData)
        {
            try
            {
                var assetValuationReportResult = _valuationReportService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.ValuationReportName, assetValuationReportResult);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації звіту: {ex.Message}");
            }
        }

        public async void GenerateDismantlingReport(IDismantlingReportData reportData)
        {
            try
            {
                var assetDismantlingReportResult = _dismantlingReportService.TryGenerateReport(reportData);
                var assetValuationReportResult = _valuationReportService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.DismantlingReportName, assetDismantlingReportResult);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації звіту: {ex.Message}");
            }
        }

        public async void GenerateCommissioningAct(ICommissioningActReportData reportData)
        {
            try
            {
                var commissioningActResult = _commissioningActService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.CommisioninaActName, commissioningActResult);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації акту: {ex.Message}");
            }
        }

        public async void GenerateCommissioningAct(IList<ICommissioningActReportData> reportData)
        {
            try
            {
                var commissioningActResult = _commissioningActService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.CommisioninaActName, commissioningActResult);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації акту: {ex.Message}");
            }
        }

        public async void GenerateInvoice(IInvoceReportData reportData)
        {
            try
            {
                var invocieResult = _invoiceReportService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.InvoiceName, invocieResult);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації накладної: {ex.Message}");
            }
        }

        public async void GenerateHandover23Act(IHandoverReportData reportData)
        {
            try
            {
                var handoverResult = _handover23ReportService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.Handover23Name, handoverResult);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації акту: {ex.Message}");
            }
        }

        public async void GenerateWriteOffOrder(IWriteOffOrderReportData reportData)
        {
            try
            {
                var result = _writeOffOrderReportService.TryGenerateReport(reportData);

                var status = TextFormatHelper.GetReportStatusMessage("Наказ про списання", result);
                await _dialogService.ShowMessageAsync(status);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync($"Помилка генерації наказу: {ex.Message}");
            }
        }
    }
}
