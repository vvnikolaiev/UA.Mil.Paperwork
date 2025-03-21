namespace Mil.Paperwork.WriteOff.ViewModels
{
    public interface ITabViewModel
    {
        event EventHandler<ITabViewModel> TabCloseRequested;

        string Header { get; }
    }
}
