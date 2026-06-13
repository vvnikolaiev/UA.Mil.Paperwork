using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.ViewModels.Ribbon;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Dictionaries
{
    internal class MeasurementUnitsDictionaryViewModel : BaseTabViewModel, ISettingsTabViewModel, ISilentRefreshable
    {
        private const string GroupRecords = "Записи";
        private const string GroupData = "Дані";

        private const string CaptionAdd = "Додати";
        private const string CaptionRemove = "Видалити";
        private const string CaptionSave = "Зберегти";
        private const string CaptionRefresh = "Оновити";

        private const string IconKeyAdd = "IconAdd";
        private const string IconKeyRemove = "IconDelete";
        private const string IconKeySave = "IconSaveDraft";
        private const string IconKeyRefresh = "IconRefresh";

        private readonly IDataService _dataService;

        private MeasurementUnitViewModel _selectedUnit;

        public ObservableCollection<MeasurementUnitViewModel> Units { get; }
        public override string Header => "Довідник одиниць виміру";
        public bool IsClosed { get; private set; }

        public ObservableCollection<NounGender> Genders { get; }

        public MeasurementUnitViewModel SelectedUnit
        {
            get => _selectedUnit;
            set => SetProperty(ref _selectedUnit, value);
        }

        public IDelegateCommand AddItemCommand { get; }
        public IDelegateCommand RemoveItemCommand { get; }
        public IDelegateCommand SaveCommand { get; }
        public IDelegateCommand RefreshCommand { get; }

        public MeasurementUnitsDictionaryViewModel(IDataService dataService, IDialogService dialogService)
            : base(dialogService)
        {
            _dataService = dataService;

            Units = [.. GetUnitsData()];
            Genders = [.. EnumHelper.GetValues<NounGender>()];

            AddItemCommand = new DelegateCommand(AddItemCommandExecute);
            RemoveItemCommand = new DelegateCommand(RemoveItemCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
        }

        protected override IList<RibbonGroupViewModel> BuildRibbonGroups()
        {
            var groups = new List<RibbonGroupViewModel>
            {
                new RibbonGroupViewModel(GroupRecords, new[]
                {
                    new RibbonActionViewModel(CaptionAdd, IconKeyAdd, AddItemCommand),
                    new RibbonActionViewModel(CaptionRemove, IconKeyRemove, RemoveItemCommand, isDestructive: true)
                }),
                new RibbonGroupViewModel(GroupData, new[]
                {
                    new RibbonActionViewModel(CaptionSave, IconKeySave, SaveCommand),
                    new RibbonActionViewModel(CaptionRefresh, IconKeyRefresh, RefreshCommand)
                })
            };
            return groups;
        }

        public void SilentRefresh()
        {
            RefreshCommandExecute();
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

        private void RemoveItemCommandExecute()
        {
            if (SelectedUnit != null && Units.Contains(SelectedUnit))
            {
                Units.Remove(SelectedUnit);
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