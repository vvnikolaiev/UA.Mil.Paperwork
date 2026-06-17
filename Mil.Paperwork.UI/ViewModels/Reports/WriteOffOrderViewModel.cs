using Mil.MVVM.Common;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
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
    internal class WriteOffOrderViewModel : BaseReportTabViewModel, IReportDataLoadable<IWriteOffOrderReportData>
    {
        private const string HeaderText = "Наказ про списання";

        private const string GroupTitlePersonnel = "Виконавці";
        private const string CaptionAddService = "Додати службу";
        private const string CaptionAddWitness = "Додати свідка";
        private const string CaptionRemoveWitness = "Видалити свідка";
        private const string AutomationIdAddService = "WriteOffOrder_AddServiceAction";
        private const string AutomationIdAddWitness = "WriteOffOrder_AddWitnessAction";
        private const string AutomationIdRemoveWitness = "WriteOffOrder_RemoveWitnessAction";

        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly IDialogService _dialogService;

        // Event fields
        private string _reportNum = string.Empty;
        private DateTime _reportDate = DateTime.Today;
        private DateTime _eventDate = DateTime.Today;
        private string _eventTime = string.Empty;
        private string _battleOrder = string.Empty;
        private DateTime _battleOrderDate = DateTime.Today;
        private string _battleOrderLocation = string.Empty;
        private string _whatHappened = string.Empty;
        private string _milUnitApproval = string.Empty;
        private string _subdivisionName = string.Empty;

        private WriteOffWitnessViewModel? _selectedWitness;

        // REPORTER_ — person who filed the incident report (in the "Подія" section)
        private string _reporterRank = string.Empty;
        private string _reporterName = string.Empty;

        // REPORT_CREATOR_ — Укладач, person who compiled the write-off order
        private PersonViewModel? _selectedCreator;
        private string _creatorPosition = string.Empty;
        private string _creatorRank = string.Empty;
        private string _creatorName = string.Empty;

        public override string Header => HeaderText;

        public ObservableCollection<MilitaryServiceViewModel> AvailableServices { get; }
        public ObservableCollection<AssetType> AssetTypes { get; }
        public IList<MeasurementUnitViewModel> AvailableMeasurementUnits { get; }
        public ObservableCollection<PersonViewModel> AvailablePeople { get; }
        public ObservableCollection<WriteOffServiceViewModel> Services { get; }
        public ObservableCollection<WriteOffWitnessViewModel> Witnesses { get; }

        public PersonViewModel? SelectedCreator
        {
            get => _selectedCreator;
            set
            {
                if (SetProperty(ref _selectedCreator, value) && value != null)
                {
                    CreatorName = value.FullName;
                    CreatorPosition = value.Position;
                    CreatorRank = value.Rank;
                }
            }
        }

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
        public string BattleOrderLocation { get => _battleOrderLocation; set => SetProperty(ref _battleOrderLocation, value); }
        public string WhatHappened { get => _whatHappened; set => SetProperty(ref _whatHappened, value); }
        public string MilUnitApproval { get => _milUnitApproval; set => SetProperty(ref _milUnitApproval, value); }
        public string SubdivisionName { get => _subdivisionName; set => SetProperty(ref _subdivisionName, value); }
        public string ReporterRank { get => _reporterRank; set => SetProperty(ref _reporterRank, value); }
        public string ReporterName { get => _reporterName; set => SetProperty(ref _reporterName, value); }
        public string CreatorPosition { get => _creatorPosition; set => SetProperty(ref _creatorPosition, value); }
        public string CreatorRank { get => _creatorRank; set => SetProperty(ref _creatorRank, value); }
        public string CreatorName { get => _creatorName; set => SetProperty(ref _creatorName, value); }

        public WriteOffWitnessViewModel? SelectedWitness
        {
            get => _selectedWitness;
            set => SetProperty(ref _selectedWitness, value);
        }

        public IDelegateCommand AddServiceCommand { get; }
        public IDelegateCommand AddWitnessCommand { get; }
        public IDelegateCommand RemoveWitnessCommand { get; }
        public IDelegateCommand GenerateReportCommand { get; }
        public IDelegateCommand OpenConfigurationCommand { get; }

        protected override ReportType HistoryReportType => ReportType.WriteOffOrder;

        protected override string AutomationIdPrefix => "WriteOffOrder";

        public WriteOffOrderViewModel(
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
            AvailablePeople = [.. _dataService.LoadPeopleData().Select(p => new PersonViewModel(p))];
            Services = [];
            Witnesses = [];

            ReloadServices();

            var commonParams = _reportDataService.GetReportParametersDictionary(ReportType.Common);
            MilUnitApproval = commonParams.GetValueOrDefault(WriteOffOrderHelper.MilUnitConfigKey, string.Empty);

            AddServiceCommand = new DelegateCommand(AddService);
            AddWitnessCommand = new DelegateCommand(AddWitness);
            RemoveWitnessCommand = new DelegateCommand(RemoveSelectedWitness);
            GenerateReportCommand = new DelegateCommand(GenerateReportCommandExecute);
            OpenConfigurationCommand = new DelegateCommand(OpenConfigurationCommandExecute);

            ResumeDirtyTracking();
        }

        protected override IList<RibbonGroupViewModel> BuildRibbonGroups()
        {
            var documentGroup = CreateDocumentGroup(GenerateReportCommand);
            var personnelActions = new List<RibbonActionViewModel>
            {
                new RibbonActionViewModel(CaptionAddService, RibbonIconKeys.AddRow, AddServiceCommand, AutomationIdAddService),
                new RibbonActionViewModel(CaptionAddWitness, RibbonIconKeys.AddRow, AddWitnessCommand, AutomationIdAddWitness),
                new RibbonActionViewModel(CaptionRemoveWitness, RibbonIconKeys.Remove, RemoveWitnessCommand, AutomationIdRemoveWitness, isDestructive: true)
            };
            var personnelGroup = new RibbonGroupViewModel(GroupTitlePersonnel, personnelActions);
            var reportGroup = CreateReportGroup(OpenConfigurationCommand, null);
            var groups = new List<RibbonGroupViewModel> { documentGroup, personnelGroup, reportGroup };
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

        public MilitaryServiceViewModel? AddServiceToDictionary(string nominativeName, string genitiveName, AssetType assetType)
        {
            var key = Guid.NewGuid().ToString();
            var dto = new MilitaryServiceDTO();
            dto.ServiceNameFull.Value = nominativeName;
            dto.ServiceNameGenitive.Value = genitiveName;
            dto.AssetTypes = [assetType.ToString()];

            _reportDataService.SaveServiceData(key, dto);
            ReloadServices();

            var newVm = AvailableServices.FirstOrDefault(vm => vm.ServiceKey == key);
            return newVm;
        }

        private void AddService()
        {
            var service = new WriteOffServiceViewModel(
                AvailableServices,
                AssetTypes,
                AvailableMeasurementUnits,
                RemoveService,
                AddServiceToDictionary);
            Services.Add(service);
        }

        private void RemoveService(WriteOffServiceViewModel service)
        {
            Services.Remove(service);
        }

        private void AddWitness()
        {
            Witnesses.Add(new WriteOffWitnessViewModel());
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

            var creatorDto = new PersonDTO(CreatorName, CreatorPosition, CreatorRank);

            var reportData = (WriteOffOrderReportData)BuildReportData();
            reportData.DestinationFolder = folderName;

            _dataService.AlterPeople([creatorDto]);
            _reportManager.GenerateWriteOffOrder(reportData, EnsureHistoryEntryId());

            ResetDirtyState();
        }

        protected override IReportData BuildReportData()
        {
            var reportData = new WriteOffOrderReportData
            {
                ReportNum = ReportNum,
                ReportDate = ReportDate,
                EventDate = EventDate,
                EventTime = EventTime,
                BattleOrder = BattleOrder,
                BattleOrderDate = BattleOrderDate,
                BattleOrderLocation = BattleOrderLocation,
                SubdivisionName = SubdivisionName,
                ReporterRank = ReporterRank,
                ReporterName = ReporterName,
                CreatorPosition = CreatorPosition,
                CreatorRank = CreatorRank,
                CreatorName = CreatorName,
                MilUnitApproval = MilUnitApproval,
                WhatHappened = WhatHappened,
                Services = [.. Services.Select(s => s.ToServiceData())],
                Witnesses = [.. Witnesses.Select(w => w.ToWitnessData())],
            };

            return reportData;
        }

        public void LoadReportData(IWriteOffOrderReportData data)
        {
            WithDirtyTrackingSuspended(() =>
            {
                ReportNum = data.ReportNum;
                ReportDate = data.ReportDate;
                EventDate = data.EventDate;
                EventTime = data.EventTime;
                BattleOrder = data.BattleOrder;
                BattleOrderDate = data.BattleOrderDate;
                BattleOrderLocation = data.BattleOrderLocation;
                SubdivisionName = data.SubdivisionName;
                ReporterRank = data.ReporterRank;
                ReporterName = data.ReporterName;
                CreatorPosition = data.CreatorPosition;
                CreatorRank = data.CreatorRank;
                CreatorName = data.CreatorName;
                MilUnitApproval = data.MilUnitApproval;
                WhatHappened = data.WhatHappened;

                Services.Clear();
                foreach (var serviceData in data.Services ?? [])
                {
                    var serviceViewModel = new WriteOffServiceViewModel(
                        AvailableServices,
                        AssetTypes,
                        AvailableMeasurementUnits,
                        RemoveService,
                        AddServiceToDictionary);

                    serviceViewModel.LoadFrom(serviceData);
                    Services.Add(serviceViewModel);
                }

                Witnesses.Clear();
                foreach (var witnessData in data.Witnesses ?? [])
                {
                    Witnesses.Add(WriteOffWitnessViewModel.FromWitnessData(witnessData));
                }
            });
        }

        private void OpenConfigurationCommandExecute()
        {
            OpenSettings(ReportType.WriteOffOrder);
        }
    }
}
