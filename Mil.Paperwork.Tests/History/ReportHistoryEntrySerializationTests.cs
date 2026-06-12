using Mil.Paperwork.DataAccess.DataModels.History;
using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.DataAccess.Enums;
using Mil.Paperwork.DataAccess.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using System.Text.Json;

namespace Mil.Paperwork.Tests.History
{
    public class ReportHistoryEntrySerializationTests
    {
        public static TheoryData<ReportSnapshotBase> Snapshots => new()
        {
            new InvoiceReportSnapshot { DocumentNumber = "Н-1" },
            new ResidualValueReportSnapshot { MetalCosts = new Dictionary<string, decimal> { ["XAU"] = 1m } },
            new InitialTechnicalStateReportSnapshot { EventType = 1 },
            new TechnicalStateReportSnapshot { Reason = "бойові дії" },
            new WriteOffPackageReportSnapshot { OrdenNumber = 5, BookOfLossesExtract = new BookExtractSnapshot { Year = 2026 } },
            new CommissioningActReportSnapshot { DocumentNumber = "А-1", Asset = new ProductSnapshot { Name = "Генератор" } },
            new ValuationReportSnapshot { ValuationData = [new AssetValuationSnapshot { Name = "Ноутбук" }] },
            new DismantlingReportSnapshot { Dismantlings = [new AssetDismantlingSnapshot { Name = "Сервер" }] },
            new Handover23ReportSnapshot { Supplier = "в/ч А1111" },
            new WriteOffOrderReportSnapshot { ReportNum = "12" }
        };

        [Theory]
        [MemberData(nameof(Snapshots))]
        public void Entry_WithSnapshot_RoundTripsThroughJson(ReportSnapshotBase snapshot)
        {
            var entry = new ReportHistoryEntry
            {
                SchemaVersion = HistorySchema.CurrentVersion,
                Id = Guid.NewGuid(),
                ReportType = ReportType.Invoice,
                Status = HistoryEntryStatus.Generated,
                CreatedAt = new DateTime(2026, 01, 01, 09, 00, 00),
                ModifiedAt = new DateTime(2026, 01, 02, 10, 00, 00),
                DocumentNumber = "42",
                Summary = "Тест",
                GeneratedFiles = ["C:\\Out\\report.docx"],
                Snapshot = snapshot
            };

            var json = JsonSerializer.Serialize(entry, HistoryJsonOptions.Default);
            var restored = JsonSerializer.Deserialize<ReportHistoryEntry>(json, HistoryJsonOptions.Default);

            Assert.Contains("$type", json);
            Assert.NotNull(restored);
            Assert.Equal(entry.Id, restored.Id);
            Assert.Equal(entry.SchemaVersion, restored.SchemaVersion);
            Assert.Equal(entry.Status, restored.Status);
            Assert.Equal(entry.CreatedAt, restored.CreatedAt);
            Assert.Equal(entry.ModifiedAt, restored.ModifiedAt);
            Assert.Equal(entry.DocumentNumber, restored.DocumentNumber);
            Assert.Equal(entry.Summary, restored.Summary);
            Assert.Equal(entry.GeneratedFiles, restored.GeneratedFiles);
            Assert.NotNull(restored.Snapshot);
            Assert.IsType(snapshot.GetType(), restored.Snapshot);
        }
    }
}
