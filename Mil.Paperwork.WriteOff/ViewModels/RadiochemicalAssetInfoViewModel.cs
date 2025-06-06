﻿using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Managers;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class RadiochemicalAssetInfoViewModel : AssetViewModel
    {
        private readonly RadiochemicalAssetInfo _assetInfo;

        private bool _isLocal = true;

        // for future iplementation of IDataGridColumnProvider using Source Generator?
        //[DataGridColumn("Local?", ColumnType.CheckBox)]
        public bool IsLocal
        {
            get => _isLocal;
            set => SetProperty(ref _isLocal, value);
        }

        internal override IAssetInfo AssetInfo => _assetInfo;

        public RadiochemicalAssetInfoViewModel(
            ReportManager reportManager,
            IDataService dataService,
            INavigationService navigationService) : base(reportManager, dataService, navigationService)
        {
            _assetInfo = new RadiochemicalAssetInfo();
        }

        public override IAssetInfo ToAssetInfo(EventType eventType = EventType.Lost, DateTime ? reportDate = null)
        {
            base.ToAssetInfo(eventType, reportDate);
            _assetInfo.IsLocal = IsLocal;

            return _assetInfo;
        }
    }
}
