using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.Services;
using System;

namespace Mil.Paperwork.UI.Services
{
    internal class UserSettingsService : IUserSettingsService
    {
        private readonly IFileStorageService _fileStorageService;

        private UserSettingsDTO? _settings;

        public UserSettingsService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public UserSettingsDTO GetSettings()
        {
            if (_settings == null)
            {
                _settings = LoadSettings();
            }

            return _settings;
        }

        public void SaveSettings(UserSettingsDTO settings)
        {
            _settings = settings;

            // The (fileName, directory) overload resolves against the app base directory —
            // the same root ReadJsonFile uses; the (filePath) overload would write to the CWD.
            _fileStorageService.WriteJsonToFile(settings, LocalDataPaths.UserSettings, directory: null);
        }

        private UserSettingsDTO LoadSettings()
        {
            UserSettingsDTO? settings = null;

            try
            {
                settings = _fileStorageService.ReadJsonFile<UserSettingsDTO>(LocalDataPaths.UserSettings);
            }
            catch (Exception)
            {
                // missing or corrupted settings file — fall back to defaults
            }

            var result = settings ?? new UserSettingsDTO();
            return result;
        }
    }
}
