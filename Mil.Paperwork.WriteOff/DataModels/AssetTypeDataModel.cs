using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.WriteOff.DataModels
{
    internal class AssetTypeDataModel
    {
        public AssetType AssetType { get; set; }
        public string Title { get; set; }

        public AssetTypeDataModel(AssetType assetType, string title)
        {
            AssetType = assetType;
            Title = title;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
