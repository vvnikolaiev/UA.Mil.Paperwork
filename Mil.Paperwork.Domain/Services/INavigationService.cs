namespace Mil.Paperwork.Domain.Services
{
    public interface INavigationService
    {
        void OpenWindow<TViewModel>() where TViewModel : class;
    }
}
