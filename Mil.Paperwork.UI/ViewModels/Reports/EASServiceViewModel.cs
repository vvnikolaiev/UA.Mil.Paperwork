using Mil.MVVM.Common;
using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.UI.ViewModels.Dictionaries;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Reports
{
    internal class EASServiceViewModel : ObservableItem
    {
        private MilitaryServiceViewModel? _selectedService;
        private EASAssetViewModel? _selectedAsset;
        private bool _isAddingNewService;
        private string _newServiceNominative = string.Empty;
        private string _newServiceGenitive = string.Empty;
        private AssetType _newServiceAssetType;
        private string _newServiceHeadRank = string.Empty;
        private string _newServiceHeadName = string.Empty;
        private string _newServiceHeadPosition = string.Empty;

        private readonly Action<EASServiceViewModel> _removeCallback;
        private readonly Func<string, string, AssetType, string, string, string, MilitaryServiceViewModel?> _addServiceCallback;
        private readonly Action<MilitaryServiceViewModel> _saveHeadCallback;

        public IList<MilitaryServiceViewModel> AvailableServices { get; }
        public ObservableCollection<AssetType> AssetTypes { get; }
        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }
        public ObservableCollection<EASAssetViewModel> Assets { get; }

        public MilitaryServiceViewModel? SelectedService
        {
            get => _selectedService;
            set
            {
                if (_selectedService != null)
                {
                    _selectedService.PropertyChanged -= OnSelectedServicePropertyChanged;
                }

                if (SetProperty(ref _selectedService, value))
                {
                    if (_selectedService != null)
                    {
                        _selectedService.PropertyChanged += OnSelectedServicePropertyChanged;
                    }

                    OnPropertyChanged(nameof(IsHeadMissing));
                }
            }
        }

        public EASAssetViewModel? SelectedAsset
        {
            get => _selectedAsset;
            set => SetProperty(ref _selectedAsset, value);
        }

        public bool IsAddingNewService
        {
            get => _isAddingNewService;
            set => SetProperty(ref _isAddingNewService, value);
        }

        public string NewServiceNominative
        {
            get => _newServiceNominative;
            set => SetProperty(ref _newServiceNominative, value);
        }

        public string NewServiceGenitive
        {
            get => _newServiceGenitive;
            set => SetProperty(ref _newServiceGenitive, value);
        }

        public AssetType NewServiceAssetType
        {
            get => _newServiceAssetType;
            set => SetProperty(ref _newServiceAssetType, value);
        }

        public string NewServiceHeadRank
        {
            get => _newServiceHeadRank;
            set => SetProperty(ref _newServiceHeadRank, value);
        }

        public string NewServiceHeadName
        {
            get => _newServiceHeadName;
            set => SetProperty(ref _newServiceHeadName, value);
        }

        public string NewServiceHeadPosition
        {
            get => _newServiceHeadPosition;
            set => SetProperty(ref _newServiceHeadPosition, value);
        }

        public bool IsHeadMissing => SelectedService != null && string.IsNullOrWhiteSpace(SelectedService.HeadName);

        public decimal ServiceSubtotal
        {
            get
            {
                var result = Math.Round(Assets.Sum(a => a.Sum), 2);
                return result;
            }
        }

        public IDelegateCommand AddAssetCommand { get; }
        public IDelegateCommand RemoveAssetCommand { get; }
        public IDelegateCommand RequestRemoveCommand { get; }
        public IDelegateCommand ToggleAddServiceFormCommand { get; }
        public IDelegateCommand ConfirmAddServiceCommand { get; }
        public IDelegateCommand SaveHeadCommand { get; }

        public EASServiceViewModel(
            IList<MilitaryServiceViewModel> availableServices,
            ObservableCollection<AssetType> assetTypes,
            IList<MeasurementUnitViewModel> measurementUnits,
            Action<EASServiceViewModel> removeCallback,
            Func<string, string, AssetType, string, string, string, MilitaryServiceViewModel?> addServiceCallback,
            Action<MilitaryServiceViewModel> saveHeadCallback)
        {
            AvailableServices = availableServices;
            AssetTypes = assetTypes;
            MeasurementUnits = new ObservableCollection<MeasurementUnitViewModel>(measurementUnits);
            Assets = [];
            Assets.CollectionChanged += OnAssetsCollectionChanged;

            _removeCallback = removeCallback;
            _addServiceCallback = addServiceCallback;
            _saveHeadCallback = saveHeadCallback;

            AddAssetCommand = new DelegateCommand(AddAsset);
            RemoveAssetCommand = new DelegateCommand(RemoveAsset);
            RequestRemoveCommand = new DelegateCommand(RequestRemoveCommandExecute);
            ToggleAddServiceFormCommand = new DelegateCommand(ToggleAddServiceForm);
            ConfirmAddServiceCommand = new DelegateCommand(ConfirmAddService);
            SaveHeadCommand = new DelegateCommand(SaveHead);
        }

        private void RequestRemoveCommandExecute()
        {
            _removeCallback(this);
        }

        private void ToggleAddServiceForm()
        {
            IsAddingNewService = !IsAddingNewService;
            if (IsAddingNewService)
            {
                NewServiceNominative = string.Empty;
                NewServiceGenitive = string.Empty;
                NewServiceAssetType = AssetType.Default;
                NewServiceHeadRank = string.Empty;
                NewServiceHeadName = string.Empty;
                NewServiceHeadPosition = string.Empty;
            }
        }

        private void ConfirmAddService()
        {
            if (string.IsNullOrWhiteSpace(NewServiceNominative))
            {
                return;
            }

            if (NewServiceAssetType == AssetType.Default)
            {
                return;
            }

            var newVm = _addServiceCallback(
                NewServiceNominative, NewServiceGenitive, NewServiceAssetType,
                NewServiceHeadRank, NewServiceHeadName, NewServiceHeadPosition);
            SelectedService = newVm;
            IsAddingNewService = false;
        }

        private void SaveHead()
        {
            if (SelectedService != null)
            {
                _saveHeadCallback(SelectedService);
            }
        }

        private void OnSelectedServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MilitaryServiceViewModel.HeadName))
            {
                OnPropertyChanged(nameof(IsHeadMissing));
            }
        }

        private void OnAssetsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (EASAssetViewModel item in e.NewItems)
                {
                    item.PropertyChanged += OnAssetPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (EASAssetViewModel item in e.OldItems)
                {
                    item.PropertyChanged -= OnAssetPropertyChanged;
                }
            }

            OnPropertyChanged(nameof(ServiceSubtotal));
        }

        private void OnAssetPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EASAssetViewModel.Sum))
            {
                OnPropertyChanged(nameof(ServiceSubtotal));
            }
        }

        private void AddAsset()
        {
            var asset = new EASAssetViewModel();
            Assets.Add(asset);
            SelectedAsset = asset;
        }

        private void RemoveAsset()
        {
            if (SelectedAsset != null)
            {
                Assets.Remove(SelectedAsset);
                SelectedAsset = Assets.LastOrDefault();
            }
        }

        public void LoadFrom(EASServiceData data)
        {
            SelectedService = AvailableServices.FirstOrDefault(service => service.NominativeName == data.ServiceName);

            Assets.Clear();
            foreach (var assetData in data.Assets ?? [])
            {
                Assets.Add(EASAssetViewModel.FromAssetData(assetData));
            }

            SelectedAsset = Assets.FirstOrDefault();
        }

        public EASServiceData ToServiceData()
        {
            var result = new EASServiceData
            {
                ServiceName = SelectedService?.NominativeName ?? string.Empty,
                ServiceNameGenitive = SelectedService?.GenitiveName ?? string.Empty,
                HeadRank = SelectedService?.HeadRank ?? string.Empty,
                HeadName = SelectedService?.HeadName ?? string.Empty,
                HeadPosition = SelectedService?.HeadPosition ?? string.Empty,
                Assets = [.. Assets.Select(a => a.ToAssetData())]
            };
            return result;
        }
    }
}
