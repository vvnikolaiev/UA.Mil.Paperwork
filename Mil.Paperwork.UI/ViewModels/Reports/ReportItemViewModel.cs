using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.UI.Enums;

namespace Mil.Paperwork.UI.ViewModels
{
    internal class ReportItemViewModel : ObservableItem
    {
        public string Text { get; set; }

        public DocumentTypeEnum DocumentType { get; set; }

        public ReportItemViewModel(DocumentTypeEnum documentType)
        {
            Text = documentType.GetDescription();
            DocumentType = documentType;
        }
    }
}
