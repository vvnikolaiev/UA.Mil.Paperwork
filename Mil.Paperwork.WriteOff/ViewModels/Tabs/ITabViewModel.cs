namespace Mil.Paperwork.WriteOff.ViewModels.Tabs
{
    public interface ITabViewModel
    {
        event EventHandler<ITabViewModel> TabCloseRequested;

        string Header { get; }
    }
}
