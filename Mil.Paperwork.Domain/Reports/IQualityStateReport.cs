﻿using Mil.Paperwork.Domain.DataModels;

namespace Mil.Paperwork.Domain.Reports
{
    public interface IQualityStateReport : IReport
    {
        bool TryCreate(WriteOffReportData reportData);
    }
}
