using Spire.Doc.Documents;

namespace Mil.Paperwork.Domain.DataModels.Parameters
{
    internal struct WordCellParameters
    {
        public int FontSize { get; set; }
        public bool IsBold { get; set; }
        public HorizontalAlignment? HorizontalAlignment { get; set; }
        public VerticalAlignment? VerticalAlignment { get; set; }

        public WordCellParameters(int fontSize = 12, HorizontalAlignment? horizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Center, VerticalAlignment? verticalAlignment = null, bool isBold = false)
        {
            FontSize = fontSize;
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
            IsBold = isBold;
        }
    }
}
