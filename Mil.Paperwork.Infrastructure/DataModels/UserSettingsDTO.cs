using Mil.Paperwork.Infrastructure.Enums;
using System.Collections.Generic;

namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class UserSettingsDTO
    {
        public const string ThemeAuto = "Auto";
        public const string ThemeLight = "Light";
        public const string ThemeDark = "Dark";

        public string Theme { get; set; } = ThemeAuto;

        public List<ReportType> FavoriteReportTypes { get; set; } = [];

        public List<ReportType> RecentReportTypes { get; set; } = [];
    }
}
