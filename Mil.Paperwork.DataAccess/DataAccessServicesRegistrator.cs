using Microsoft.Extensions.DependencyInjection;
using Mil.Paperwork.DataAccess.Repositories;
using Mil.Paperwork.DataAccess.Services;

namespace Mil.Paperwork.DataAccess
{
    public static class DataAccessServicesRegistrator
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IDataService, DataService>();
            services.AddSingleton<IReportHistoryRepository, JsonReportHistoryRepository>();
            services.AddSingleton<IReportHistoryService, ReportHistoryService>();
        }
    }
}
