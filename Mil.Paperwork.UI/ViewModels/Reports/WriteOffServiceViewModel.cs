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
    internal class WriteOffServiceViewModel : ObservableItem
    {
        private MilitaryServiceViewModel? _selectedService;
        private WriteOffServiceAssetViewModel? _selectedAsset;
        private bool _isAddingNewService;
        private string _newServiceNominative = string.Empty;
        private string _newServiceGenitive = string.Empty;
        private AssetType _newServiceAssetType;

        private readonly Action<WriteOffServiceViewModel> _removeCallback;
        private readonly Func<string, string, AssetType, MilitaryServiceViewModel?> _addServiceCallback;

        public IList<MilitaryServiceViewModel> AvailableServices { get; }
        public ObservableCollection<AssetType> AssetTypes { get; }
        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }
        public ObservableCollection<WriteOffServiceAssetViewModel> Assets { get; }

        public MilitaryServiceViewModel? SelectedService
        {
            get => _selectedService;
            set => SetProperty(ref _selectedService, value);
        }

        public WriteOffServiceAssetViewModel? SelectedAsset
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

        public decimal ServiceSubtotal
        {
            get
            {
                var result = Math.Round(Assets.Sum(a => a.Amount), 2);
                return result;
            }
        }

        public IDelegateCommand AddAssetCommand { get; }
        public IDelegateCommand RemoveAssetCommand { get; }
        public IDelegateCommand RequestRemoveCommand { get; }
        public IDelegateCommand ToggleAddServiceFormCommand { get; }
        public IDelegateCommand ConfirmAddServiceCommand { get; }

        public WriteOffServiceViewModel(
            IList<MilitaryServiceViewModel> availableServices,
            ObservableCollection<AssetType> assetTypes,
            IList<MeasurementUnitViewModel> measurementUnits,
            Action<WriteOffServiceViewModel> removeCallback,
            Func<string, string, AssetType, MilitaryServiceViewModel?> addServiceCallback)
        {
            AvailableServices = availableServices;
            AssetTypes = assetTypes;
            MeasurementUnits = new ObservableCollection<MeasurementUnitViewModel>(measurementUnits);
            Assets = [];
            Assets.CollectionChanged += OnAssetsCollectionChanged;

            _removeCallback = removeCallback;
            _addServiceCallback = addServiceCallback;

            AddAssetCommand = new DelegateCommand(AddAsset);
            RemoveAssetCommand = new DelegateCommand(RemoveAsset);
            RequestRemoveCommand = new DelegateCommand(RequestRemoveCommandExecute);
            ToggleAddServiceFormCommand = new DelegateCommand(ToggleAddServiceForm);
            ConfirmAddServiceCommand = new DelegateCommand(ConfirmAddService);
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

            var newVm = _addServiceCallback(NewServiceNominative, NewServiceGenitive, NewServiceAssetType);
            SelectedService = newVm;
            IsAddingNewService = false;
        }

        private void OnAssetsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (WriteOffServiceAssetViewModel item in e.NewItems)
                {
                    item.PropertyChanged += OnAssetPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (WriteOffServiceAssetViewModel item in e.OldItems)
                {
                    item.PropertyChanged -= OnAssetPropertyChanged;
                }
            }

            OnPropertyChanged(nameof(ServiceSubtotal));
        }

        private void OnAssetPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WriteOffServiceAssetViewModel.Amount))
            {
                OnPropertyChanged(nameof(ServiceSubtotal));
            }
        }

        private void AddAsset()
        {
            var asset = new WriteOffServiceAssetViewModel();
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

        public WriteOffServiceData ToServiceData()
        {
            var result = new WriteOffServiceData
            {
                ServiceName = SelectedService?.NominativeName ?? string.Empty,
                ServiceNameGenitive = SelectedService?.GenitiveName ?? string.Empty,
                Assets = [.. Assets.Select(a => new WriteOffServiceAssetData
                {
                    Name = a.Name,
                    Count = a.Count,
                    MeasurementUnit = a.MeasurementUnit,
                    Amount = a.Amount
                })]
            };
            return result;
        }
    }
}
