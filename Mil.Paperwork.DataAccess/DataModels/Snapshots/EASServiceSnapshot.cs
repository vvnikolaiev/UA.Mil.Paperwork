namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class EASServiceSnapshot
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceNameGenitive { get; set; } = string.Empty;
        public string HeadRank { get; set; } = string.Empty;
        public string HeadName { get; set; } = string.Empty;
        public string HeadPosition { get; set; } = string.Empty;
        public List<EASAssetSnapshot> Assets { get; set; } = [];
    }
}
