using System.Collections.ObjectModel;
using System.Windows.Input;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.WriteOff.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.ViewModels.Tabs;

namespace Mil.Paperwork.WriteOff.ViewModels.Dictionaries
{
    internal class MeasurementUnitsDictionaryViewModel : ISettingsTabViewModel
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<MeasurementUnitViewModel> Units { get; }
        public string Header => "Довідник одиниць виміру";
        public bool IsClosed { get; private set; }

        public ObservableCollection<NounGender> Genders { get;}
        
        public event EventHandler<ITabViewModel> TabCloseRequested;

        public ICommand AddItemCommand { get; }
        public ICommand<MeasurementUnitViewModel> RemoveItemCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CloseCommand { get; }

        public MeasurementUnitsDictionaryViewModel(IDataService dataService, IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;

            Units = [.. GetUnitsData()];
            Genders = [.. EnumHelper.GetValues<NounGender>()];

            AddItemCommand = new DelegateCommand(AddItemCommandExecute);
            RemoveItemCommand = new DelegateCommand<MeasurementUnitViewModel>(RemoveItemCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
        }

        private MeasurementUnitViewModel[] GetUnitsData()
        {
            var units = _dataService.LoadMeasurementUnitsData();
            var unitViewModels = units.Select(x => new MeasurementUnitViewModel(x));
            return unitViewModels.ToArray();
        }

        private void AddItemCommandExecute() => Units.Add(new MeasurementUnitViewModel());

        private void RemoveItemCommandExecute(MeasurementUnitViewModel unit)
        {
            if (unit != null && Units.Contains(unit))
            {
                Units.Remove(unit);
            }
        }

        private void SaveCommandExecute()
        {
            var dtos = Units.Select(vm => vm.ToDTO()).ToArray();
            _dataService.SaveMeasurementUnitsData(dtos);
        }

        private void RefreshCommandExecute()
        {
            var units = GetUnitsData();
            Units.Clear();
            foreach (var unit in units)
            {
                Units.Add(unit);
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