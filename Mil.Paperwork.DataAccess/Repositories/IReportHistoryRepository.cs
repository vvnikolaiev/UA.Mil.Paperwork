using Mil.Paperwork.DataAccess.DataModels.History;

namespace Mil.Paperwork.DataAccess.Repositories
{
    public interface IReportHistoryRepository
    {
        IReadOnlyList<ReportHistoryIndexEntry> GetIndex();
        ReportHistoryEntry? GetEntry(Guid id);
        void Save(ReportHistoryEntry entry);
        void Delete(Guid id);
        void RebuildIndex();
    }
}
