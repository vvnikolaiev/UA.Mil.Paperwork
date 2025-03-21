namespace Mil.Paperwork.WriteOff.Memento
{
    internal class AssetValuationItemViewModelMemento
    {
        public string Name { get; set; }
        public string NomenclatureCode { get; set; }
        public string MeasurementUnit { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool Exclude { get; set; }
    }
}
