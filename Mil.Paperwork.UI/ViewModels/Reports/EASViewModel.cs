using Mil.MVVM.Common;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels.Configuration;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Helpers;
using Mil.Paperwork.UI.Managers;
using Mil.Paperwork.UI.ViewModels.Dictionaries;
using Mil.Paperwork.UI.ViewModels.Ribbon;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Reports
{
    internal class EASViewModel : BaseReportTabViewModel, IReportDataLoadable<IEASReportData>
    {
        private const string HeaderText = "Єдиний акт списання";

        private const string GroupTitlePersonnel = "Виконавці";
        private const string CaptionAddService = "Додати службу";
        private const string CaptionAddWitness = "Додати свідка";
        private const string CaptionRemoveWitness = "Видалити свідка";
        private const string AutomationIdAddService = "EAS_AddServiceAction";
        private const string AutomationIdAddWitness = "EAS_AddWitnessAction";
        private const string AutomationIdRemoveWitness = "EAS_RemoveWitnessAction";

        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly IDialogService _dialogService;

        private string _reportNum = string.Empty;
        private DateTime _reportDate = DateTime.Today;
        private DateTime _eventDate = DateTime.Today;
        private string _eventTime = string.Empty;
        private string _battleOrder = string.Empty;
        private DateTime _battleOrderDate = DateTime.Today;
        private string _subdivisionName = string.Empty;
        private string _reporterRank = string.Empty;
        private string _reporterName = string.Empty;
        private string _whatHappened = string.Empty;
        private string _ordenNum = string.Empty;
        private DateTime _ordenDate = DateTime.Today;

        private PersonViewModel? _selectedWitness;

        public override string Header => HeaderText;

        public ObservableCollection<MilitaryServiceViewModel> AvailableServices { get; }
        public ObservableCollection<AssetType> AssetTypes { get; }
        public IList<MeasurementUnitViewModel> AvailableMeasurementUnits { get; }
        public ObservableCollection<EASServiceViewModel> Services { get; }
        public ObservableCollection<PersonViewModel> Witnesses { get; }

        public string ReportNum { get => _reportNum; set => SetProperty(ref _reportNum, value); }
        public DateTime ReportDate
        {
            get => _reportDate;
            set
            {
                if (SetProperty(ref _reportDate, value))
                {
                    OnPropertyChanged(nameof(MaxDate));
                    if (BattleOrderDate > _reportDate)
                    {
                        BattleOrderDate = _reportDate;
                    }
                    if (EventDate > _reportDate)
                    {
                        EventDate = _reportDate;
                    }
                }
            }
        }

        public DateTime? MaxDate => _reportDate;

        public DateTime EventDate
        {
            get => _eventDate;
            set
            {
                var clamped = value > _reportDate ? _reportDate : value;
                SetProperty(ref _eventDate, clamped);
            }
        }

        public string EventTime { get => _eventTime; set => SetProperty(ref _eventTime, value); }
        public string BattleOrder { get => _battleOrder; set => SetProperty(ref _battleOrder, value); }

        public DateTime BattleOrderDate
        {
            get => _battleOrderDate;
            set
            {
                var clamped = value > _reportDate ? _reportDate : value;
                SetProperty(ref _battleOrderDate, clamped);
            }
        }

        public string SubdivisionName { get => _subdivisionName; set => SetProperty(ref _subdivisionName, value); }
        public string ReporterRank { get => _reporterRank; set => SetProperty(ref _reporterRank, value); }
        public string ReporterName { get => _reporterName; set => SetProperty(ref _reporterName, value); }
        public string WhatHappened { get => _whatHappened; set => SetProperty(ref _whatHappened, value); }
        public string OrdenNum { get => _ordenNum; set => SetProperty(ref _ordenNum, value); }
        public DateTime OrdenDate { get => _ordenDate; set => SetProperty(ref _ordenDate, value); }

        public PersonViewModel? SelectedWitness
        {
            get => _selectedWitness;
            set => SetProperty(ref _selectedWitness, value);
        }

        public IDelegateCommand AddServiceCommand { get; }
        public IDelegateCommand AddWitnessCommand { get; }
        public IDelegateCommand RemoveWitnessCommand { get; }
        public IDelegateCommand GenerateReportCommand { get; }

        protected override ReportType HistoryReportType => ReportType.EAS;

        protected override string AutomationIdPrefix => "EAS";

        public EASViewModel(
            ReportManager reportManager,
            IDataService dataService,
            IReportDataService reportDataService,
            IReportHistoryService reportHistoryService,
            IDialogService dialogService)
            : base(reportHistoryService, dialogService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
            _reportDataService = reportDataService;
            _dialogService = dialogService;

            AvailableServices = [];
            AssetTypes = [.. EnumHelper.GetValues<AssetType>()];
            AvailableMeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(u => new MeasurementUnitViewModel(u))];
            Services = [];
            Witnesses = [];

            ReloadServices();

            AddServiceCommand = new DelegateCommand(AddService);
            AddWitnessCommand = new DelegateCommand(AddWitness);
            RemoveWitnessCommand = new DelegateCommand(RemoveSelectedWitness);
            GenerateReportCommand = new DelegateCommand(GenerateReportCommandExecute);

            ResumeDirtyTracking();
        }

        protected override IList<RibbonGroupViewModel> BuildRibbonGroups()
        {
            var documentGroup = CreateDocumentGroup(GenerateReportCommand);
            var personnelActions = new List<RibbonActionViewModel>
            {
                new RibbonActionViewModel(CaptionAddService, IconKeys.AddRow, AddServiceCommand, AutomationIdAddService),
                new RibbonActionViewModel(CaptionAddWitness, IconKeys.AddRow, AddWitnessCommand, AutomationIdAddWitness),
                new RibbonActionViewModel(CaptionRemoveWitness, IconKeys.Remove, RemoveWitnessCommand, AutomationIdRemoveWitness, isDestructive: true)
            };
            var personnelGroup = new RibbonGroupViewModel(GroupTitlePersonnel, personnelActions);
            var groups = new List<RibbonGroupViewModel> { documentGroup, personnelGroup };
            return groups;
        }

        private void ReloadServices()
        {
            var services = _reportDataService.GetAllServices();
            var vms = services
                .Select(x => new MilitaryServiceViewModel(x.Key, x.Value))
                .OrderBy(vm => vm.NominativeName)
                .ToList();

            AvailableServices.Clear();

            foreach (var vm in vms)
            {
                AvailableServices.Add(vm);
            }
        }

        public MilitaryServiceViewModel? AddServiceToDictionary(
            string nominativeName, string genitiveName, AssetType assetType,
            string headRank, string headName, string headPosition)
        {
            var key = Guid.NewGuid().ToString();
            var dto = new MilitaryServiceDTO();
            dto.ServiceNameFull.Value = nominativeName;
            dto.ServiceNameGenitive.Value = genitiveName;
            dto.AssetTypes = [assetType.ToString()];
            dto.HeadOfServiceRank.Value = headRank;
            dto.HeadOfServiceName.Value = headName;
            dto.HeadOfServicePosition.Value = headPosition;

            _reportDataService.SaveServiceData(key, dto);
            ReloadServices();

            var newVm = AvailableServices.FirstOrDefault(vm => vm.ServiceKey == key);
            return newVm;
        }

        public void SaveServiceHead(MilitaryServiceViewModel service)
        {
            _reportDataService.SaveServiceData(service.ServiceKey, service.GetDTO());
            ReloadServices();
        }

        private void AddService()
        {
            var service = new EASServiceViewModel(
                AvailableServices,
                AssetTypes,
                AvailableMeasurementUnits,
                RemoveService,
                AddServiceToDictionary,
                SaveServiceHead);
            Services.Add(service);
        }

        private void RemoveService(EASServiceViewModel service)
        {
            Services.Remove(service);
        }

        private void AddWitness()
        {
            Witnesses.Add(new PersonViewModel());
        }

        private void RemoveSelectedWitness()
        {
            if (SelectedWitness != null)
            {
                Witnesses.Remove(SelectedWitness);
            }
        }

        private static bool IsValidEventTime(string value)
        {
            if (value is null || value.Length != 5 || value[2] != ':')
            {
                return false;
            }
            var result = int.TryParse(value[..2], out int h) && int.TryParse(value[3..], out int m)
                && h is >= 0 and <= 23 && m is >= 0 and <= 59;
            return result;
        }

        private async void GenerateReportCommandExecute()
        {
            if (!IsValidEventTime(EventTime))
            {
                await _dialogService.ShowMessageAsync("Невірний формат часу. Введіть час у форматі гг:хх (наприклад, 14:30).", "Помилка валідації", icon: DialogIcon.Warning);
                return;
            }

            if (!_dialogService.TryPickFolder(out var folderName))
            {
                return;
            }

            var reportData = (EASReportData)BuildReportData();
            reportData.DestinationFolder = folderName;

            _reportManager.GenerateEAS(reportData, EnsureHistoryEntryId());

            ResetDirtyState();
        }

        protected override IReportData BuildReportData()
        {
            var reportData = new EASReportData
            {
                ReportNum = ReportNum,
                ReportDate = ReportDate,
                EventDate = EventDate,
                EventTime = EventTime,
                BattleOrder = BattleOrder,
                BattleOrderDate = BattleOrderDate,
                SubdivisionName = SubdivisionName,
                ReporterRank = ReporterRank,
                ReporterName = ReporterName,
                WhatHappened = WhatHappened,
                OrdenNum = OrdenNum,
                OrdenDate = OrdenDate,
                Services = [.. Services.Select(s => s.ToServiceData())],
                Witnesses = [.. Witnesses.Select(w => w.ToDTO())],
            };

            return reportData;
        }

        public void LoadReportData(IEASReportData data)
        {
            WithDirtyTrackingSuspended(() =>
            {
                ReportNum = data.ReportNum;
                ReportDate = data.ReportDate;
                EventDate = data.EventDate;
                EventTime = data.EventTime;
                BattleOrder = data.BattleOrder;
                BattleOrderDate = data.BattleOrderDate;
                SubdivisionName = data.SubdivisionName;
                ReporterRank = data.ReporterRank;
                ReporterName = data.ReporterName;
                WhatHappened = data.WhatHappened;
                OrdenNum = data.OrdenNum;
                OrdenDate = data.OrdenDate;

                Services.Clear();
                foreach (var serviceData in data.Services ?? [])
                {
                    var serviceViewModel = new EASServiceViewModel(
                        AvailableServices,
                        AssetTypes,
                        AvailableMeasurementUnits,
                        RemoveService,
                        AddServiceToDictionary,
                        SaveServiceHead);

                    serviceViewModel.LoadFrom(serviceData);
                    Services.Add(serviceViewModel);
                }

                Witnesses.Clear();
                foreach (var witnessData in data.Witnesses ?? [])
                {
                    Witnesses.Add(new PersonViewModel(witnessData));
                }
            });
        }
    }
}
