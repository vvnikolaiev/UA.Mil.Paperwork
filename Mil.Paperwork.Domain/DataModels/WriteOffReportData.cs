﻿using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels
{
    public class WriteOffReportData : ITechnicalStateReportData, IDismantlingReportData, IAssetValuationReportData
    {
        public string DestinationFolder { get; set; }

        public string RegistrationNumber { get; set; }

        public string DocumentNumber { get; set; }

        public string Reason { get; set; }
        
        public DateTime ReportDate { get; set; }

        public IList<AssetInfo> Assets { get; set; }

        public IList<AssetDismantlingData> Dismantlings { get; set; }

        public IList<IAssetValuationData?> ValuationData => this.GetValuationDataCollection();

        public string GetDestinationPath()
        {
            var destinationPath = PathsHelper.GetDestinationPath(DestinationFolder, ReportDate);
            return destinationPath;
        }

    }
}
