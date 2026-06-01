using System.ComponentModel;

namespace Mil.Paperwork.Infrastructure.Enums
{
    public enum AssetType
    {
        [Description("Невизначений")]
        Default = 0,
        [Description("Зв'язок")]
        Connectivity,
        [Description("РХБЗ")]
        Radiochemical,
        [Description("Ракетне озброєння та засоби ППО")]
        MissileAirDefense,
        [Description("Ракети")]
        Missiles,
        [Description("Артилерійське озброєння")]
        Artillery,
        [Description("Стрілецька зброя")]
        SmallArms,
        [Description("Боєприпаси")]
        Ammunition,
        [Description("Бронетанкова техніка")]
        ArmoredVehicles,
        [Description("Засоби інженерного озброєння")]
        Engineering,
        [Description("Інженерні боєприпаси")]
        EngineeringAmmunition,
        [Description("Автомобільна техніка")]
        Automotive,
        [Description("Автомобільне майно")]
        AutomotiveProperty,
        [Description("Пально-мастильні матеріали")]
        Fuel,
        [Description("Засоби електрозабезпечення")]
        ElectricalEquipment,
        [Description("Топографо-геодезичні засоби")]
        Topographic,
        [Description("Авіаційна техніка")]
        Aviation,
        [Description("Повітрянодесантна техніка")]
        Airborne,
        [Description("Кораблі та плавзасоби")]
        Naval,
        [Description("Вимірювальна техніка")]
        MeasuringEquipment,
        [Description("Залізнично-технічне майно")]
        Railway,
        [Description("Аеродромне майно")]
        Aerodrome,
        [Description("Технічні засоби продовольчої служби")]
        FoodService,
        [Description("Техніка РЕБ")]
        ElectronicWarfare,
        [Description("Майно з нормою в роках")]
        StandardProperty,
        [Description("Техніка БпЛА")]
        UAV,
    }
}
