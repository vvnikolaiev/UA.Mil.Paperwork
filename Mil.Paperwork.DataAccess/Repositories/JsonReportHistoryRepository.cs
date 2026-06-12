using Mil.Paperwork.DataAccess.DataModels.History;
using Mil.Paperwork.DataAccess.Helpers;
using Mil.Paperwork.DataAccess.Mappers;
using System.IO;
using System.Text.Json;

namespace Mil.Paperwork.DataAccess.Repositories
{
    public class JsonReportHistoryRepository : IReportHistoryRepository
    {
        private const string DataFolderName = "Data";
        private const string HistoryFolderName = "History";
        private const string EntriesFolderName = "Entries";
        private const string IndexFileName = "index.json";
        private const string EntryFileExtension = ".json";
        private const string EntryFileSearchPattern = "*.json";

        private readonly object _sync = new();
        private readonly string _historyDirectory;
        private readonly string _entriesDirectory;
        private readonly string _indexFilePath;

        public JsonReportHistoryRepository() : this(AppDomain.CurrentDomain.BaseDirectory)
        {
        }

        public JsonReportHistoryRepository(string baseDirectory)
        {
            _historyDirectory = Path.Combine(baseDirectory, DataFolderName, HistoryFolderName);
            _entriesDirectory = Path.Combine(_historyDirectory, EntriesFolderName);
            _indexFilePath = Path.Combine(_historyDirectory, IndexFileName);
        }

        public IReadOnlyList<ReportHistoryIndexEntry> GetIndex()
        {
            lock (_sync)
            {
                var index = ReadIndexFile();
                index ??= RebuildIndexInternal();

                return index;
            }
        }

        public ReportHistoryEntry? GetEntry(Guid id)
        {
            lock (_sync)
            {
                var entry = ReadEntryFile(GetEntryFilePath(id));
                return entry;
            }
        }

        public void Save(ReportHistoryEntry entry)
        {
            lock (_sync)
            {
                if (entry.Id == Guid.Empty)
                {
                    entry.Id = Guid.NewGuid();
                }

                if (entry.CreatedAt == default)
                {
                    entry.CreatedAt = DateTime.Now;
                }

                entry.ModifiedAt = DateTime.Now;
                entry.SchemaVersion = HistorySchema.CurrentVersion;

                WriteEntryFile(entry);

                var index = ReadIndexFile() ?? RebuildIndexInternal();
                index.RemoveAll(indexEntry => indexEntry.Id == entry.Id);
                index.Add(ReportSnapshotMapper.ToIndexEntry(entry));
                WriteIndexFile(index);
            }
        }

        public void Delete(Guid id)
        {
            lock (_sync)
            {
                var entryFilePath = GetEntryFilePath(id);
                if (File.Exists(entryFilePath))
                {
                    File.Delete(entryFilePath);
                }

                var index = ReadIndexFile() ?? RebuildIndexInternal();
                index.RemoveAll(indexEntry => indexEntry.Id == id);
                WriteIndexFile(index);
            }
        }

        public void RebuildIndex()
        {
            lock (_sync)
            {
                RebuildIndexInternal();
            }
        }

        private List<ReportHistoryIndexEntry> RebuildIndexInternal()
        {
            var index = new List<ReportHistoryIndexEntry>();

            if (Directory.Exists(_entriesDirectory))
            {
                var entryFiles = Directory.EnumerateFiles(_entriesDirectory, EntryFileSearchPattern);
                foreach (var entryFile in entryFiles)
                {
                    var entry = ReadEntryFile(entryFile);
                    if (entry != null)
                    {
                        index.Add(ReportSnapshotMapper.ToIndexEntry(entry));
                    }
                }
            }

            WriteIndexFile(index);

            return index;
        }

        private List<ReportHistoryIndexEntry>? ReadIndexFile()
        {
            if (!File.Exists(_indexFilePath))
            {
                return null;
            }

            try
            {
                var jsonContent = File.ReadAllText(_indexFilePath);
                var index = JsonSerializer.Deserialize<List<ReportHistoryIndexEntry>>(jsonContent, HistoryJsonOptions.Default);
                return index;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void WriteIndexFile(List<ReportHistoryIndexEntry> index)
        {
            Directory.CreateDirectory(_historyDirectory);

            var jsonContent = JsonSerializer.Serialize(index, HistoryJsonOptions.Default);
            File.WriteAllText(_indexFilePath, jsonContent);
        }

        private ReportHistoryEntry? ReadEntryFile(string entryFilePath)
        {
            if (!File.Exists(entryFilePath))
            {
                return null;
            }

            try
            {
                var jsonContent = File.ReadAllText(entryFilePath);
                var entry = JsonSerializer.Deserialize<ReportHistoryEntry>(jsonContent, HistoryJsonOptions.Default);
                return entry;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void WriteEntryFile(ReportHistoryEntry entry)
        {
            Directory.CreateDirectory(_entriesDirectory);

            var jsonContent = JsonSerializer.Serialize(entry, HistoryJsonOptions.Default);
            File.WriteAllText(GetEntryFilePath(entry.Id), jsonContent);
        }

        private string GetEntryFilePath(Guid id)
        {
            var result = Path.Combine(_entriesDirectory, id + EntryFileExtension);
            return result;
        }
    }
}
