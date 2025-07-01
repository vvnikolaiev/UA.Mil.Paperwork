namespace Mil.Paperwork.WriteOff.DataModels
{
    internal class ImportDataResult(bool isSuccessful, int rowsCount = 0)
    {
        public bool IsSuccessful { get; set; } = isSuccessful;
        public int ImportedRowsCount { get; set; } = rowsCount;
        public int InvalidRowsCount { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public IList<object> Rows { get; set; }
    }
}
