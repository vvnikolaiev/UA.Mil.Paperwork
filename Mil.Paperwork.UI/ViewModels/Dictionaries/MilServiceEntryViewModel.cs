using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.DataModels;
using System;

namespace Mil.Paperwork.UI.ViewModels.Dictionaries
{
    public class MilServiceEntryViewModel : ObservableItem
    {
        private string _nominativeName;
        private string _genitiveName;

        public Guid Id { get; }

        public string NominativeName
        {
            get => _nominativeName;
            set => SetProperty(ref _nominativeName, value);
        }

        public string GenitiveName
        {
            get => _genitiveName;
            set => SetProperty(ref _genitiveName, value);
        }

        public MilServiceEntryViewModel()
        {
            Id = Guid.NewGuid();
            _nominativeName = string.Empty;
            _genitiveName = string.Empty;
        }

        public MilServiceEntryViewModel(MilServiceEntry dto)
        {
            Id = dto.Id;
            _nominativeName = dto.NominativeName;
            _genitiveName = dto.GenitiveName;
        }

        public MilServiceEntry ToDTO()
        {
            var result = new MilServiceEntry
            {
                Id = Id,
                NominativeName = NominativeName,
                GenitiveName = GenitiveName
            };
            return result;
        }
    }
}
