using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Common.MVVM;

namespace Mil.Paperwork.UI.ViewModels.Dictionaries
{
    public class PersonViewModel : ObservableItem
    {
        private string _firstName;
        private string _lastName;
        private string _patronymic;
        private string _position;
        private string _rank;

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string Patronymic
        {
            get => _patronymic;
            set => SetProperty(ref _patronymic, value);
        }

        public string FullName => $"{FirstName} {LastName}";

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
            FirstName = dto.FirstName;
            LastName = dto.LastName;
            Patronymic = dto.Patronymic;
            Position = dto.Position;
            Rank = dto.Rank;
        }

        public PersonDTO ToDTO() => new()
        {
            FirstName = FirstName,
            LastName = LastName,
            Patronymic = Patronymic,
            Position = Position,
            Rank = Rank
        };
    }
}
