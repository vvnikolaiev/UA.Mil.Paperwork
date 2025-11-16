using Mil.Paperwork.Infrastructure.Helpers;

namespace Mil.Paperwork.Infrastructure.DataModels.Configuration
{
    public struct MilitaryServiceDTO
    {
        // dunno if needed, but could be useful
        public List<string> AssetTypes { get; set; }

        public ReportParameter ServiceName { get; set; } // зв'язок
        public ReportParameter ServiceNameFull { get; set; } // служба зв'язку
        public ReportParameter ServiceNameGenitive { get; set; } // служби зв'язку
        public ReportParameter HeadOfServiceName { get; set; }
        public ReportParameter HeadOfServicePosition { get; set; }
        public ReportParameter HeadOfServiceRank { get; set; }

        public string Description { get; set; }

        public MilitaryServiceDTO()
        {
            // fill default values on creation
            AssetTypes = [];

            ServiceName = new ReportParameter(ReportConfigHelper.FIELD_SERVICE);
            ServiceNameFull = new ReportParameter(ReportConfigHelper.FIELD_SERVICE_NAME);
            ServiceNameGenitive = new ReportParameter(ReportConfigHelper.FIELD_SERVICE_NAME_GENITIVE);
            HeadOfServiceName = new ReportParameter(ReportConfigHelper.FIELD_HEAD_OF_SERVICE_NAME);
            HeadOfServicePosition = new ReportParameter(ReportConfigHelper.FIELD_HEAD_OF_SERVICE_POSITION);
            HeadOfServiceRank = new ReportParameter(ReportConfigHelper.FIELD_HEAD_OF_SERVICE_RANK);
        }

        public MilitaryServiceDTO(MilitaryServiceDTO sourceDTO)
        {
            UpdateProperties(sourceDTO);
        }

        public void UpdateProperties(MilitaryServiceDTO sourceDTO)
        {
            AssetTypes = sourceDTO.AssetTypes;
            ServiceNameFull = sourceDTO.ServiceNameFull;
            ServiceName = sourceDTO.ServiceName;
            ServiceNameGenitive = sourceDTO.ServiceNameGenitive;
            HeadOfServiceName = sourceDTO.HeadOfServiceName;
            HeadOfServiceRank = sourceDTO.HeadOfServiceRank;
            HeadOfServicePosition = sourceDTO.HeadOfServicePosition;
            Description = sourceDTO.Description;
        }

        public List<ReportParameter> GetAllParameters()
        {
            return [
                ServiceName,
                ServiceNameFull,
                ServiceNameGenitive,
                HeadOfServiceName,
                HeadOfServicePosition,
                HeadOfServiceRank
            ];
        }
    }
}
