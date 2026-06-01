using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.DataModels.Configuration;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.MVVM.Common;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    public class MilitaryServiceViewModel : ObservableItem
    {
        private const string DefaultServiceString = " (за замовчуванням)";

        private string _serviceKey;
        private string _description;
        private bool _isMarkedAsDefault;
        private bool _isSaved;
        private AssetType _assetType;

        public string ServiceKey
        {
            get => _serviceKey;
            set => SetProperty(ref _serviceKey, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public bool IsMarkedAsDefault
        {
            get => _isMarkedAsDefault;
            private set => SetProperty(ref _isMarkedAsDefault, value);
        }

        public bool IsSaved
        {
            get => _isSaved;
            private set => SetProperty(ref _isSaved, value);
        }

        public string Title
        {
            get
            {
                var defaultSuffix = IsMarkedAsDefault ? DefaultServiceString : string.Empty;
                return $"{_serviceNameFull?.Value ?? _serviceName?.Value ?? ServiceKey}{defaultSuffix}";
            }
        }

        public AssetType AssetType
        {
            get => _assetType;
            set => SetProperty(ref _assetType, value);
        }

        public string NominativeName
        {
            get => _serviceNameFull.Value;
            set
            {
                if (_serviceNameFull.Value != value)
                {
                    _serviceNameFull.Value = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string GenitiveName
        {
            get => _serviceNameGenitive.Value;
            set
            {
                if (_serviceNameGenitive.Value != value)
                {
                    _serviceNameGenitive.Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ShortName
        {
            get => _serviceName.Value;
            set
            {
                if (_serviceName.Value != value)
                {
                    _serviceName.Value = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string HeadName
        {
            get => _headOfServiceName.Value;
            set
            {
                if (_headOfServiceName.Value != value)
                {
                    _headOfServiceName.Value = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HeadDisplayString));
                }
            }
        }

        public string HeadPosition
        {
            get => _headOfServicePosition.Value;
            set
            {
                if (_headOfServicePosition.Value != value)
                {
                    _headOfServicePosition.Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public string HeadRank
        {
            get => _headOfServiceRank.Value;
            set
            {
                if (_headOfServiceRank.Value != value)
                {
                    _headOfServiceRank.Value = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HeadDisplayString));
                }
            }
        }

        public string HeadDisplayString
        {
            get
            {
                var rank = _headOfServiceRank?.Value;
                var name = _headOfServiceName?.Value;
                string result;

                if (string.IsNullOrEmpty(rank))
                {
                    result = name ?? string.Empty;
                }
                else if (string.IsNullOrEmpty(name))
                {
                    result = rank;
                }
                else
                {
                    result = $"{rank} {name}";
                }

                return result;
            }
        }

        private ReportParameter _serviceName;
        private ReportParameter _serviceNameFull;
        private ReportParameter _serviceNameGenitive;
        private ReportParameter _headOfServiceName;
        private ReportParameter _headOfServicePosition;
        private ReportParameter _headOfServiceRank;


        public ObservableCollection<ReportParameter> Parameters { get; private set; }

        public MilitaryServiceViewModel(string key) : base()
        {
            ServiceKey = key;
            var dto = new MilitaryServiceDTO();
            Initialize(dto);
        }

        public MilitaryServiceViewModel(string key, MilitaryServiceDTO dto) : this(key)
        {
            ServiceKey = key;
            Initialize(dto);
            IsSaved = true;
        }

        private void Initialize(MilitaryServiceDTO dto)
        {
            UpdateAssetTypesCollection(dto.AssetTypes);

            _serviceName = dto.ServiceName;
            _serviceNameFull = dto.ServiceNameFull;
            _serviceNameGenitive = dto.ServiceNameGenitive;
            _headOfServiceName = dto.HeadOfServiceName;
            _headOfServiceRank = dto.HeadOfServiceRank;
            _headOfServicePosition = dto.HeadOfServicePosition;
            _description = dto.Description;

            Parameters =
            [
                _serviceName,
                _serviceNameFull,
                _serviceNameGenitive,
                _headOfServiceName,
                _headOfServicePosition,
                _headOfServiceRank
            ];
        }

        public void SetAsDefault(bool isDefault)
        {
            IsMarkedAsDefault = isDefault;
            OnPropertyChanged(nameof(Title));
        }

        public void SetAsSaved()
        {
            IsSaved = true;
        }

        private void UpdateAssetTypesCollection(List<string> stringAssetTypes)
        {
            var assetTypes = stringAssetTypes.Select(x =>
            {
                var isSuccessul = EnumHelper.TryGetEnumValue(x, out AssetType assetType);
                return isSuccessul ? assetType : AssetType.Default;
            });

            _assetType = assetTypes.FirstOrDefault(x => x != AssetType.Default);
        }

        public MilitaryServiceDTO GetDTO()
        {
            var result = new MilitaryServiceDTO()
            {
                ServiceName = _serviceName,
                ServiceNameFull = _serviceNameFull,
                ServiceNameGenitive = _serviceNameGenitive,
                HeadOfServiceName = _headOfServiceName,
                HeadOfServiceRank = _headOfServiceRank,
                HeadOfServicePosition = _headOfServicePosition,
                Description = _description,
                AssetTypes = [AssetType.ToString()]
                //AssetTypes = AssetTypes.Select(x => x.ToString()).ToList()
            };
            return result;
        }
    }
}
