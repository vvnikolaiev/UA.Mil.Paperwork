namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class WriteOffServiceSnapshot
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceNameGenitive { get; set; } = string.Empty;
        public List<WriteOffServiceAssetSnapshot> Assets { get; set; } = [];
    }
}
