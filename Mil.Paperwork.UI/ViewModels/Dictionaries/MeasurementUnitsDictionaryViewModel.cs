using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Mil.Paperwork.UI.ViewModels.Dictionaries
{
    internal class MeasurementUnitsDictionaryViewModel : BaseTabViewModel, ISettingsTabViewModel
    {
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<MeasurementUnitViewModel> Units { get; }
        public override string Header => "Довідник одиниць виміру";
        public bool IsClosed { get; private set; }

        public ObservableCollection<NounGender> Genders { get;}
        
        public IDelegateCommand AddItemCommand { get; }
        public IDelegateCommand<MeasurementUnitViewModel> RemoveItemCommand { get; }
        public IDelegateCommand SaveCommand { get; }
        public IDelegateCommand RefreshCommand { get; }

        public MeasurementUnitsDictionaryViewModel(IDataService dataService, IDialogService dialogService) 
            : base(dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;

            Units = [.. GetUnitsData()];
            Genders = [.. EnumHelper.GetValues<NounGender>()];

            AddItemCommand = new DelegateCommand(AddItemCommandExecute);
            RemoveItemCommand = new DelegateCommand<MeasurementUnitViewModel>(RemoveItemCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
        }

        private MeasurementUnitViewModel[] GetUnitsData()
        {
            var units = _dataService.LoadMeasurementUnitsData();
            var unitViewModels = units.Select(x => new MeasurementUnitViewModel(x));
            var result = unitViewModels.ToArray();
            return result;
        }

        private void AddItemCommandExecute()
        {
            Units.Add(new MeasurementUnitViewModel());
        }

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
    }
}