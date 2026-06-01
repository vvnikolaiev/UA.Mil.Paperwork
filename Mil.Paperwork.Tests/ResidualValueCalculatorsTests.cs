using Mil.Paperwork.Domain.Calculators;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Tests.Calculators
{
    // Verifies column headers per КМУ Постанова 759-98-п, Додаток 3
    public class ResidualValueCalculatorsTests
    {
        private const string Ke  = "Ке";   // коефіцієнт експлуатації
        private const string Kr  = "Кр";   // коефіцієнт витрати ресурсу
        private const string Kzb = "Кзб";  // коефіцієнт зберігання (зв'язок)
        private const string Kts = "Ктс";  // коефіцієнт технічного стану
        private const string Kz  = "Кз";   // коефіцієнт умов зберігання
        private const string Kk  = "Кк";   // коефіцієнт категорійності
        private const string Kb  = "Кб";   // коефіцієнт безвідмовності
        private const string Kd  = "Кд";   // коефіцієнт довговічності
        private const string Kt  = "Кт";   // коефіцієнт строку зберігання
        private const string Kya = "Кя";   // коефіцієнт якості
        private const string Kh  = "Кх";   // коефіцієнт хімічного старіння
        private const string K   = "К";    // ПММ: строки зберігання
        private const string Kud = "Куд";  // ПММ: коефіцієнт важливості показників
        private const string Kfs = "Кфс";  // ПММ: коефіцієнт фактичного стану
        private const string Kcons = "КК"; // коефіцієнт консервації (кораблі)

        public static IEnumerable<object[]> AssetTypeColumnData =>
        [
            // п.22 — Техніка та майно зв'язку: Ке × Кр × Кзб × Ктс
            [AssetType.Connectivity,          new[] { Ke, Kr, Kzb, Kts }],
            // п.1  — Ракетне озброєння та ППО: Кз × Ке × Кр
            [AssetType.MissileAirDefense,     new[] { Kz, Ke, Kr }],
            // п.3  — Артилерійське озброєння: Кз × Ке × Кр
            [AssetType.Artillery,             new[] { Kz, Ke, Kr }],
            // п.4  — Стрілецька зброя: Кз × Ке × Кк
            [AssetType.SmallArms,             new[] { Kz, Ke, Kk }],
            // п.5  — Боєприпаси: Кз × Ке × Кк
            [AssetType.Ammunition,            new[] { Kz, Ke, Kk }],
            // п.6  — Бронетанкова техніка: Кб × Кд × Кз
            [AssetType.ArmoredVehicles,       new[] { Kb, Kd, Kz }],
            // п.7  — Засоби інженерного озброєння: Кб × Кд × Кз
            [AssetType.Engineering,           new[] { Kb, Kd, Kz }],
            // п.8  — Інженерні боєприпаси: Кз × Кт × Кя
            [AssetType.EngineeringAmmunition, new[] { Kz, Kt, Kya }],
            // п.10 — Автомобільне майно: Кб × Кд × Кз
            [AssetType.AutomotiveProperty,    new[] { Kb, Kd, Kz }],
            // п.11 — ПММ: К × Куд × Кфс
            [AssetType.Fuel,                  new[] { K, Kud, Kfs }],
            // п.14 — Топографо-геодезичні засоби: Ке × Кх × Ктс
            [AssetType.Topographic,           new[] { Ke, Kh, Kts }],
            // п.17 — Кораблі та судна: КК
            [AssetType.Naval,                 new[] { Kcons }],
            // п.18 — Вимірювальна техніка: Ке × Кх × Ктс
            [AssetType.MeasuringEquipment,    new[] { Ke, Kh, Kts }],
            // п.21 — Технічні засоби продовольчої служби: Ке × Кз × Ктс
            [AssetType.FoodService,           new[] { Ke, Kz, Kts }],
            // п.23 — Спеціальна техніка РЕБ: Кз × Ке × Кр
            [AssetType.ElectronicWarfare,     new[] { Kz, Ke, Kr }],
            // п.25 — Техніка та майно БпЛА: Ке × Кзб × Ктс
            [AssetType.UAV,                   new[] { Ke, Kzb, Kts }],
            // Без проміжних колонок — табличні або лінійна/специфічна формула
            // п.13 — РХБЗ: таблична
            [AssetType.Radiochemical,         Array.Empty<string>()],
            // п.2  — Ракети: таблична (гарантійний строк)
            [AssetType.Missiles,              Array.Empty<string>()],
            // п.9  — Автомобільна техніка: специфічна формула
            [AssetType.Automotive,            Array.Empty<string>()],
            // п.12 — Засоби електрозабезпечення: таблична
            [AssetType.ElectricalEquipment,   Array.Empty<string>()],
            // п.15 — Авіаційна техніка: специфічна формула
            [AssetType.Aviation,              Array.Empty<string>()],
            // п.16 — Повітрянодесантна техніка: лінійна (1 − Те/Тн)
            [AssetType.Airborne,              Array.Empty<string>()],
            // п.19 — Залізнично-технічне майно: специфічна формула
            [AssetType.Railway,               Array.Empty<string>()],
            // п.20 — Аеродромне майно: специфічна формула
            [AssetType.Aerodrome,             Array.Empty<string>()],
            // п.24 — Майно з нормою в роках: лінійна (1 − te/tn)
            [AssetType.StandardProperty,      Array.Empty<string>()],
        ];

        [Theory]
        [MemberData(nameof(AssetTypeColumnData))]
        public void GetColumnHeaders_ReturnsExpectedColumns_PerMethodology(AssetType assetType, string[] expectedColumns)
        {
            var calculator = ResidualPriceCalculatorFactory.CreateCalculator(assetType);

            var headers = calculator.GetColumnHeaders();

            Assert.Equal(expectedColumns, headers);
        }
    }
}
