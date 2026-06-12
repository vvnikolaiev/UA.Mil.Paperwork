using Mil.Paperwork.DataAccess.Conversions;
using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Tests.History
{
    public class ReportDataConversionTests
    {
        private static PersonDTO CreatePerson(string firstName, string lastName)
        {
            var person = new PersonDTO(firstName, lastName, "технік", "солдат")
            {
                Patronymic = "Іванович"
            };

            return person;
        }

        private static AssetInfo CreateAsset(string name, string serialNumber, int count = 1)
        {
            var asset = new AssetInfo
            {
                Name = name,
                ShortName = "Р/С",
                MeasurementUnit = "шт.",
                SerialNumber = serialNumber,
                NomenclatureCode = "NC-77",
                Price = 1500.50m,
                Count = count,
                StartDate = new DateTime(2022, 05, 10),
                WarrantyPeriodMonths = 24,
                YearManufactured = 2021,
                ResourceYears = 7
            };

            return asset;
        }

        private static InvoceReportData CreateInvoiceData()
        {
            var invoiceData = new InvoceReportData
            {
                DocumentNumber = "ВН-15",
                DateCreated = new DateTime(2026, 02, 10),
                DueDate = new DateTime(2026, 02, 20),
                Reason = "передача майна",
                Recipient = CreatePerson("Ігор", "Петренко"),
                Transmitter = CreatePerson("Олег", "Шевченко"),
                Assets =
                [
                    CreateAsset("Радіостанція", "SN-01", 2),
                    CreateAsset("Антена", "SN-02"),
                    CreateAsset("Блок живлення", "SN-03", 5)
                ]
            };

            return invoiceData;
        }

        private static ResidualValueReportData CreateResidualValueData()
        {
            var residualValueData = new ResidualValueReportData
            {
                EventDate = new DateTime(2026, 03, 01),
                Assets =
                [
                    CreateAsset("Радіостанція", "SN-01", 2),
                    CreateAsset("Антена", "SN-02")
                ]
            };

            return residualValueData;
        }

        [Fact]
        public void InvoiceToCommissioningAct_MultiAssetInvoice_ProducesActPerAsset()
        {
            var conversion = new InvoiceToCommissioningActConversion();
            var invoiceData = CreateInvoiceData();

            var results = conversion.Convert(invoiceData);

            Assert.Equal(invoiceData.Assets.Count, results.Count);
        }

        [Fact]
        public void InvoiceToCommissioningAct_MapsFields()
        {
            var conversion = new InvoiceToCommissioningActConversion();
            var invoiceData = CreateInvoiceData();

            var results = conversion.Convert(invoiceData);

            var actData = Assert.IsType<CommissioningActReportData>(results[0]);
            var sourceAsset = invoiceData.Assets[0];

            Assert.Same(sourceAsset, actData.Asset);
            Assert.Equal(sourceAsset.Count, actData.Count);
            Assert.Equal(invoiceData.DocumentNumber, actData.DocumentNumber);
            Assert.Equal(invoiceData.DateCreated, actData.DocumentDate);
            Assert.Same(invoiceData.Transmitter, actData.PersonHanded);
            Assert.Same(invoiceData.Recipient, actData.PersonAccepted);

            var assetId = Assert.Single(actData.AssetIds);
            Assert.Equal(sourceAsset.SerialNumber, assetId.SerialNumber);
        }

        [Fact]
        public void InvoiceToCommissioningAct_WrongSourceType_ReturnsEmpty()
        {
            var conversion = new InvoiceToCommissioningActConversion();

            var results = conversion.Convert(new ResidualValueReportData());

            Assert.Empty(results);
        }

        [Fact]
        public void InvoiceToInitialTechnicalState_MapsFields()
        {
            var conversion = new InvoiceToInitialTechnicalStateConversion();
            var invoiceData = CreateInvoiceData();

            var results = conversion.Convert(invoiceData);

            var reportData = Assert.IsType<InitialTechnicalStateReportData>(Assert.Single(results));
            Assert.Equal(invoiceData.Assets, reportData.Assets);
            Assert.Same(invoiceData.Recipient, reportData.PersonAccepted);
            Assert.Same(invoiceData.Transmitter, reportData.PersonHanded);
        }

        [Fact]
        public void ResidualValueToWriteOffPackage_MapsFields()
        {
            var conversion = new ResidualValueToWriteOffPackageConversion();
            var residualValueData = CreateResidualValueData();

            var results = conversion.Convert(residualValueData);

            var reportData = Assert.IsType<WriteOffPackageReportData>(Assert.Single(results));
            Assert.Equal(residualValueData.Assets, reportData.Assets);
            Assert.Equal(residualValueData.EventDate, reportData.EventDate);
            Assert.Equal(DateTime.Now.Date, reportData.DocumentDate);
            Assert.Equal(DateTime.Now.Date, reportData.OrdenDate);
        }

        [Fact]
        public void ResidualValueToInitialTechnicalState_MapsAssets()
        {
            var conversion = new ResidualValueToInitialTechnicalStateConversion();
            var residualValueData = CreateResidualValueData();

            var results = conversion.Convert(residualValueData);

            var reportData = Assert.IsType<InitialTechnicalStateReportData>(Assert.Single(results));
            Assert.Equal(residualValueData.Assets, reportData.Assets);
        }

        [Fact]
        public void Registry_GetTargets_ReturnsConfiguredTargets()
        {
            var registry = CreateRegistry();

            var invoiceTargets = registry.GetTargets(ReportType.Invoice);
            var residualValueTargets = registry.GetTargets(ReportType.ResidualValueReport);
            var emptyTargets = registry.GetTargets(ReportType.Handover23Act);

            Assert.Equal([ReportType.CommissioningAct, ReportType.TechnicalStateReport], invoiceTargets);
            Assert.Equal([ReportType.WriteOffPackage, ReportType.TechnicalStateReport], residualValueTargets);
            Assert.Empty(emptyTargets);
        }

        [Fact]
        public void Registry_Convert_UnknownPair_ReturnsEmpty()
        {
            var registry = CreateRegistry();

            var results = registry.Convert(ReportType.Invoice, ReportType.WriteOffPackage, CreateInvoiceData());

            Assert.Empty(results);
        }

        [Fact]
        public void Registry_Convert_KnownPair_DelegatesToConversion()
        {
            var registry = CreateRegistry();
            var invoiceData = CreateInvoiceData();

            var results = registry.Convert(ReportType.Invoice, ReportType.CommissioningAct, invoiceData);

            Assert.Equal(invoiceData.Assets.Count, results.Count);
            Assert.All(results, result => Assert.IsType<CommissioningActReportData>(result));
        }

        private static ReportConversionRegistry CreateRegistry()
        {
            var registry = new ReportConversionRegistry(
            [
                new InvoiceToCommissioningActConversion(),
                new InvoiceToInitialTechnicalStateConversion(),
                new ResidualValueToWriteOffPackageConversion(),
                new ResidualValueToInitialTechnicalStateConversion()
            ]);

            return registry;
        }
    }
}
