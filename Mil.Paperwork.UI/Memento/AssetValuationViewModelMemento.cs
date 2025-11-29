using System;
using System.Collections.Generic;

namespace Mil.Paperwork.UI.Memento
{
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
