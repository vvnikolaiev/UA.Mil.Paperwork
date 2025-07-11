﻿using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Infrastructure.Services
{
    public interface IReportDataService
    {
        void SaveReportConfig(IReadOnlyCollection<ReportParameter> parameters, ReportType reportType);
        void SaveReportConfigTemprorary(IReadOnlyCollection<ReportParameter> parameters, ReportType reportType);

        Dictionary<string, string> GetReportParametersDictionary(ReportType reportType);

        List<ReportParameter> GetReportParameters(ReportType reportType, bool withReload = false);

        AssetType GetAssetType();

        void SetAssetType(AssetType assetType);
    }
}
