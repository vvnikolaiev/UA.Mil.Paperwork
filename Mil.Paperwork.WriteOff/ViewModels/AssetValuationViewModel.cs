using Microsoft.Win32;
using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.Memento;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    public class AssetValuationViewModel : ObservableItem, ITabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;

        private AssetValuationViewModelMemento _memento;

        private bool _isValid;
        private string _name;
        private string _shortName;
        private string _measurementUnit;
        private decimal _totalPrice;
        private string _serialNumber;
        private ObservableCollection<AssetValuationItemViewModel> _components = [];

        private string _description;
        private DateTime _valuationDate = new DateTime(2023, 01, 01);

        private IAssetValuationData _selectedValuationTemplate;
        private ObservableCollection<IAssetValuationData> _valuationDataTemplates = [];

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public virtual string Header => "Акт оцінки";

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string MeasurementUnit
        {
            get => _measurementUnit;
            set => SetProperty(ref _measurementUnit, value);
        }

        public DateTime ValuationDate
        {
            get => _valuationDate;
            set => SetProperty(ref _valuationDate, value);
        }

        public bool IsValid
        {
            get => _isValid;
            set => SetProperty(ref _isValid, value);
        }

        public virtual string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string ShortName
        {
            get => _shortName;
            set => SetProperty(ref _shortName, value);
        }

        public virtual string SerialNumber
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }

        public decimal TotalPrice
        {
            get => _totalPrice;
            set => SetProperty(ref _totalPrice, value);
        }

        public virtual ObservableCollection<AssetValuationItemViewModel> Components
        {
            get => _components;
            set => SetProperty(ref _components, value);
        }

        public int ItemsCount => Components.Count;

        public IAssetValuationData SelectedValuationTemplate
        {
            get => _selectedValuationTemplate;
            set => SetProperty(ref _selectedValuationTemplate, value);
        }

        public ObservableCollection<IAssetValuationData> ValuationDataTemplates
        {
            get => _valuationDataTemplates;
            set => SetProperty(ref _valuationDataTemplates, value);
        }

        public bool IsReadOnly { get; }

        public ICommand ApplyValuationTemplateCommand { get; }
        public ICommand AddRowCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand CloseTabCommand { get; }
        public ICommand GenerateReportCommand { get; }

        public AssetValuationViewModel(ReportManager reportManager, IDataService dataService, INavigationService navigationService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
            _navigationService = navigationService;

            UpdateValuationTemplatesCollection();

            ApplyValuationTemplateCommand = new DelegateCommand(ApplyValuationTemplateExecute);
            AddRowCommand = new DelegateCommand(AddRow);
            ClearCommand = new DelegateCommand(Clear);
            OkCommand = new DelegateCommand(Ok);
            CancelCommand = new DelegateCommand(Cancel);
            CloseTabCommand = new DelegateCommand(CloseTabCommandExecute);
            GenerateReportCommand = new DelegateCommand(GenerateReportCommandExecute);

            SaveState();
        }

        public AssetValuationViewModel(AssetInfo asset, ReportManager reportManager, IDataService dataService, INavigationService navigationService)
            : this(reportManager, dataService, navigationService)
        {
            Name = asset.Name;
            ShortName = asset.ShortName;
            TotalPrice = asset.Price;
            SerialNumber = asset.SerialNumber;
            MeasurementUnit = asset.MeasurementUnit;

            IsReadOnly = true;

            SaveState();
        }

        internal AssetValuationData ToAssetValuationData()
        {
            CalculatePrices();

            var assetValuationData = new AssetValuationData
            {
                Name = Name,
                TotalPrice = TotalPrice,
                SerialNumber = SerialNumber,
                ValuationDate = ValuationDate,
                Description = Description,
                AssetComponents = GetAssetComponents()
            };

            return assetValuationData;
        }

        protected virtual void ValidateData()
        {
            var isValid = true;

            isValid &= !String.IsNullOrEmpty(Name);
            isValid &= Components.Any(x => x.IsValid);
            isValid &= TotalPrice > 0;

            IsValid = isValid;
        }

        protected virtual void ApplyTemplateData(IAssetValuationData valuationDataTemplate)
        {
            var items = valuationDataTemplate.AssetComponents.Select(x => new AssetValuationItemViewModel(x));
            ClearComponents();
            foreach (var item in items)
            {
                AddComponent(item);
            }
        }

        protected virtual void AddComponent(AssetValuationItemViewModel component)
        {
            Components.Add(component);
        }

        protected virtual void ClearComponents()
        {
            Components.Clear();
        }

        protected List<AssetComponent> GetAssetComponents()
        {
            var components = Components
                .Where(x => x.IsValid)
                .Select(x => x.ToAssetComponent())
                .ToList();

            return components;
        }

        private void UpdateValuationTemplatesCollection()
        {
            ValuationDataTemplates = [.. _dataService.LoadValuationData()];
        }

        private void ApplyValuationTemplateExecute()
        {
            if (SelectedValuationTemplate == null || SelectedValuationTemplate.AssetComponentsCount == 0)
            {
                return;
            }

            if (MessageBox.Show("Are you sure you want to replace all the table items?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ApplyTemplateData(SelectedValuationTemplate);
            }
        }

        private void AddRow()
        {
            AddComponent(new AssetValuationItemViewModel());
        }

        private void Clear()
        {
            if (MessageBox.Show("Are you sure you want to clear the table?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ClearComponents();
            }
        }

        private void GenerateReportCommandExecute()
        {
            ValidateData();
            if (IsValid)
            {
                var folderDialog = new OpenFolderDialog();
                // TODO: folderDialog.InitialDirectory = ;
                if (folderDialog.ShowDialog() == true)
                {
                    var folderName = folderDialog.FolderName;
                    GenerateReport(folderName);
                }
            }
        }

        protected virtual void GenerateReport(string folderName)
        {
            IAssetValuationData assetValuationData = ToAssetValuationData();

            if (assetValuationData != null)
            {
                var reportData = new AssetValuationReportData
                {
                    DestinationFolder = folderName,
                    ValuationData = [assetValuationData]
                };

                _reportManager.GenerateValuationReport(reportData);
            }
        }

        private void Ok()
        {
            CalculatePrices();
            ValidateData();
            SaveState();
            CloseWindow();
        }

        private void Cancel()
        {
            // there will be a bug when User closes the window with "X" button.
            RestoreState();
            CloseWindow();
        }

        private void CloseTabCommandExecute()
        {
            if (MessageBox.Show("Are you sure you want to close this tab?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TabCloseRequested.Invoke(this, this);
            }
        }

        protected void CalculatePrices()
        {
            if (Components.Count == 0)
            {
                return;
            }

            var sumLeft = TotalPrice;
            var zeroPriceItems = new List<AssetValuationItemViewModel>();


            foreach (var item in Components)
            {
                if (item.IsValid)
                {
                    sumLeft -= (item.Price * item.Quantity);
                    if (item.Price == 0)
                    {
                        zeroPriceItems.Add(item);
                    }
                }
            }

            if (zeroPriceItems.Count > 0)
            {
                var priceOfLeftItems = sumLeft / zeroPriceItems.Sum(x => x.Quantity);
                zeroPriceItems.ForEach(x => x.Price = priceOfLeftItems);
            }
            else
            {
                var mainComponent = Components.First();
                mainComponent.Price += sumLeft;
            }
        }

        private void CloseWindow()
        {
            _navigationService.CloseWindow(this);
        }

        #region Memento

        public virtual void SaveState()
        {
            _memento = new AssetValuationViewModelMemento
            {
                IsValid = _isValid,
                Name = _name,
                TotalPrice = _totalPrice,
                SerialNumber = _serialNumber,
                Description = _description,
                ValuationDate = _valuationDate,
                Components = [.. _components.Select(c => new AssetValuationItemViewModelMemento
                {
                    Name = c.Name,
                    NomenclatureCode = c.NomenclatureCode,
                    MeasurementUnit = c.MeasurementUnit,
                    Quantity = c.Quantity,
                    Price = c.Price,
                    Exclude = c.Exclude
                })]
                //SelectedValuationTemplate = this.SelectedValuationTemplate,
                //ValuationDataTemplates = new ObservableCollection<AssetValuationData>(this.ValuationDataTemplates)
            };
        }

        public virtual void RestoreState()
        {
            if (_memento == null) return;

            IsValid = _memento.IsValid;
            Name = _memento.Name;
            TotalPrice = _memento.TotalPrice;
            SerialNumber = _memento.SerialNumber;
            Description = _memento.Description;
            ValuationDate = _memento.ValuationDate;
            Components = [.. _memento.Components.Select(m => new AssetValuationItemViewModel
                {
                    Name = m.Name,
                    NomenclatureCode = m.NomenclatureCode,
                    MeasurementUnit = m.MeasurementUnit,
                    Quantity = m.Quantity,
                    Price = m.Price,
                    Exclude = m.Exclude
                })];
            //this.SelectedValuationTemplate = _memento.SelectedValuationTemplate;
            //this.ValuationDataTemplates = new ObservableCollection<AssetValuationData>(_memento.ValuationDataTemplates);
        }

        #endregion
    }
}