using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;
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
            var productInfos = reportData.Assets.Select(ConvertToProductDTO).ToList();
            _dataService.SaveData(productInfos);

            _reportManager.GenerateWriteOffReport(reportData);
        }

        public IList<ProductDTO> LoadProductData()
        {
            return _dataService.LoadProductsData();
        }

        private ProductDTO ConvertToProductDTO(AssetInfo asset)
        {
            return new ProductDTO
            {
                Name = asset.Name,
                MeasurementUnit = asset.MeasurementUnit,
                NomenclatureCode = asset.NomenclatureCode,
                Price = (decimal)asset.Price,
                StartDate = asset.StartDate,
                WarrantyPeriodYears = asset.WarrantyPeriodYears
            };
        }
    }
}
