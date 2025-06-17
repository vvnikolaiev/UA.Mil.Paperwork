using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class MeasurementUnitDTO
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public NounGender Gender { get; set; }

        public MeasurementUnitDTO(string name, string shortName, NounGender gender) : this()
        {
            Name = name;
            ShortName = shortName;
            Gender = gender;
        }
        public MeasurementUnitDTO() { }
    }
}