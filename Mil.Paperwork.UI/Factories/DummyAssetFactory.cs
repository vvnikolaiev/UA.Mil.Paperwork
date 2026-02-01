using Mil.Paperwork.UI.ViewModels.Assets;

namespace Mil.Paperwork.UI.Factories
{
    internal class DummyAssetFactory : IAssetFactory
    {
        public WriteOffAssetViewModel CreateAssetViewModel()
        {
            var result = new DefaultAssetInfoViewModel();
            return result;
        }
    }
}
