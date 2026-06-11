using Mil.Paperwork.DataAccess.DataModels.History;
using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.DataAccess.Enums;
using Mil.Paperwork.DataAccess.Helpers;
using Mil.Paperwork.DataAccess.Repositories;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Tests.History
{
    public class JsonReportHistoryRepositoryTests : IDisposable
    {
        private readonly string _baseDirectory;
        private readonly JsonReportHistoryRepository _repository;

        public JsonReportHistoryRepositoryTests()
        {
            _baseDirectory = Path.Combine(Path.GetTempPath(), "MilPaperworkHistoryTests", Guid.NewGuid().ToString());
            _repository = new JsonReportHistoryRepository(_baseDirectory);
        }

        public void Dispose()
        {
            if (Directory.Exists(_baseDirectory))
            {
                Directory.Delete(_baseDirectory, recursive: true);
            }
        }

        private string HistoryDirectory => Path.Combine(_baseDirectory, "Data", "History");
        private string EntriesDirectory => Path.Combine(HistoryDirectory, "Entries");
        private string IndexFilePath => Path.Combine(HistoryDirectory, "index.json");

        private static ReportHistoryEntry CreateEntry(string documentNumber = "123")
        {
            var entry = new ReportHistoryEntry
            {
                Id = Guid.NewGuid(),
                ReportType = ReportType.Invoice,
                Status = HistoryEntryStatus.Draft,
                CreatedAt = new DateTime(2026, 01, 15, 10, 30, 00),
                DocumentNumber = documentNumber,
                Summary = "Тестова накладна",
                GeneratedFiles = ["C:\\Out\\invoice.docx"],
                Snapshot = new InvoiceReportSnapshot
                {
                    DocumentNumber = documentNumber,
                    Reason = "перевірка",
                    Assets =
                    [
                        new AssetSnapshot { Name = "Радіостанція", SerialNumber = "SN-1", NomenclatureCode = "NC-1" }
                    ]
                }
            };

            return entry;
        }

        [Fact]
        public void Save_ThenGetEntry_ReturnsEqualEntry()
        {
            var entry = CreateEntry();

            _repository.Save(entry);
            var loaded = _repository.GetEntry(entry.Id);

            Assert.NotNull(loaded);
            Assert.Equal(entry.Id, loaded.Id);
            Assert.Equal(ReportType.Invoice, loaded.ReportType);
            Assert.Equal(HistoryEntryStatus.Draft, loaded.Status);
            Assert.Equal(entry.DocumentNumber, loaded.DocumentNumber);
            Assert.Equal(entry.Summary, loaded.Summary);
            Assert.Equal(entry.GeneratedFiles, loaded.GeneratedFiles);
            Assert.Equal(HistorySchema.CurrentVersion, loaded.SchemaVersion);
            Assert.NotEqual(default, loaded.ModifiedAt);

            var snapshot = Assert.IsType<InvoiceReportSnapshot>(loaded.Snapshot);
            Assert.Equal("перевірка", snapshot.Reason);
            Assert.Single(snapshot.Assets);
            Assert.Equal("Радіостанція", snapshot.Assets[0].Name);
        }

        [Fact]
        public void Save_AddsIndexRowWithSearchFields()
        {
            var entry = CreateEntry();

            _repository.Save(entry);
            var index = _repository.GetIndex();

            var indexEntry = Assert.Single(index);
            Assert.Equal(entry.Id, indexEntry.Id);
            Assert.Equal(entry.DocumentNumber, indexEntry.DocumentNumber);
            Assert.Contains("Радіостанція", indexEntry.AssetNames);
            Assert.Contains("SN-1", indexEntry.SerialNumbers);
            Assert.Contains("NC-1", indexEntry.NomenclatureCodes);
        }

        [Fact]
        public void Save_SameId_UpsertsSingleIndexRow()
        {
            var entry = CreateEntry();

            _repository.Save(entry);
            entry.DocumentNumber = "456";
            _repository.Save(entry);

            var index = _repository.GetIndex();
            var indexEntry = Assert.Single(index);
            Assert.Equal("456", indexEntry.DocumentNumber);
        }

        [Fact]
        public void Save_EmptyId_AssignsNewId()
        {
            var entry = CreateEntry();
            entry.Id = Guid.Empty;

            _repository.Save(entry);

            Assert.NotEqual(Guid.Empty, entry.Id);
            Assert.NotNull(_repository.GetEntry(entry.Id));
        }

        [Fact]
        public void Delete_RemovesEntryAndIndexRow()
        {
            var entry = CreateEntry();
            _repository.Save(entry);

            _repository.Delete(entry.Id);

            Assert.Null(_repository.GetEntry(entry.Id));
            Assert.Empty(_repository.GetIndex());
            Assert.Empty(Directory.GetFiles(EntriesDirectory));
        }

        [Fact]
        public void GetEntry_UnknownId_ReturnsNull()
        {
            var loaded = _repository.GetEntry(Guid.NewGuid());

            Assert.Null(loaded);
        }

        [Fact]
        public void GetIndex_MissingIndexFile_RebuildsFromEntries()
        {
            var firstEntry = CreateEntry("111");
            var secondEntry = CreateEntry("222");
            _repository.Save(firstEntry);
            _repository.Save(secondEntry);

            File.Delete(IndexFilePath);
            var index = _repository.GetIndex();

            Assert.Equal(2, index.Count);
            Assert.Contains(index, indexEntry => indexEntry.Id == firstEntry.Id);
            Assert.Contains(index, indexEntry => indexEntry.Id == secondEntry.Id);
            Assert.True(File.Exists(IndexFilePath));
        }

        [Fact]
        public void GetIndex_CorruptIndexFile_RebuildsFromEntries()
        {
            var entry = CreateEntry();
            _repository.Save(entry);

            File.WriteAllText(IndexFilePath, "{ not valid json ");
            var index = _repository.GetIndex();

            var indexEntry = Assert.Single(index);
            Assert.Equal(entry.Id, indexEntry.Id);
        }

        [Fact]
        public void GetIndex_CorruptEntryFile_SkipsItAndReturnsOthers()
        {
            var goodEntry = CreateEntry("111");
            var corruptEntry = CreateEntry("222");
            _repository.Save(goodEntry);
            _repository.Save(corruptEntry);

            File.WriteAllText(Path.Combine(EntriesDirectory, corruptEntry.Id + ".json"), "broken");
            File.Delete(IndexFilePath);
            var index = _repository.GetIndex();

            var indexEntry = Assert.Single(index);
            Assert.Equal(goodEntry.Id, indexEntry.Id);
        }
    }
}
