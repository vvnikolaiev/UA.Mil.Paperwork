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

        private static HandoverReportData CreateHandoverData()
        {
            var handoverData = new HandoverReportData
            {
                DocumentNumber = "ПП-7",
                DocumentDate = new DateTime(2026, 04, 05),
                ReasonDocumentName = "наказ командира",
                PersonResponsible = CreatePerson("Тарас", "Бондаренко"),
                PersonReceiver = CreatePerson("Андрій", "Коваленко"),
                Assets =
                [
                    CreateAsset("Радіостанція", "SN-01", 2),
                    CreateAsset("Антена", "SN-02")
                ]
            };

            return handoverData;
        }

        private static CommissioningActReportData CreateCommissioningActData()
        {
            var actData = new CommissioningActReportData
            {
                DocumentNumber = "АВ-3",
                DocumentDate = new DateTime(2026, 05, 12),
                Asset = CreateAsset("Генератор", "SN-10"),
                AssetIds = [new ProductIdentification { SerialNumber = "SN-10" }],
                Count = 3,
                PersonAccepted = CreatePerson("Ігор", "Петренко"),
                PersonHanded = CreatePerson("Олег", "Шевченко")
            };

            return actData;
        }

        [Fact]
        public void Handover23ToInvoice_MapsFields()
        {
            var conversion = new Handover23ToInvoiceConversion();
            var handoverData = CreateHandoverData();

            var results = conversion.Convert(handoverData);

            var invoiceData = Assert.IsType<InvoceReportData>(Assert.Single(results));
            Assert.Equal(handoverData.DocumentNumber, invoiceData.DocumentNumber);
            Assert.Equal(handoverData.DocumentDate, invoiceData.DateCreated);
            Assert.Equal(handoverData.DocumentDate.AddDays(10), invoiceData.DueDate);
            Assert.Equal(handoverData.ReasonDocumentName, invoiceData.Reason);
            Assert.Same(handoverData.PersonResponsible, invoiceData.Transmitter);
            Assert.Same(handoverData.PersonReceiver, invoiceData.Recipient);
            Assert.Equal(handoverData.Assets, invoiceData.Assets);
        }

        [Fact]
        public void Handover23ToInvoice_WrongSourceType_ReturnsEmpty()
        {
            var conversion = new Handover23ToInvoiceConversion();

            var results = conversion.Convert(new ResidualValueReportData());

            Assert.Empty(results);
        }

        [Fact]
        public void InvoiceToHandover23_MapsFields()
        {
            var conversion = new InvoiceToHandover23Conversion();
            var invoiceData = CreateInvoiceData();

            var results = conversion.Convert(invoiceData);

            var handoverData = Assert.IsType<HandoverReportData>(Assert.Single(results));
            Assert.Equal(invoiceData.DocumentNumber, handoverData.DocumentNumber);
            Assert.Equal(invoiceData.DateCreated, handoverData.DocumentDate);
            Assert.Equal(invoiceData.Reason, handoverData.ReasonDocumentName);
            Assert.Equal(invoiceData.DateCreated, handoverData.ReasonDocumentDate);
            Assert.Same(invoiceData.Transmitter, handoverData.PersonResponsible);
            Assert.Same(invoiceData.Recipient, handoverData.PersonReceiver);
            Assert.Equal(invoiceData.Assets, handoverData.Assets);
        }

        [Fact]
        public void CommissioningActToInvoice_MapsFields()
        {
            var conversion = new CommissioningActToInvoiceConversion();
            var actData = CreateCommissioningActData();

            var results = conversion.Convert(actData);

            var invoiceData = Assert.IsType<InvoceReportData>(Assert.Single(results));
            Assert.Equal(actData.DocumentNumber, invoiceData.DocumentNumber);
            Assert.Equal(actData.DocumentDate, invoiceData.DateCreated);
            Assert.Equal(actData.DocumentDate.AddDays(10), invoiceData.DueDate);
            Assert.Same(actData.PersonHanded, invoiceData.Transmitter);
            Assert.Same(actData.PersonAccepted, invoiceData.Recipient);

            var asset = Assert.Single(invoiceData.Assets);
            Assert.Same(actData.Asset, asset);
            Assert.Equal(actData.Count, asset.Count);
        }

        [Fact]
        public void CommissioningActToInvoice_ProductAsset_WrapsIntoAssetInfo()
        {
            var conversion = new CommissioningActToInvoiceConversion();
            var actData = CreateCommissioningActData();
            actData.Asset = new ProductDTO
            {
                Name = "Генератор",
                MeasurementUnit = "шт.",
                NomenclatureCode = "NC-77",
                Price = 1500.50m,
                StartDate = new DateTime(2022, 05, 10)
            };

            var results = conversion.Convert(actData);

            var invoiceData = Assert.IsType<InvoceReportData>(Assert.Single(results));
            var asset = Assert.Single(invoiceData.Assets);
            Assert.IsType<AssetInfo>(asset);
            Assert.Equal(actData.Asset.Name, asset.Name);
            Assert.Equal(actData.Asset.Price, asset.Price);
            Assert.Equal("SN-10", asset.SerialNumber);
            Assert.Equal(actData.Count, asset.Count);
        }

        [Fact]
        public void CommissioningActToInvoice_NoAsset_ReturnsEmpty()
        {
            var conversion = new CommissioningActToInvoiceConversion();
            var actData = CreateCommissioningActData();
            actData.Asset = null;

            var results = conversion.Convert(actData);

            Assert.Empty(results);
        }

        [Fact]
        public void CommissioningActToInitialTechnicalState_MapsFields()
        {
            var conversion = new CommissioningActToInitialTechnicalStateConversion();
            var actData = CreateCommissioningActData();

            var results = conversion.Convert(actData);

            var reportData = Assert.IsType<InitialTechnicalStateReportData>(Assert.Single(results));
            var asset = Assert.Single(reportData.Assets);
            Assert.Same(actData.Asset, asset);
            Assert.Same(actData.PersonAccepted, reportData.PersonAccepted);
            Assert.Same(actData.PersonHanded, reportData.PersonHanded);
        }

        [Fact]
        public void Registry_GetTargets_ReturnsConfiguredTargets()
        {
            var registry = CreateRegistry();

            var invoiceTargets = registry.GetTargets(ReportType.Invoice);
            var residualValueTargets = registry.GetTargets(ReportType.ResidualValueReport);
            var handoverTargets = registry.GetTargets(ReportType.Handover23Act);
            var commissioningActTargets = registry.GetTargets(ReportType.CommissioningAct);
            var emptyTargets = registry.GetTargets(ReportType.WriteOffOrder);

            Assert.Equal([ReportType.CommissioningAct, ReportType.TechnicalStateReport, ReportType.Handover23Act], invoiceTargets);
            Assert.Equal([ReportType.WriteOffPackage, ReportType.TechnicalStateReport], residualValueTargets);
            Assert.Equal([ReportType.Invoice], handoverTargets);
            Assert.Equal([ReportType.Invoice, ReportType.TechnicalStateReport], commissioningActTargets);
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
                new ResidualValueToInitialTechnicalStateConversion(),
                new Handover23ToInvoiceConversion(),
                new InvoiceToHandover23Conversion(),
                new CommissioningActToInvoiceConversion(),
                new CommissioningActToInitialTechnicalStateConversion()
            ]);

            return registry;
        }
    }
}
