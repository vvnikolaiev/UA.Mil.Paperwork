using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.DataAccess.Enums;
using Mil.Paperwork.DataAccess.Mappers;
using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Tests.History
{
    public class ReportSnapshotMapperTests
    {
        private static PersonDTO CreatePerson(string firstName = "Ігор", string lastName = "Петренко")
        {
            var person = new PersonDTO(firstName, lastName, "технік", "солдат")
            {
                Patronymic = "Іванович"
            };

            return person;
        }

        private static void AssertPersonEqual(IPerson? expected, IPerson? actual)
        {
            Assert.NotNull(expected);
            Assert.NotNull(actual);
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
            Assert.Equal(expected.Patronymic, actual.Patronymic);
            Assert.Equal(expected.Position, actual.Position);
            Assert.Equal(expected.Rank, actual.Rank);
        }

        private static AssetInfo CreateAsset(string name = "Радіостанція", string serialNumber = "SN-01")
        {
            var asset = new AssetInfo
            {
                Name = name,
                ShortName = "Р/С",
                MeasurementUnit = "шт.",
                SerialNumber = serialNumber,
                NomenclatureCode = "NC-77",
                InitialCategory = 3,
                Price = 1500.50m,
                Count = 2,
                StartDate = new DateTime(2022, 05, 10),
                EventType = EventType.Destroyed,
                TSRegisterNumber = "TS-R-1",
                TSDocumentNumber = "TS-D-1",
                WarrantyPeriodMonths = 24,
                YearManufactured = 2021,
                ResourceYears = 7
            };

            return asset;
        }

        private static void AssertAssetEqual(IAssetInfo expected, IAssetInfo actual)
        {
            Assert.Equal(expected.GetType(), actual.GetType());
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.ShortName, actual.ShortName);
            Assert.Equal(expected.MeasurementUnit, actual.MeasurementUnit);
            Assert.Equal(expected.SerialNumber, actual.SerialNumber);
            Assert.Equal(expected.NomenclatureCode, actual.NomenclatureCode);
            Assert.Equal(expected.InitialCategory, actual.InitialCategory);
            Assert.Equal(expected.Price, actual.Price);
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected.StartDate, actual.StartDate);
            Assert.Equal(expected.EventType, actual.EventType);
            Assert.Equal(expected.TSRegisterNumber, actual.TSRegisterNumber);
            Assert.Equal(expected.TSDocumentNumber, actual.TSDocumentNumber);
            Assert.Equal(expected.WarrantyPeriodMonths, actual.WarrantyPeriodMonths);
            Assert.Equal(expected.YearManufactured, actual.YearManufactured);
            Assert.Equal(expected.ResourceYears, actual.ResourceYears);
        }

        [Fact]
        public void Invoice_RoundTrip_PreservesData()
        {
            var data = new InvoceReportData
            {
                DocumentNumber = "Н-12",
                DateCreated = new DateTime(2026, 02, 01),
                DueDate = new DateTime(2026, 02, 10),
                Reason = "переміщення",
                Recipient = CreatePerson("Олег", "Шевченко"),
                Transmitter = CreatePerson(),
                HeadOfService = CreatePerson("Петро", "Грищенко"),
                Assets = new List<IAssetInfo> { CreateAsset() },
                DestinationFolder = "C:\\Out"
            };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.Invoice, data);
            var restored = Assert.IsType<InvoceReportData>(ReportSnapshotMapper.ToReportData(snapshot));

            Assert.Equal(data.DocumentNumber, restored.DocumentNumber);
            Assert.Equal(data.DateCreated, restored.DateCreated);
            Assert.Equal(data.DueDate, restored.DueDate);
            Assert.Equal(data.Reason, restored.Reason);
            Assert.Equal(data.DestinationFolder, restored.DestinationFolder);
            AssertPersonEqual(data.Recipient, restored.Recipient);
            AssertPersonEqual(data.Transmitter, restored.Transmitter);
            AssertPersonEqual(data.HeadOfService, restored.HeadOfService);
            Assert.Single(restored.Assets);
            AssertAssetEqual(data.Assets[0], restored.Assets[0]);
        }

        [Fact]
        public void ResidualValue_RoundTrip_PreservesAllAssetKindsAndMetalCosts()
        {
            var defaultAsset = CreateAsset("Майно 1", "SN-1");
            var radiochemicalAsset = new RadiochemicalAssetInfo { Name = "Дозиметр", SerialNumber = "SN-2", IsLocal = false };
            var connectivityAsset = new ConnectivityAssetInfo { Name = "Комутатор", SerialNumber = "SN-3", WearAndTearCoeff = 0.55m };

            var data = new ResidualValueReportData
            {
                AssetType = AssetType.Connectivity,
                Assets = new List<IAssetInfo> { defaultAsset, radiochemicalAsset, connectivityAsset },
                MetalCosts = new Dictionary<MetalType, decimal>
                {
                    [MetalType.XAU] = 2500.75m,
                    [MetalType.CU] = 12.3m
                },
                EventDate = new DateTime(2025, 12, 31),
                EventReportNumber = 42,
                DestinationFolder = "C:\\Out"
            };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.ResidualValueReport, data);
            var typedSnapshot = Assert.IsType<ResidualValueReportSnapshot>(snapshot);
            Assert.Equal(AssetSnapshotKind.Default, typedSnapshot.Assets[0].Kind);
            Assert.Equal(AssetSnapshotKind.Radiochemical, typedSnapshot.Assets[1].Kind);
            Assert.Equal(AssetSnapshotKind.Connectivity, typedSnapshot.Assets[2].Kind);
            Assert.Equal(2500.75m, typedSnapshot.MetalCosts["XAU"]);
            Assert.Equal(12.3m, typedSnapshot.MetalCosts["CU"]);

            var restored = Assert.IsType<ResidualValueReportData>(ReportSnapshotMapper.ToReportData(snapshot));

            Assert.Equal(data.AssetType, restored.AssetType);
            Assert.Equal(data.EventDate, restored.EventDate);
            Assert.Equal(data.EventReportNumber, restored.EventReportNumber);
            Assert.Equal(3, restored.Assets.Count);
            AssertAssetEqual(defaultAsset, restored.Assets[0]);

            var restoredRadiochemical = Assert.IsType<RadiochemicalAssetInfo>(restored.Assets[1]);
            Assert.False(restoredRadiochemical.IsLocal);

            var restoredConnectivity = Assert.IsType<ConnectivityAssetInfo>(restored.Assets[2]);
            Assert.Equal(0.55m, restoredConnectivity.WearAndTearCoeff);

            Assert.Equal(data.MetalCosts[MetalType.XAU], restored.MetalCosts[MetalType.XAU]);
            Assert.Equal(data.MetalCosts[MetalType.CU], restored.MetalCosts[MetalType.CU]);
        }

        [Fact]
        public void InitialTechnicalState_RoundTrip_PreservesData()
        {
            var data = new InitialTechnicalStateReportData
            {
                EventType = EventType.Accounted,
                Assets = new List<IAssetInfo> { CreateAsset() },
                PersonAccepted = CreatePerson("Олег", "Шевченко"),
                PersonHanded = CreatePerson(),
                DestinationFolder = "C:\\Out"
            };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.TechnicalStateReport, data);
            var restored = Assert.IsType<InitialTechnicalStateReportData>(ReportSnapshotMapper.ToReportData(snapshot));

            Assert.Equal(data.EventType, restored.EventType);
            Assert.Equal(data.DestinationFolder, restored.DestinationFolder);
            AssertPersonEqual(data.PersonAccepted, restored.PersonAccepted);
            AssertPersonEqual(data.PersonHanded, restored.PersonHanded);
            AssertAssetEqual(data.Assets[0], restored.Assets[0]);
        }

        [Fact]
        public void TechnicalState_RoundTrip_PreservesData()
        {
            var data = new TechnicalStateReportData
            {
                EventType = EventType.Lost,
                Assets = new List<IAssetInfo> { CreateAsset() },
                DocumentDate = new DateTime(2026, 03, 03),
                Reason = "бойові дії",
                EventDate = new DateTime(2026, 02, 20),
                OrdenNumber = 15,
                OrdenDate = new DateTime(2026, 02, 25),
                GenerateWriteOffActs = false,
                DestinationFolder = "C:\\Out"
            };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.TechnicalStateReport, data);
            var restored = Assert.IsType<TechnicalStateReportData>(ReportSnapshotMapper.ToReportData(snapshot));

            Assert.Equal(data.EventType, restored.EventType);
            Assert.Equal(data.DocumentDate, restored.DocumentDate);
            Assert.Equal(data.Reason, restored.Reason);
            Assert.Equal(data.EventDate, restored.EventDate);
            Assert.Equal(data.OrdenNumber, restored.OrdenNumber);
            Assert.Equal(data.OrdenDate, restored.OrdenDate);
            Assert.Equal(data.GenerateWriteOffActs, restored.GenerateWriteOffActs);
            AssertAssetEqual(data.Assets[0], restored.Assets[0]);
        }

        [Fact]
        public void WriteOffPackage_RoundTrip_PreservesBookExtract()
        {
            var packageData = new WriteOffPackageReportData
            {
                Assets = new List<IAssetInfo> { CreateAsset() },
                DocumentDate = new DateTime(2026, 04, 01),
                EventDate = new DateTime(2026, 03, 15),
                OrdenNumber = 7,
                OrdenDate = new DateTime(2026, 03, 20),
                BookOfLossesExtractData = new BookExtractData
                {
                    Year = 2026,
                    Number = 3,
                    PageNumber = 12,
                    RecordDate = new DateTime(2026, 03, 21)
                },
                DestinationFolder = "C:\\Out"
            };
            var data = new WriteOffPackageTabData { PackageData = packageData };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.WriteOffPackage, data);
            var restored = Assert.IsType<WriteOffPackageTabData>(ReportSnapshotMapper.ToReportData(snapshot)).PackageData;

            Assert.Equal(packageData.DocumentDate, restored.DocumentDate);
            Assert.Equal(packageData.EventDate, restored.EventDate);
            Assert.Equal(packageData.OrdenNumber, restored.OrdenNumber);
            Assert.Equal(packageData.OrdenDate, restored.OrdenDate);
            AssertAssetEqual(packageData.Assets[0], restored.Assets[0]);

            Assert.NotNull(restored.BookOfLossesExtractData);
            Assert.Equal(2026, restored.BookOfLossesExtractData.Year);
            Assert.Equal(3, restored.BookOfLossesExtractData.Number);
            Assert.Equal(12, restored.BookOfLossesExtractData.PageNumber);
            Assert.Equal(new DateTime(2026, 03, 21), restored.BookOfLossesExtractData.RecordDate);
        }

        [Fact]
        public void WriteOffPackage_RoundTrip_PreservesMissingBookExtract()
        {
            var packageData = new WriteOffPackageReportData
            {
                Assets = new List<IAssetInfo>(),
                BookOfLossesExtractData = null,
                DestinationFolder = "C:\\Out"
            };
            var data = new WriteOffPackageTabData { PackageData = packageData };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.WriteOffPackage, data);
            var restored = Assert.IsType<WriteOffPackageTabData>(ReportSnapshotMapper.ToReportData(snapshot)).PackageData;

            Assert.Null(restored.BookOfLossesExtractData);
        }

        [Fact]
        public void WriteOffPackage_RoundTrip_PreservesTabFields()
        {
            var packageData = new WriteOffPackageReportData
            {
                Assets = new List<IAssetInfo> { CreateAsset() },
                DocumentDate = new DateTime(2026, 04, 01),
                EventDate = new DateTime(2026, 03, 15),
                OrdenNumber = 7,
                OrdenDate = new DateTime(2026, 03, 20),
                DestinationFolder = "C:\\Out"
            };
            var data = new WriteOffPackageTabData
            {
                PackageData = packageData,
                Reason = "Втрачено під час обстрілу",
                EventType = EventType.Lost,
                GenerateWriteOffPackage = false,
                GenerateWriteOffActs = false,
                WriteOffRegNumber = "РЕГ-1",
                WriteOffDocNumber = "ДОК-1",
                GenerateQualityStateReportInstead = true,
                QSRRegNumber = "РЕГ-2",
                QSRDocNumber = "ДОК-2"
            };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.WriteOffPackage, data);
            var restored = Assert.IsType<WriteOffPackageTabData>(ReportSnapshotMapper.ToReportData(snapshot));

            Assert.Equal(data.Reason, restored.Reason);
            Assert.Equal(data.EventType, restored.EventType);
            Assert.Equal(data.GenerateWriteOffPackage, restored.GenerateWriteOffPackage);
            Assert.Equal(data.GenerateWriteOffActs, restored.GenerateWriteOffActs);
            Assert.Equal(data.WriteOffRegNumber, restored.WriteOffRegNumber);
            Assert.Equal(data.WriteOffDocNumber, restored.WriteOffDocNumber);
            Assert.Equal(data.GenerateQualityStateReportInstead, restored.GenerateQualityStateReportInstead);
            Assert.Equal(data.QSRRegNumber, restored.QSRRegNumber);
            Assert.Equal(data.QSRDocNumber, restored.QSRDocNumber);
        }

        [Fact]
        public void WriteOffPackage_LegacySnapshotWithoutTabFields_RestoresViewModelDefaults()
        {
            var snapshot = new WriteOffPackageReportSnapshot
            {
                Assets = [],
                DocumentDate = new DateTime(2026, 04, 01),
                EventDate = new DateTime(2026, 03, 15),
                DestinationFolder = "C:\\Out"
            };

            var restored = WriteOffPackageSnapshotMapper.ToReportData(snapshot);

            Assert.Equal(string.Empty, restored.Reason);
            Assert.Equal(EventType.None, restored.EventType);
            Assert.True(restored.GenerateWriteOffPackage);
            Assert.True(restored.GenerateWriteOffActs);
            Assert.False(restored.GenerateQualityStateReportInstead);
            Assert.Equal(string.Empty, restored.WriteOffRegNumber);
            Assert.Equal(string.Empty, restored.WriteOffDocNumber);
            Assert.Equal(string.Empty, restored.QSRRegNumber);
            Assert.Equal(string.Empty, restored.QSRDocNumber);
        }

        [Fact]
        public void CommissioningAct_RoundTrip_PreservesData()
        {
            var data = new CommissioningActReportData
            {
                DocumentNumber = "А-1",
                DocumentDate = new DateTime(2026, 01, 05),
                Asset = new ProductDTO
                {
                    Name = "Генератор",
                    ShortName = "Ген.",
                    MeasurementUnit = "шт.",
                    NomenclatureCode = "NC-9",
                    Price = 800m,
                    StartDate = new DateTime(2024, 06, 06),
                    WarrantyPeriodMonths = 18,
                    YearManufactured = 2023,
                    ResourceYears = 10
                },
                AssetState = "прид.",
                AssetIds = new List<IProductIdentification>
                {
                    new ProductIdentification { SerialNumber = "SN-5", InventoryNumber = "INV-5" }
                },
                CountText = "один",
                Count = 1,
                CommissioningLocation = "склад",
                ShortCharacteristic = "дизельний",
                AssetCompliance = "відповідає",
                CompletionState = "не потрібна",
                TestResults = "успішно",
                OtherInfo = "немає",
                Conclusion = "ввести в експлуатацію",
                AttachedDocumentation = "паспорт",
                PersonAccepted = CreatePerson("Олег", "Шевченко"),
                PersonHanded = CreatePerson(),
                DestinationFolder = "C:\\Out"
            };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.CommissioningAct, data);
            var restored = Assert.IsType<CommissioningActReportData>(ReportSnapshotMapper.ToReportData(snapshot));

            Assert.Equal(data.DocumentNumber, restored.DocumentNumber);
            Assert.Equal(data.DocumentDate, restored.DocumentDate);
            Assert.Equal(data.Asset.Name, restored.Asset.Name);
            Assert.Equal(data.Asset.Price, restored.Asset.Price);
            Assert.Equal(data.Asset.NomenclatureCode, restored.Asset.NomenclatureCode);
            Assert.Equal(data.AssetState, restored.AssetState);
            Assert.Single(restored.AssetIds);
            Assert.Equal("SN-5", restored.AssetIds[0].SerialNumber);
            Assert.Equal("INV-5", restored.AssetIds[0].InventoryNumber);
            Assert.Equal(data.CountText, restored.CountText);
            Assert.Equal(data.Count, restored.Count);
            Assert.Equal(data.CommissioningLocation, restored.CommissioningLocation);
            Assert.Equal(data.ShortCharacteristic, restored.ShortCharacteristic);
            Assert.Equal(data.AssetCompliance, restored.AssetCompliance);
            Assert.Equal(data.CompletionState, restored.CompletionState);
            Assert.Equal(data.TestResults, restored.TestResults);
            Assert.Equal(data.OtherInfo, restored.OtherInfo);
            Assert.Equal(data.Conclusion, restored.Conclusion);
            Assert.Equal(data.AttachedDocumentation, restored.AttachedDocumentation);
            AssertPersonEqual(data.PersonAccepted, restored.PersonAccepted);
            AssertPersonEqual(data.PersonHanded, restored.PersonHanded);
        }

        [Fact]
        public void Valuation_RoundTrip_PreservesComponents()
        {
            var data = new AssetValuationReportData
            {
                ValuationData = new List<IAssetValuationData?>
                {
                    new AssetValuationData
                    {
                        Name = "Ноутбук",
                        SerialNumber = "SN-10",
                        ShortName = "НБ",
                        NomenclatureCode = "NC-10",
                        Price = 1200m,
                        Description = "вживаний",
                        ValuationDate = new DateTime(2025, 11, 11),
                        AssetComponents = new List<AssetComponent>
                        {
                            new()
                            {
                                Name = "Акумулятор",
                                Unit = "шт.",
                                NomenclatureCode = "NC-11",
                                Quantity = 2,
                                Category = 3,
                                Price = 50m,
                                Exclude = true
                            }
                        }
                    },
                    null
                },
                DestinationFolder = "C:\\Out"
            };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.AssetValuationReport, data);
            var restored = Assert.IsType<AssetValuationReportData>(ReportSnapshotMapper.ToReportData(snapshot));

            var valuation = Assert.Single(restored.ValuationData);
            Assert.NotNull(valuation);
            Assert.Equal("Ноутбук", valuation.Name);
            Assert.Equal("SN-10", valuation.SerialNumber);
            Assert.Equal("НБ", valuation.ShortName);
            Assert.Equal("NC-10", valuation.NomenclatureCode);
            Assert.Equal(1200m, valuation.Price);
            Assert.Equal("вживаний", valuation.Description);
            Assert.Equal(new DateTime(2025, 11, 11), valuation.ValuationDate);

            var component = Assert.Single(valuation.AssetComponents);
            Assert.Equal("Акумулятор", component.Name);
            Assert.Equal("шт.", component.Unit);
            Assert.Equal("NC-11", component.NomenclatureCode);
            Assert.Equal(2, component.Quantity);
            Assert.Equal(3, component.Category);
            Assert.Equal(50m, component.Price);
            Assert.True(component.Exclude);
        }

        [Fact]
        public void Dismantling_RoundTrip_PreservesData()
        {
            var data = new DismantlingReportData
            {
                Dismantlings = new List<AssetDismantlingData>
                {
                    new()
                    {
                        Name = "Сервер",
                        SerialNumber = "SN-20,SN-21",
                        ShortName = "Срв",
                        NomenclatureCode = "NC-20",
                        Price = 3000m,
                        Description = "розкомплектація",
                        ValuationDate = new DateTime(2025, 10, 10),
                        RegistrationNumber = "REG-1",
                        DocumentNumber = "DOC-1",
                        Reason = "пошкодження",
                        Category = 4,
                        MeasurementUnit = "к-т",
                        AssetComponents = new List<AssetComponent>
                        {
                            new() { Name = "Диск", Price = 100m, Quantity = 4 }
                        }
                    }
                },
                DestinationFolder = "C:\\Out"
            };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.AssetDismantlingReport, data);
            var restored = Assert.IsType<DismantlingReportData>(ReportSnapshotMapper.ToReportData(snapshot));

            var dismantling = Assert.Single(restored.Dismantlings);
            Assert.Equal("Сервер", dismantling.Name);
            Assert.Equal("SN-20,SN-21", dismantling.SerialNumber);
            Assert.Equal(2, dismantling.Count);
            Assert.Equal("NC-20", dismantling.NomenclatureCode);
            Assert.Equal(3000m, dismantling.Price);
            Assert.Equal("REG-1", dismantling.RegistrationNumber);
            Assert.Equal("DOC-1", dismantling.DocumentNumber);
            Assert.Equal("пошкодження", dismantling.Reason);
            Assert.Equal(4, dismantling.Category);
            Assert.Equal("к-т", dismantling.MeasurementUnit);

            var component = Assert.Single(dismantling.AssetComponents);
            Assert.Equal("Диск", component.Name);
            Assert.Equal(100m, component.Price);
            Assert.Equal(4, component.Quantity);
        }

        [Fact]
        public void Handover23_RoundTrip_PreservesData()
        {
            var data = new HandoverReportData
            {
                DocumentNumber = "П-23",
                DocumentDate = new DateTime(2026, 05, 05),
                DateStart = new DateTime(2026, 05, 01),
                DateEnd = null,
                Supplier = "в/ч А1111",
                Receiver = "в/ч А2222",
                ReasonDocumentName = "розпорядження",
                ReasonDocumentNumber = "Р-9",
                ReasonDocumentDate = new DateTime(2026, 04, 28),
                Reason = "передислокація",
                PersonResponsible = CreatePerson(),
                PersonReceiver = CreatePerson("Олег", "Шевченко"),
                Assets = new List<IAssetInfo> { CreateAsset() },
                DestinationFolder = "C:\\Out"
            };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.Handover23Act, data);
            var restored = Assert.IsType<HandoverReportData>(ReportSnapshotMapper.ToReportData(snapshot));

            Assert.Equal(data.DocumentNumber, restored.DocumentNumber);
            Assert.Equal(data.DocumentDate, restored.DocumentDate);
            Assert.Equal(data.DateStart, restored.DateStart);
            Assert.Null(restored.DateEnd);
            Assert.Equal(data.Supplier, restored.Supplier);
            Assert.Equal(data.Receiver, restored.Receiver);
            Assert.Equal(data.ReasonDocumentName, restored.ReasonDocumentName);
            Assert.Equal(data.ReasonDocumentNumber, restored.ReasonDocumentNumber);
            Assert.Equal(data.ReasonDocumentDate, restored.ReasonDocumentDate);
            Assert.Equal(data.Reason, restored.Reason);
            AssertPersonEqual(data.PersonResponsible, restored.PersonResponsible);
            AssertPersonEqual(data.PersonReceiver, restored.PersonReceiver);
            AssertAssetEqual(data.Assets[0], restored.Assets[0]);
        }

        [Fact]
        public void WriteOffOrder_RoundTrip_PreservesServicesAndWitnesses()
        {
            var data = new WriteOffOrderReportData
            {
                ReportNum = "12",
                ReportDate = new DateTime(2026, 06, 01),
                EventDate = new DateTime(2026, 05, 20),
                EventTime = "14:30",
                BattleOrder = "БР-3",
                BattleOrderDate = new DateTime(2026, 05, 19),
                BattleOrderLocation = "м. Дружківка",
                SubdivisionName = "1 рота",
                ReporterRank = "сержант",
                ReporterName = "Іван КОВАЛЬ",
                CreatorPosition = "начальник служби",
                CreatorRank = "лейтенант",
                CreatorName = "Петро ГРИЩЕНКО",
                MilUnitApproval = "А1488",
                WhatHappened = "артилерійський обстріл",
                Services = new List<WriteOffServiceData>
                {
                    new()
                    {
                        ServiceName = "служба зв'язку",
                        ServiceNameGenitive = "служби зв'язку",
                        Assets = new List<WriteOffServiceAssetData>
                        {
                            new() { Name = "Радіостанція", Count = 2, MeasurementUnit = "шт.", Amount = 3001m }
                        }
                    }
                },
                Witnesses = new List<PersonDTO>
                {
                    new() { Rank = "солдат", FirstName = "Олег", LastName = "ШЕВЧЕНКО", Position = "стрілець" }
                },
                DestinationFolder = "C:\\Out"
            };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.WriteOffOrder, data);
            var restored = Assert.IsType<WriteOffOrderReportData>(ReportSnapshotMapper.ToReportData(snapshot));

            Assert.Equal(data.ReportNum, restored.ReportNum);
            Assert.Equal(data.ReportDate, restored.ReportDate);
            Assert.Equal(data.EventDate, restored.EventDate);
            Assert.Equal(data.EventTime, restored.EventTime);
            Assert.Equal(data.BattleOrder, restored.BattleOrder);
            Assert.Equal(data.BattleOrderDate, restored.BattleOrderDate);
            Assert.Equal(data.BattleOrderLocation, restored.BattleOrderLocation);
            Assert.Equal(data.SubdivisionName, restored.SubdivisionName);
            Assert.Equal(data.ReporterRank, restored.ReporterRank);
            Assert.Equal(data.ReporterName, restored.ReporterName);
            Assert.Equal(data.CreatorPosition, restored.CreatorPosition);
            Assert.Equal(data.CreatorRank, restored.CreatorRank);
            Assert.Equal(data.CreatorName, restored.CreatorName);
            Assert.Equal(data.MilUnitApproval, restored.MilUnitApproval);
            Assert.Equal(data.WhatHappened, restored.WhatHappened);

            var service = Assert.Single(restored.Services);
            Assert.Equal("служба зв'язку", service.ServiceName);
            Assert.Equal("служби зв'язку", service.ServiceNameGenitive);
            var serviceAsset = Assert.Single(service.Assets);
            Assert.Equal("Радіостанція", serviceAsset.Name);
            Assert.Equal(2, serviceAsset.Count);
            Assert.Equal("шт.", serviceAsset.MeasurementUnit);
            Assert.Equal(3001m, serviceAsset.Amount);

            var witness = Assert.Single(restored.Witnesses);
            Assert.Equal("солдат", witness.Rank);
            Assert.Equal("Олег", witness.FirstName);
            Assert.Equal("ШЕВЧЕНКО", witness.LastName);
            Assert.Equal("стрілець", witness.Position);
        }

        [Fact]
        public void EAS_RoundTrip_PreservesServicesWitnessesAndAssets()
        {
            var data = new EASReportData
            {
                ReportNum = "17",
                ReportDate = new DateTime(2026, 06, 01),
                EventDate = new DateTime(2026, 05, 20),
                EventTime = "14:30",
                BattleOrder = "БР-3",
                BattleOrderDate = new DateTime(2026, 05, 19),
                SubdivisionName = "1 рота",
                ReporterRank = "сержант",
                ReporterName = "Іван КОВАЛЬ",
                WhatHappened = "артилерійський обстріл",
                OrdenNum = "5",
                OrdenDate = new DateTime(2026, 05, 30),
                Services = new List<EASServiceData>
                {
                    new()
                    {
                        ServiceName = "служба зв'язку",
                        ServiceNameGenitive = "служби зв'язку",
                        HeadRank = "старший лейтенант",
                        HeadName = "Олександр Шупер",
                        HeadPosition = "Начальник групи зв'язку",
                        Assets = new List<EASAssetData>
                        {
                            new() { Name = "Радіостанція", Code = "NC-1", MeasurementUnit = "шт.", Category = 2, Count = 2, OriginalPrice = 4000m, ResidualPrice = 3001m }
                        }
                    }
                },
                Witnesses = new List<PersonDTO>
                {
                    new() { Rank = "солдат", LastName = "Шевченко", FirstName = "Олег", Patronymic = "Васильович", Position = "стрілець" }
                },
                DestinationFolder = "C:\\Out"
            };

            var snapshot = ReportSnapshotMapper.ToSnapshot(ReportType.EAS, data);
            var restored = Assert.IsType<EASReportData>(ReportSnapshotMapper.ToReportData(snapshot));

            Assert.Equal(data.ReportNum, restored.ReportNum);
            Assert.Equal(data.ReportDate, restored.ReportDate);
            Assert.Equal(data.EventDate, restored.EventDate);
            Assert.Equal(data.EventTime, restored.EventTime);
            Assert.Equal(data.BattleOrder, restored.BattleOrder);
            Assert.Equal(data.BattleOrderDate, restored.BattleOrderDate);
            Assert.Equal(data.SubdivisionName, restored.SubdivisionName);
            Assert.Equal(data.ReporterRank, restored.ReporterRank);
            Assert.Equal(data.ReporterName, restored.ReporterName);
            Assert.Equal(data.WhatHappened, restored.WhatHappened);
            Assert.Equal(data.OrdenNum, restored.OrdenNum);
            Assert.Equal(data.OrdenDate, restored.OrdenDate);

            var service = Assert.Single(restored.Services);
            Assert.Equal("служба зв'язку", service.ServiceName);
            Assert.Equal("служби зв'язку", service.ServiceNameGenitive);
            Assert.Equal("старший лейтенант", service.HeadRank);
            Assert.Equal("Олександр Шупер", service.HeadName);
            Assert.Equal("Начальник групи зв'язку", service.HeadPosition);

            var serviceAsset = Assert.Single(service.Assets);
            Assert.Equal("Радіостанція", serviceAsset.Name);
            Assert.Equal("NC-1", serviceAsset.Code);
            Assert.Equal("шт.", serviceAsset.MeasurementUnit);
            Assert.Equal(2, serviceAsset.Category);
            Assert.Equal(2, serviceAsset.Count);
            Assert.Equal(4000m, serviceAsset.OriginalPrice);
            Assert.Equal(3001m, serviceAsset.ResidualPrice);

            var witness = Assert.Single(restored.Witnesses);
            Assert.Equal("солдат", witness.Rank);
            Assert.Equal("Шевченко", witness.LastName);
            Assert.Equal("Олег", witness.FirstName);
            Assert.Equal("Васильович", witness.Patronymic);
            Assert.Equal("стрілець", witness.Position);
        }

        [Fact]
        public void ToSnapshot_UnsupportedData_Throws()
        {
            var data = new CommonWriteOffReportData();

            Assert.Throws<NotSupportedException>(() => ReportSnapshotMapper.ToSnapshot(ReportType.QualityStateReport, data));
        }
    }
}
