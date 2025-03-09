using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.WriteOff.Managers;
using System.Windows.Input;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    public class MainViewModel : ObservableItem
    {
        private readonly ReportManager _reportManager;
        private readonly INavigationService _navigationService;
        
        private WriteOffReportViewModel _writeOffReportViewModel;

        public ICommand OpenOtherWindowCommand { get; }
        public WriteOffReportViewModel WriteOffReportViewModel
        {
            get => _writeOffReportViewModel;
            set => SetProperty(ref _writeOffReportViewModel, value);
        }

        public MainViewModel(ReportManager reportManager, IDataService dataService, INavigationService navigationService)
        {
            _reportManager = reportManager;
            _navigationService = navigationService;
            OpenOtherWindowCommand = new DelegateCommand(OpenOtherWindow);

            WriteOffReportViewModel = new WriteOffReportViewModel(reportManager, dataService);
        }

        private void OpenOtherWindow()
        {
            //_navigationService.OpenWindow<OtherViewModel>();
        }
    }
}
