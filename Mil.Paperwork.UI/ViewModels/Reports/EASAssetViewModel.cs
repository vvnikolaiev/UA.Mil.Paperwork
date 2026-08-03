using Mil.MVVM.Common;
using Mil.Paperwork.Domain.DataModels;
using System;

namespace Mil.Paperwork.UI.ViewModels.Reports
{
    internal class EASAssetViewModel : ObservableItem
    {
        private string _name = string.Empty;
        private string _code = string.Empty;
        private string _measurementUnit = string.Empty;
        private int _category;
        private int _count = 1;
        private decimal _originalPrice;
        private decimal _residualPrice;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        public string MeasurementUnit
        {
            get => _measurementUnit;
            set => SetProperty(ref _measurementUnit, value);
        }

        public int Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public int Count
        {
            get => _count;
            set
            {
                if (SetProperty(ref _count, value))
                {
                    OnPropertyChanged(nameof(Sum));
                }
            }
        }

        public decimal OriginalPrice
        {
            get => _originalPrice;
            set => SetProperty(ref _originalPrice, value);
        }

        public decimal ResidualPrice
        {
            get => _residualPrice;
            set
            {
                if (SetProperty(ref _residualPrice, value))
                {
                    OnPropertyChanged(nameof(Sum));
                }
            }
        }

        public decimal Sum => Math.Round(Count * ResidualPrice, 2);

        public static EASAssetViewModel FromAssetData(EASAssetData data)
        {
            var result = new EASAssetViewModel
            {
                Name = data.Name,
                Code = data.Code,
                MeasurementUnit = data.MeasurementUnit,
                Category = data.Category,
                Count = data.Count,
                OriginalPrice = data.OriginalPrice,
                ResidualPrice = data.ResidualPrice
            };
            return result;
        }

        public EASAssetData ToAssetData()
        {
            var result = new EASAssetData
            {
                Name = Name,
                Code = Code,
                MeasurementUnit = MeasurementUnit,
                Category = Category,
                Count = Count,
                OriginalPrice = OriginalPrice,
                ResidualPrice = ResidualPrice
            };
            return result;
        }
    }
}
