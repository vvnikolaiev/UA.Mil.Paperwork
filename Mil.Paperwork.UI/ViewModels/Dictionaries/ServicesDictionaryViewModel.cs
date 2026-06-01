using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Dictionaries
{
    internal class ServicesDictionaryViewModel : SettingsTabViewModel
    {
        private readonly IDataService _dataService;

        public ObservableCollection<MilServiceEntryViewModel> Services { get; }
        public override string Header => "Довідник служб";

        public IDelegateCommand AddItemCommand { get; }
        public IDelegateCommand<MilServiceEntryViewModel> RemoveItemCommand { get; }
        public IDelegateCommand SaveCommand { get; }
        public IDelegateCommand RefreshCommand { get; }

        public ServicesDictionaryViewModel(IDataService dataService, IDialogService dialogService) : base(dialogService)
        {
            _dataService = dataService;
            Services = [.. GetServicesData()];

            AddItemCommand = new DelegateCommand(AddItemCommandExecute);
            RemoveItemCommand = new DelegateCommand<MilServiceEntryViewModel>(RemoveItemCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
        }

        private MilServiceEntryViewModel[] GetServicesData()
        {
            return _dataService.LoadServicesData()
                .Select(s => new MilServiceEntryViewModel(s))
                .ToArray();
        }

        private void AddItemCommandExecute()
        {
            Services.Add(new MilServiceEntryViewModel());
        }

        private void RemoveItemCommandExecute(MilServiceEntryViewModel service)
        {
            if (service != null && Services.Contains(service))
                Services.Remove(service);
        }

        private void SaveCommandExecute()
        {
            var dtos = Services.Select(vm => vm.ToDTO()).ToList();
            _dataService.SaveServicesData(dtos);
        }

        private void RefreshCommandExecute()
        {
            var services = GetServicesData();
            Services.Clear();
            foreach (var s in services)
                Services.Add(s);
        }
    }
}
