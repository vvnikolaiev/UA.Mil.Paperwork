using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.MVVM;

namespace Mil.Paperwork.WriteOff.ViewModels.Dictionaries
{
    public class PersonViewModel : ObservableItem
    {
        private string _name;
        private string _position;
        private string _rank;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        public string Rank
        {
            get => _rank;
            set => SetProperty(ref _rank, value);
        }

        public PersonViewModel() { }

        public PersonViewModel(PersonDTO dto)
        {
            Name = dto.FullName;
            Position = dto.Position;
            Rank = dto.Rank;
        }

        public PersonDTO ToDTO() => new()
        {
            FullName = Name,
            Position = Position,
            Rank = Rank
        };
    }
}
