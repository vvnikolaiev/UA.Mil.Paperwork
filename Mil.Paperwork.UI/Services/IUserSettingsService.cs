using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.UI.Services
{
    public interface IUserSettingsService
    {
        UserSettingsDTO GetSettings();

        void SaveSettings(UserSettingsDTO settings);
    }
}
