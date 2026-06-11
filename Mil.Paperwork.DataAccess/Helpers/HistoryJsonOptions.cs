using System.Text.Json;

namespace Mil.Paperwork.DataAccess.Helpers
{
    public static class HistoryJsonOptions
    {
        public static JsonSerializerOptions Default { get; } = CreateDefaultOptions();

        private static JsonSerializerOptions CreateDefaultOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return options;
        }
    }
}
