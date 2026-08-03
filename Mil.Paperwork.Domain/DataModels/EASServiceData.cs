namespace Mil.Paperwork.Domain.DataModels
{
    public class EASServiceData
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceNameGenitive { get; set; } = string.Empty;
        public string HeadRank { get; set; } = string.Empty;
        public string HeadName { get; set; } = string.Empty;
        public string HeadPosition { get; set; } = string.Empty;
        public IList<EASAssetData> Assets { get; set; } = [];
    }
}
