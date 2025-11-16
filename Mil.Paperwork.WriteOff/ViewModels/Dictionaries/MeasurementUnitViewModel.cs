using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.WriteOff.MVVM;

namespace Mil.Paperwork.WriteOff.ViewModels.Dictionaries
{
    public class MeasurementUnitViewModel : ObservableItem
    {
        private string _name;
        private string _shortName;
        private NounGender _gender = NounGender.Masculine;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string ShortName
        {
            get => _shortName;
            set => SetProperty(ref _shortName, value);
        }

        public NounGender Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        public MeasurementUnitViewModel() { }

        public MeasurementUnitViewModel(MeasurementUnitDTO dto)
        {
            Name = dto.Name;
            ShortName = dto.ShortName;
            Gender = dto.Gender;
        }

        public MeasurementUnitDTO ToDTO() => new()
        {
            Name = Name,
            ShortName = ShortName,
            Gender = Gender
        };
    }
}
