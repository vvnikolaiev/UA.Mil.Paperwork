using Microsoft.Extensions.DependencyInjection;
using Mil.Paperwork.DataAccess.Conversions;
using Mil.Paperwork.DataAccess.Repositories;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.DataAccess
{
    public static class DataAccessServicesRegistrator
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IDataService, DataService>();
            services.AddSingleton<IReportDataService, ReportDataService>();
            services.AddSingleton<IReportHistoryRepository, JsonReportHistoryRepository>();
            services.AddSingleton<IReportHistoryService, ReportHistoryService>();

            services.AddSingleton<IReportDataConversion, InvoiceToCommissioningActConversion>();
            services.AddSingleton<IReportDataConversion, InvoiceToInitialTechnicalStateConversion>();
            services.AddSingleton<IReportDataConversion, ResidualValueToWriteOffPackageConversion>();
            services.AddSingleton<IReportDataConversion, Handover23ToInvoiceConversion>();
            services.AddSingleton<IReportDataConversion, InvoiceToHandover23Conversion>();
            services.AddSingleton<IReportDataConversion, CommissioningActToInvoiceConversion>();
            services.AddSingleton<IReportDataConversion, CommissioningActToInitialTechnicalStateConversion>();
            services.AddSingleton<IReportDataConversion, WriteOffOrderToEASConversion>();
            services.AddSingleton<IReportDataConversion, WriteOffPackageToEASConversion>();
            services.AddSingleton<ReportConversionRegistry>();
        }
    }
}
