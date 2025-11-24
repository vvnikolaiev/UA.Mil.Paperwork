namespace Mil.Paperwork.UI.Memento
{
    internal class AssetDismantlingViewModelMemento : AssetValuationViewModelMemento
    {
        public string RegistrationNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string NomenclatureCode { get; set; }
        public string DismantlingReason { get; set; }
    }
}
