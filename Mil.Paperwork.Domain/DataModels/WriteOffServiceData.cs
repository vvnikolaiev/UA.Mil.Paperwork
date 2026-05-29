namespace Mil.Paperwork.Domain.DataModels
{
    public class WriteOffServiceData
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceNameGenitive { get; set; } = string.Empty;
        public IList<WriteOffServiceAssetData> Assets { get; set; } = [];
    }
}
