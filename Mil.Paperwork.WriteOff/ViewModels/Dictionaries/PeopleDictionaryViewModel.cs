using System.Collections.ObjectModel;
using System.Windows.Input;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.ViewModels.Tabs;
using Mil.Paperwork.WriteOff.Views;
using Mil.Paperwork.WriteOff.Configuration;
using Mil.Paperwork.Common.Enums;

namespace Mil.Paperwork.WriteOff.ViewModels.Dictionaries
{
    internal class PeopleDictionaryViewModel : ISettingsTabViewModel
    {
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<PersonViewModel> People { get; }
        public string Header => "Довідник осіб";
        public bool IsClosed { get; private set; }

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public ICommand AddItemCommand { get; }
        public ICommand<PersonViewModel> RemoveItemCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand CloseCommand { get; }

        public PeopleDictionaryViewModel(
            IDataService dataService, 
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _dataService = dataService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            People = [.. GetPeopleData()];

            AddItemCommand = new DelegateCommand(AddItemCommandExecute);
            RemoveItemCommand = new DelegateCommand<PersonViewModel>(RemoveItemCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
            ImportCommand = new DelegateCommand(ImportCommandExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
        }


        private PersonViewModel[] GetPeopleData()
        {
            var people = _dataService.LoadPeopleData();
            var productViewModels = people.Select(x => new PersonViewModel(x));
            return productViewModels.ToArray();
        }

        private void AddItemCommandExecute()
        {
            var person = new PersonViewModel();
            People.Add(person);
        }

        private void RemoveItemCommandExecute(PersonViewModel person)
        {
            if (person != null && People.Contains(person))
            {
                People.Remove(person);
            }
        }

        private void SaveCommandExecute()
        {
            var dtos = People.Select(vm => vm.ToDTO()).ToArray();
            _dataService.SavePeopleData(dtos);
        }

        private void RefreshCommandExecute()
        {
            var people = GetPeopleData();
            People.Clear();
            foreach (var person in people)
            {
                People.Add(person);
            }
        }

        private void ImportCommandExecute()
        {
            var importViewModel = _navigationService.GetViewModel<ImportViewModel>();
            importViewModel.SetImportType(ImportType.People);

            _navigationService.OpenWindow<ImportDialogWindow, ImportViewModel>(importViewModel);

            if (importViewModel.IsValid)
            {
                RefreshCommandExecute();
            }
        }

        private void CloseCommandExecute()
        {
            if (_dialogService.ShowMessage("Close tab?", "Confirmation", DialogButtons.YesNo) == DialogResult.Yes)
            {
                TabCloseRequested?.Invoke(this, this);
                IsClosed = true;
            }
        }
    }
}