﻿using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.WriteOff.Helpers;
using System.Windows;

namespace Mil.Paperwork.WriteOff.Managers
{
    public class ReportManager
    {
        private readonly IReportService<WriteOffReportData> _qualityStateReportService;
        private readonly IReportService<WriteOffReportData> _residualValueReportService;
        private readonly IReportService<ITechnicalStateReportData> _technicalStateReportService;
        private readonly IReportService<IInitialTechnicalStateReportData> _initialTechnicalStateReportService;
        private readonly IReportService<IAssetValuationReportData> _valuationReportService;
        private readonly IReportService<IDismantlingReportData> _dismantlingReportService;
        private readonly IReportService<ICommissioningActReportData> _commissioningActService;

        public ReportManager(
            QualityStateReportService qualityStateReportService,
            TechnicalStateReportService technicalStateReportService,
            ResidualValueReportService residualValueService,
            AssetValuationReportService valuationReportService,
            AssetDismantlingReportService dismantlingReportService,
            CommissioningActService commissioningActService)
        {
            _qualityStateReportService = qualityStateReportService;
            _technicalStateReportService = technicalStateReportService;
            _initialTechnicalStateReportService = technicalStateReportService;
            _residualValueReportService = residualValueService;
            _valuationReportService = valuationReportService;
            _dismantlingReportService = dismantlingReportService;
            _commissioningActService = commissioningActService;
        }

        public void GenerateWriteOffReport(WriteOffReportData reportData)
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
            assetValuationReportResultStatus = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.ValuationReportName, residualValueReportResult);
            dismantlingReportResultStatus = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.DismantlingReportName, residualValueReportResult);
            
            var message = $"{residualValueReportResultStatus}\n{technicalStateReportResultStatus}\n{qualityStateReportResultStatus}\n{assetValuationReportResult}\n{dismantlingReportResultStatus}";

            MessageBox.Show(message);
        }

        public void GenerateInitialTechnicalStateReport(IInitialTechnicalStateReportData reportData)
        {
            var technicalStateReportResult = _initialTechnicalStateReportService.TryGenerateReport(reportData);

            var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.InitialTechnicalStateReportName, technicalStateReportResult);
            MessageBox.Show(status);
        }

        public void GenerateTechnicalStateReport(ITechnicalStateReportData reportData)
        {
            var technicalStateReportResult = _technicalStateReportService.TryGenerateReport(reportData);

            var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.TechnicalStateReportName, technicalStateReportResult);
            MessageBox.Show(status);
        }

        public void GenerateValuationReport(IAssetValuationReportData reportData)
        {
            var assetValuationReportResult = _valuationReportService.TryGenerateReport(reportData);

            var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.ValuationReportName, assetValuationReportResult);
            MessageBox.Show(status);
        }

        public void GenerateDismantlingReport(IDismantlingReportData reportData)
        {
            var assetDismantlingReportResult = _dismantlingReportService.TryGenerateReport(reportData);
            var assetValuationReportResult = _valuationReportService.TryGenerateReport(reportData);
            
            var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.DismantlingReportName, assetDismantlingReportResult);
            MessageBox.Show(status);
        }

        public void GenerateCommissioningAct(ICommissioningActReportData reportData)
        {
            var commissioningActResult = _commissioningActService.TryGenerateReport(reportData);

            var status = TextFormatHelper.GetReportStatusMessage(TextFormatHelper.CommisioninaActName, commissioningActResult);
            MessageBox.Show(status);
        }
    }
}
