using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.MVVM.Common;

namespace Mil.Paperwork.UI.ViewModels
{
    internal class ReportItemViewModel : ObservableItem
    {
        public string Text { get; set; }

        public ReportType DocumentType { get; set; }

        public ReportItemViewModel(ReportType documentType)
        {
            Text = documentType.GetDescription();
            DocumentType = documentType;
        }
    }
}
