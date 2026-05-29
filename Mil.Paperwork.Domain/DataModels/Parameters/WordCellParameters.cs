namespace Mil.Paperwork.Domain.DataModels.Parameters
{
    internal enum WordHorizontalAlignment { Left, Center, Right }
    internal enum WordVerticalAlignment   { Top, Center, Middle, Bottom }

    internal struct WordCellParameters
    {
        public int FontSize { get; set; }
        public bool IsBold { get; set; }
        public WordHorizontalAlignment? HorizontalAlignment { get; set; }
        public WordVerticalAlignment? VerticalAlignment { get; set; }

        public WordCellParameters(int fontSize = 12, WordHorizontalAlignment? horizontalAlignment = WordHorizontalAlignment.Center, WordVerticalAlignment? verticalAlignment = null, bool isBold = false)
        {
            FontSize = fontSize;
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
            IsBold = isBold;
        }
    }
}
