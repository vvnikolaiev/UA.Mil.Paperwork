using Microsoft.Extensions.DependencyInjection;

namespace Mil.Paperwork.UI.Configuration
{
    public interface IServiceConfigurator
    {
        void ConfigureServices(IServiceCollection services);
    }
}
