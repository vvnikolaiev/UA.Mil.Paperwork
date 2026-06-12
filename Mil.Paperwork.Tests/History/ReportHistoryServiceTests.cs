using Mil.Paperwork.DataAccess.Enums;
using Mil.Paperwork.DataAccess.Repositories;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Tests.History
{
    public class ReportHistoryServiceTests : IDisposable
    {
        private readonly string _baseDirectory;
        private readonly JsonReportHistoryRepository _repository;
        private readonly ReportHistoryService _service;

        public ReportHistoryServiceTests()
        {
            _baseDirectory = Path.Combine(Path.GetTempPath(), "MilPaperworkHistoryServiceTests", Guid.NewGuid().ToString());
            _repository = new JsonReportHistoryRepository(_baseDirectory);
            _service = new ReportHistoryService(_repository);
        }

        public void Dispose()
        {
            if (Directory.Exists(_baseDirectory))
            {
                Directory.Delete(_baseDirectory, recursive: true);
            }
        }

        private static InvoceReportData CreateInvoiceData(string documentNumber = "Н-1")
        {
            var data = new InvoceReportData
            {
                DocumentNumber = documentNumber,
                Recipient = new PersonDTO("Олег", "Шевченко", "технік", "солдат"),
                Transmitter = new PersonDTO("Ігор", "Петренко", "начальник складу", "сержант"),
                Assets = new List<IAssetInfo>
                {
                    new AssetInfo { Name = "Радіостанція", SerialNumber = "SN-1", NomenclatureCode = "NC-1" }
                }
            };

            return data;
        }

        [Fact]
        public void SaveDraft_NewEntry_CreatesDraftWithMetadata()
        {
            var data = CreateInvoiceData();

            var entryId = _service.SaveDraft(ReportType.Invoice, data, null);
            var entry = _repository.GetEntry(entryId);

            Assert.NotNull(entry);
            Assert.Equal(HistoryEntryStatus.Draft, entry.Status);
            Assert.Equal(ReportType.Invoice, entry.ReportType);
            Assert.Equal("Н-1", entry.DocumentNumber);
            Assert.Contains("Радіостанція", entry.Summary);
            Assert.Empty(entry.GeneratedFiles);
            Assert.NotEqual(default, entry.CreatedAt);
        }

        [Fact]
        public void SaveDraft_SameEntryIdTwice_UpsertsSingleEntry()
        {
            var data = CreateInvoiceData();

            var firstId = _service.SaveDraft(ReportType.Invoice, data, null);
            var createdAt = _repository.GetEntry(firstId)!.CreatedAt;

            data.DocumentNumber = "Н-2";
            var secondId = _service.SaveDraft(ReportType.Invoice, data, firstId);

            Assert.Equal(firstId, secondId);
            var index = _repository.GetIndex();
            var indexEntry = Assert.Single(index);
            Assert.Equal("Н-2", indexEntry.DocumentNumber);

            var entry = _repository.GetEntry(firstId);
            Assert.NotNull(entry);
            Assert.Equal(createdAt, entry.CreatedAt);
        }

        [Fact]
        public void SaveGenerated_SetsStatusAndFiles()
        {
            var data = CreateInvoiceData();
            var files = new List<string> { "C:\\Out\\invoice.docx" };

            var entryId = _service.SaveGenerated(ReportType.Invoice, data, files, null);
            var entry = _repository.GetEntry(entryId);

            Assert.NotNull(entry);
            Assert.Equal(HistoryEntryStatus.Generated, entry.Status);
            Assert.Equal(files, entry.GeneratedFiles);
        }

        [Fact]
        public void SaveGenerated_AfterDraft_FlipsSameEntryToGenerated()
        {
            var data = CreateInvoiceData();

            var draftId = _service.SaveDraft(ReportType.Invoice, data, null);
            var generatedId = _service.SaveGenerated(ReportType.Invoice, data, ["C:\\Out\\invoice.docx"], draftId);

            Assert.Equal(draftId, generatedId);

            var index = _repository.GetIndex();
            var indexEntry = Assert.Single(index);
            Assert.Equal(HistoryEntryStatus.Generated, indexEntry.Status);
            Assert.Single(indexEntry.GeneratedFiles);
        }

        [Fact]
        public void SaveDraft_AfterGenerated_KeepsGeneratedFiles()
        {
            var data = CreateInvoiceData();
            var files = new List<string> { "C:\\Out\\invoice.docx" };

            var entryId = _service.SaveGenerated(ReportType.Invoice, data, files, null);
            _service.SaveDraft(ReportType.Invoice, data, entryId);

            var entry = _repository.GetEntry(entryId);
            Assert.NotNull(entry);
            Assert.Equal(HistoryEntryStatus.Draft, entry.Status);
            Assert.Equal(files, entry.GeneratedFiles);
        }

        [Fact]
        public void SaveDraft_EmptyReportData_SavesWithoutValidation()
        {
            var data = new InvoceReportData();

            var entryId = _service.SaveDraft(ReportType.Invoice, data, null);
            var entry = _repository.GetEntry(entryId);

            Assert.NotNull(entry);
            Assert.Equal(string.Empty, entry.Summary);
        }
    }
}
