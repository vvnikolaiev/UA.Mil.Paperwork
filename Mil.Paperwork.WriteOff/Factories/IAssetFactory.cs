using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.WriteOff.ViewModels;

namespace Mil.Paperwork.WriteOff.Factories
{
    public interface IAssetFactory
    {
        AssetViewModel CreateAssetViewModel();
    }
}
