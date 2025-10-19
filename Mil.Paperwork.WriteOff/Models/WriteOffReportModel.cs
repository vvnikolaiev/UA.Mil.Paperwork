using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Helpers;
using Mil.Paperwork.WriteOff.Managers;
using System.IO;

namespace Mil.Paperwork.WriteOff.Models
{
    public class WriteOffReportModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;

        public WriteOffReportModel(ReportManager reportManager, IDataService dataService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
        }

        public void GenerateReport(ObsoleteWriteOffReportData reportData)
        {
            if (reportData == null || reportData.Assets == null)
            {
                return;
            }

            reportData.DestinationFolder = Path.Combine(reportData.DestinationFolder, $"{reportData.EventDate:yyyyMMdd} {reportData.EventReportNumber}");

            var productInfos = reportData.Assets.Select(DTOConvertionHelper.ConvertToProductDTO).ToList();
            _dataService.AlterProductsData(productInfos);
            
            var valuationData = GetValuationDataCollection(reportData.ValuationData, reportData.Dismantlings);
            reportData.ValuationData = valuationData;
            _dataService.SaveValuationData(valuationData);

            _reportManager.GenerateWriteOffReport(reportData);
        }

        private IList<IAssetValuationData?> GetValuationDataCollection(IList<IAssetValuationData?> assetValuations, IList<AssetDismantlingData> assetDismantlings)
        {

            var valuationData = new List<IAssetValuationData?>();

            if (assetValuations != null)
            {
                valuationData.AddRange(assetValuations);
            }

            if (assetDismantlings != null)
            {
                valuationData.AddRange(assetDismantlings);
            }

            return valuationData;
        }
    }
}
