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

        public int? WindowPositionX { get; set; }

        public int? WindowPositionY { get; set; }

        public double? WindowWidth { get; set; }

        public double? WindowHeight { get; set; }

        public bool WindowMaximized { get; set; }

        public bool IsSidebarCollapsed { get; set; }
    }
}
