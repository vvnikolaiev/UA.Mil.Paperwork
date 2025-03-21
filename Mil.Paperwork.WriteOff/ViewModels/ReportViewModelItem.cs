using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.WriteOff.Enums;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class ReportViewModelItem : ObservableItem
    {
        public string Text { get; set; }

        public DocumentTypeEnum DocumentType { get; set; }

        public ReportViewModelItem(string text, DocumentTypeEnum documentType)
        {
            Text = text;
            DocumentType = documentType;
        }
    }
}
