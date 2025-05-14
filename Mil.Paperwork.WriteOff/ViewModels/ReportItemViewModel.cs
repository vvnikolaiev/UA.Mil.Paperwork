using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.WriteOff.Enums;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class ReportItemViewModel : ObservableItem
    {
        public string Text { get; set; }

        public DocumentTypeEnum DocumentType { get; set; }

        public ReportItemViewModel(string text, DocumentTypeEnum documentType)
        {
            Text = text;
            DocumentType = documentType;
        }
    }
}
