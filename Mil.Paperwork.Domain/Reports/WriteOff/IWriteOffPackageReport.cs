using Mil.Paperwork.Domain.DataModels.Parameters;

namespace Mil.Paperwork.Domain.Reports.WriteOff
{
    public interface IWriteOffPackageReport : IReport
    {
        string OutputFileName { get; }
        bool TryCreate(IWriteOffPackageParameters reportParameters);
    }
}
