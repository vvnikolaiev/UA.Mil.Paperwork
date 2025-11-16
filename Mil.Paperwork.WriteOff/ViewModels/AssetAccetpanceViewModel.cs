using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.WriteOff.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.ViewModels.Dictionaries;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class AssetAccetpanceViewModel : ObservableItem
    {
        private PersonViewModel _selectedPersonAccepted;
        private PersonViewModel _selectedPersonHanded;
        private string _personAcceptedName;
        private string _personAcceptedRank;
        private string _personAcceptedPosition;
        private string _personHandedName;
        private string _personHandedRank;
        private string _personHandedPosition;

        public PersonViewModel SelectedPersonAccepted
        {
            get => _selectedPersonAccepted;
            set => SetProperty(ref _selectedPersonAccepted, value);
        }

        public PersonViewModel SelectedPersonHanded
        {
            get => _selectedPersonHanded;
            set => SetProperty(ref _selectedPersonHanded, value);
        }

        public string PersonAcceptedName
        {
            get => _personAcceptedName;
            set => SetProperty(ref _personAcceptedName, value);
        }

        public string PersonAcceptedRank
        {
            get => _personAcceptedRank;
            set => SetProperty(ref _personAcceptedRank, value);
        }

        public string PersonAcceptedPosition
        {
            get => _personAcceptedPosition;
            set => SetProperty(ref _personAcceptedPosition, value);
        }

        public string PersonHandedName
        {
            get => _personHandedName;
            set => SetProperty(ref _personHandedName, value);
        }

        public string PersonHandedRank
        {
            get => _personHandedRank;
            set => SetProperty(ref _personHandedRank, value);
        }

        public string PersonHandedPosition
        {
            get => _personHandedPosition;
            set => SetProperty(ref _personHandedPosition, value);
        }

        public ObservableCollection<PersonViewModel> People { get; }

        public ICommand PersonAccSelectedCommand { get; }
        public ICommand PersonHanSelectedCommand { get; }

        public AssetAccetpanceViewModel(IDataService dataService)
        {

            People = [.. dataService.LoadPeopleData().Select(x => new PersonViewModel(x))];

            PersonAccSelectedCommand = new DelegateCommand(PersonAccSelectedExecute);
            PersonHanSelectedCommand = new DelegateCommand(PersonHanSelectedExecute);
        }

        public PersonDTO GetReceiverDTO()
        {
            return new PersonDTO(_personAcceptedName, _personAcceptedPosition, _personAcceptedRank);
        }

        public PersonDTO GetTransmitterDTO()
        {
            return new PersonDTO(_personHandedName, _personHandedPosition, _personHandedRank);
        }

        public bool GetIsReceiverValid()
        {
            var isValid =
                !string.IsNullOrEmpty(PersonAcceptedName) &&
                !string.IsNullOrEmpty(PersonAcceptedPosition) &&
                !string.IsNullOrEmpty(PersonAcceptedRank);

            return isValid;
        }

        public bool GetIsTransmitterValid()
        {
            var isValid =
                !string.IsNullOrEmpty(PersonHandedName) &&
                !string.IsNullOrEmpty(PersonHandedPosition) &&
                !string.IsNullOrEmpty(PersonHandedRank);

            return isValid;
        }

        private void PersonAccSelectedExecute()
        {
            var person = SelectedPersonAccepted;

            if (person != null)
            {
                PersonAcceptedName = person.FullName;
                PersonAcceptedPosition = person.Position;
                PersonAcceptedRank = person.Rank;
            }
            else
            {
                OnPropertyChanged(nameof(PersonAcceptedName));
            }
        }

        private void PersonHanSelectedExecute()
        {
            var person = SelectedPersonHanded;

            if (person != null)
            {
                PersonHandedName = person.FullName;
                PersonHandedPosition = person.Position;
                PersonHandedRank = person.Rank;
            }
            else
            {
                OnPropertyChanged(nameof(PersonHandedName));
            }
        }
    }
}