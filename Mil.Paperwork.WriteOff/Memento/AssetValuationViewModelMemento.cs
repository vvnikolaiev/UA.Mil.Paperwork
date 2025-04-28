namespace Mil.Paperwork.WriteOff.Memento
{
    internal class AssetDismantlingViewModelMemento : AssetValuationViewModelMemento
    {
        public string RegistrationNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string NomenclatureCode { get; set; }
        public string DismantlingReason { get; set; }
    }

    internal class AssetValuationViewModelMemento
    {
        public bool IsValid { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string SerialNumber { get; set; }
        public string Description { get; set; }
        public DateTime ValuationDate { get; set; }
        public IList<AssetValuationItemViewModelMemento> Components { get; set; }
        //public AssetValuationData SelectedValuationTemplate { get; set; }
        //public ObservableCollection<AssetValuationData> ValuationDataTemplates { get; set; }
    }
}
