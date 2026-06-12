using Mil.MVVM.Common;
using Mil.Paperwork.Domain.DataModels;

namespace Mil.Paperwork.UI.ViewModels.Reports
{
    internal class WriteOffWitnessViewModel : ObservableItem
    {
        private string _rank = string.Empty;
        private string _name = string.Empty;
        private string _position = string.Empty;

        public string Rank { get => _rank; set => SetProperty(ref _rank, value); }
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Position { get => _position; set => SetProperty(ref _position, value); }

        public static WriteOffWitnessViewModel FromWitnessData(WriteOffWitnessData data)
        {
            var result = new WriteOffWitnessViewModel
            {
                Rank = data.Rank,
                Name = data.Name,
                Position = data.Position
            };
            return result;
        }

        public WriteOffWitnessData ToWitnessData()
        {
            var result = new WriteOffWitnessData
            {
                Rank = Rank,
                Name = Name,
                Position = Position
            };
            return result;
        }
    }
}
