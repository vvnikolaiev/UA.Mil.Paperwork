using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Helpers;
using Mil.Paperwork.WriteOff.Managers;

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

        public void GenerateReport(WriteOffReportData reportData)
        {
            if (reportData == null || reportData.Assets == null)
            {
                return;
            }

            var productInfos = reportData.Assets.Select(DTOConvertionHelper.ConvertToProductDTO).ToList();
            _dataService.SaveProductsData(productInfos);
            _dataService.SaveValuationData(reportData.ValuationData);

            _reportManager.GenerateWriteOffReport(reportData);
        }

        public IList<ProductDTO> LoadProductData()
        {
            return _dataService.LoadProductsData();
        }
    }
}
