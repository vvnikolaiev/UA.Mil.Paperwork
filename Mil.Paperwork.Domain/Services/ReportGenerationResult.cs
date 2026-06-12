namespace Mil.Paperwork.Domain.Services
{
    public class ReportGenerationResult
    {
        public bool Success { get; init; }
        public IReadOnlyList<string> OutputFiles { get; init; } = [];

        public static implicit operator bool(ReportGenerationResult result)
        {
            return result?.Success ?? false;
        }

        public static ReportGenerationResult Succeeded(IReadOnlyList<string> outputFiles)
        {
            var result = new ReportGenerationResult
            {
                Success = true,
                OutputFiles = outputFiles
            };

            return result;
        }

        public static ReportGenerationResult Failed()
        {
            var result = new ReportGenerationResult
            {
                Success = false
            };

            return result;
        }

        public static ReportGenerationResult FromResult(bool success, IReadOnlyList<string> outputFiles)
        {
            var result = new ReportGenerationResult
            {
                Success = success,
                OutputFiles = outputFiles
            };

            return result;
        }
    }
}
