using Mil.MVVM.Common;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Managers;
using Mil.Paperwork.UI.ViewModels.Dictionaries;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Reports
{
    internal class WriteOffOrderViewModel : BaseReportTabViewModel
    {
        private const string HeaderText = "Наказ про списання";
        private const string MilUnitConfigKey = "MIL_UNIT";

        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly IDialogService _dialogService;

        // Event fields
        private string _reportNum = string.Empty;
        private DateTime _reportDate = DateTime.Today;
        private DateTime _eventDate = DateTime.Today;
        private int _eventHour = 0;
        private int _eventMinute = 0;
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

        public ObservableCollection<MilServiceEntry> AvailableServices { get; }
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
        public DateTime ReportDate { get => _reportDate; set => SetProperty(ref _reportDate, value); }
        public DateTime EventDate { get => _eventDate; set => SetProperty(ref _eventDate, value); }
        public int EventHour
        {
            get => _eventHour;
            set { SetProperty(ref _eventHour, value); OnPropertyChanged(nameof(EventTime)); }
        }
        public int EventMinute
        {
            get => _eventMinute;
            set { SetProperty(ref _eventMinute, value); OnPropertyChanged(nameof(EventTime)); }
        }
        public string EventTime => $"{_eventHour:D2}:{_eventMinute:D2}";
        public string BattleOrder { get => _battleOrder; set => SetProperty(ref _battleOrder, value); }
        public DateTime BattleOrderDate { get => _battleOrderDate; set => SetProperty(ref _battleOrderDate, value); }
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

        public WriteOffOrderViewModel(
            ReportManager reportManager,
            IDataService dataService,
            IReportDataService reportDataService,
            IDialogService dialogService)
            : base(dialogService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
            _reportDataService = reportDataService;
            _dialogService = dialogService;

            AvailableServices = new ObservableCollection<MilServiceEntry>(_dataService.LoadServicesData());
            AvailableMeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(u => new MeasurementUnitViewModel(u))];
            AvailablePeople = [.. _dataService.LoadPeopleData().Select(p => new PersonViewModel(p))];
            Services = [];
            Witnesses = [];

            var commonParams = _reportDataService.GetReportParametersDictionary(ReportType.Common);
            MilUnitApproval = commonParams.GetValueOrDefault(MilUnitConfigKey, string.Empty);

            AddServiceCommand = new DelegateCommand(AddService);
            AddWitnessCommand = new DelegateCommand(AddWitness);
            RemoveWitnessCommand = new DelegateCommand(RemoveSelectedWitness);
            GenerateReportCommand = new DelegateCommand(GenerateReportCommandExecute);
            OpenConfigurationCommand = new DelegateCommand(OpenConfigurationCommandExecute);
        }

        public MilServiceEntry AddServiceToDictionary(string nominativeName, string genitiveName)
        {
            var newEntry = new MilServiceEntry
            {
                NominativeName = nominativeName,
                GenitiveName = genitiveName
            };

            var allServices = _dataService.LoadServicesData().ToList();
            allServices.Add(newEntry);
            _dataService.SaveServicesData(allServices);
            AvailableServices.Add(newEntry);

            return newEntry;
        }

        private void AddService()
        {
            var service = new WriteOffServiceViewModel(
                AvailableServices,
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
                Witnesses.Remove(SelectedWitness);
        }

        private async void GenerateReportCommandExecute()
        {
            if (!_dialogService.TryPickFolder(out var folderName))
                return;

            var creatorDto = new PersonDTO(CreatorName, CreatorPosition, CreatorRank);

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
                DestinationFolder = folderName
            };

            _dataService.AlterPeople([creatorDto]);
            _reportManager.GenerateWriteOffOrder(reportData);
        }

        private void OpenConfigurationCommandExecute()
        {
            OpenSettings(ReportType.WriteOffOrder);
        }
    }
}
