using Microsoft.Extensions.DependencyInjection;

namespace Mil.Paperwork.WriteOff.Configuration
{
    public interface IServiceConfigurator
    {
        void ConfigureServices(IServiceCollection services);
    }
}
