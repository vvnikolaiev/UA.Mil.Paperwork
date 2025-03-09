using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    public class WriteOffReportViewModel : ObservableItem
    {
        private WriteOffReportModel _model;
        private ObservableCollection<ProductDTO> _products;
        private string _registrationNumber = string.Empty;
        private string _documentNumber = string.Empty;
        private DateTime _writeOffDate = DateTime.Now.Date;
        private string _reason = string.Empty;
        private string _destinationFolderPath = "C:\\Work\\Temp";
        private AssetViewModel _selectedAsset;

        public ObservableCollection<AssetViewModel> AssetsCollection { get; set; }

        public ObservableCollection<ProductDTO> Products
        {
            get => _products; 
            set => SetProperty(ref _products, value);
        }

        public AssetViewModel SelectedAsset
        {
            get => _selectedAsset;
            set => SetProperty(ref _selectedAsset, value);
        }

        public string RegistrationNumber
        {
            get => _registrationNumber;
            set => SetProperty(ref _registrationNumber, value);
        }
        public string DocumentNumber
        {
            get => _documentNumber;
            set => SetProperty(ref _documentNumber, value);
        }

        public DateTime WriteOffDate
        {
            get => _writeOffDate;
            set => SetProperty(ref _writeOffDate, value);
        }

        public string DestinationFolderPath
        {
            get => _destinationFolderPath;
            set => SetProperty(ref _destinationFolderPath, value);
        }

        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        public ICommand CopySelectedAssetCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand ClearTableCommand { get; }
        public ICommand AddRowCommand { get; }
        public ICommand RemoveRowCommand { get; }
        public ICommand SelectFolderCommand { get; }

        public WriteOffReportViewModel(ReportManager reportManager, IDataService dataService)
        {
            _model = new WriteOffReportModel(reportManager, dataService);
            AssetsCollection = new ObservableCollection<AssetViewModel>();
            Products = [.. _model.LoadProductData()];

            GenerateReportCommand = new DelegateCommand(GenerateReport);
            ClearTableCommand = new DelegateCommand(ClearTable);
            AddRowCommand = new DelegateCommand(AddRow);
            RemoveRowCommand = new DelegateCommand(RemoveRowExecute);
            SelectFolderCommand = new DelegateCommand(SelectFolder);
            CopySelectedAssetCommand = new DelegateCommand(CopySelectedAssetExecute);
        }

        private void GenerateReport()
        {
            var reportData = new WriteOffReportData
            {
                DestinationFolder = DestinationFolderPath,
                RegistrationNumber = RegistrationNumber,
                DocumentNumber = DocumentNumber,
                Reason = Reason,
                WriteOffDate = WriteOffDate,
                Assets = AssetsCollection.Select(x => x.ToAssetInfo()).ToList()
            };

            _model.GenerateReport(reportData);

            UpdateProductsCollection();
        }

        private void UpdateProductsCollection()
        {
            Products = [.. _model.LoadProductData()];
        }

        private void ClearTable()
        {
            if (MessageBox.Show("Are you sure you want to clear the table?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AssetsCollection.Clear();
            }
        }

        private void AddRow()
        {
            var asset = CreateDefaultAsset();
            AssetsCollection.Add(asset);

            SelectedAsset = asset;
        }

        private void RemoveRowExecute()
        {
            if (SelectedAsset != null)
            {
                AssetsCollection.Remove(SelectedAsset);

                SelectedAsset = AssetsCollection.FirstOrDefault();
            }
        }

        private void CopySelectedAssetExecute()
        {
            if (SelectedAsset != null)
            {
                var copiedItem = new AssetViewModel(new AssetInfo
                {
                    Name = _selectedAsset.Name,
                    MeasurementUnit = _selectedAsset.MeasurementUnit,
                    SerialNumber = _selectedAsset.SerialNumber,
                    NomenclatureCode = _selectedAsset.NomenclatureCode,
                    Category = _selectedAsset.Category,
                    Price = _selectedAsset.Price,
                    Count = _selectedAsset.Count,
                    WearAndTearCoeff = _selectedAsset.WearAndTearCoeff,
                    StartDate = _selectedAsset.StartDate,
                    TSRegisterNumber = _selectedAsset.TSRegisterNumber,
                    TSDocumentNumber = _selectedAsset.TSDocumentNumber,
                    WarrantyPeriodYears = _selectedAsset.WarrantyPeriodYears
                });

                AssetsCollection.Add(copiedItem);

                SelectedAsset = copiedItem;
            }
        }

        private void SelectFolder()
        {
            var a = new OpenFolderDialog();
            if (a.ShowDialog() == true)
            {
                DestinationFolderPath = a.FolderName;
            }
        }

        private AssetViewModel CreateDefaultAsset()
        {
            var assetInfo = new AssetInfo();
            var assetVM = new AssetViewModel(assetInfo);
            return assetVM;
        }
    }
}
