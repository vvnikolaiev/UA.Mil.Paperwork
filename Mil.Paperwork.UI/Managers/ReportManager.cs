using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Services;
using System.Collections.Generic;
using System.Linq;

namespace Mil.Paperwork.UI.Managers
{
    public class ReportManager
    {
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
            EASReportService easReportService)
        {
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

        public ReportGenerationResult GenerateResidualValueReport(IResidualValueReportData reportData)
        {
            var result = RunReport(_residualValueReportService, reportData);
            return result;
        }

        public ReportGenerationResult GenerateInitialTechnicalStateReport(IInitialTechnicalStateReportData reportData)
        {
            var result = RunReport(_initialTechnicalStateReportService, reportData);
            return result;
        }

        public ReportGenerationResult GenerateTechnicalStateReport(ITechnicalStateReportData reportData)
        {
            var result = RunReport(_technicalStateReportService, reportData);
            return result;
        }

        public ReportGenerationResult GenerateQualityStateReport(ICommonWriteOffReportData reportData)
        {
            var result = RunReport(_qualityStateReportService, reportData);
            return result;
        }

        public ReportGenerationResult GenerateWriteOffActReport(ICommonWriteOffReportData reportData)
        {
            var result = RunReport(_writeOffActReportService, reportData);
            return result;
        }

        public ReportGenerationResult GenerateWriteOffPackage(IWriteOffPackageReportData reportData)
        {
            var result = RunReport(_writeOffReportsPackageService, reportData);
            return result;
        }

        public ReportGenerationResult GenerateValuationReport(IAssetValuationReportData reportData)
        {
            var result = RunReport(_valuationReportService, reportData);
            return result;
        }

        public ReportGenerationResult GenerateDismantlingReport(IDismantlingReportData reportData)
        {
            var assetDismantlingReportResult = RunReport(_dismantlingReportService, reportData);
            var assetValuationReportResult = RunReport(_valuationReportService, reportData);

            var combinedFiles = assetDismantlingReportResult.OutputFiles.Concat(assetValuationReportResult.OutputFiles).ToList();
            var combinedResult = ReportGenerationResult.FromResult(assetDismantlingReportResult.Success, combinedFiles);
            return combinedResult;
        }

        public ReportGenerationResult GenerateCommissioningAct(ICommissioningActReportData reportData)
        {
            var result = RunReport(_commissioningActService, reportData);
            return result;
        }

        public ReportGenerationResult GenerateCommissioningAct(IList<ICommissioningActReportData> reportData)
        {
            var result = _commissioningActService.TryGenerateReport(reportData);
            return result;
        }

        public ReportGenerationResult GenerateInvoice(IInvoceReportData reportData)
        {
            var result = RunReport(_invoiceReportService, reportData);
            return result;
        }

        public ReportGenerationResult GenerateHandover23Act(IHandoverReportData reportData)
        {
            var result = RunReport(_handover23ReportService, reportData);
            return result;
        }

        public ReportGenerationResult GenerateWriteOffOrder(IWriteOffOrderReportData reportData)
        {
            var result = RunReport(_writeOffOrderReportService, reportData);
            return result;
        }

        public ReportGenerationResult GenerateEAS(IEASReportData reportData)
        {
            var result = RunReport(_easReportService, reportData);
            return result;
        }

        private static ReportGenerationResult RunReport<TData>(IReportService<TData> service, TData reportData)
            where TData : IReportData
        {
            var result = service.TryGenerateReport(reportData);
            return result;
        }
    }
}
