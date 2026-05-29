using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Helpers;

namespace Mil.Paperwork.Domain.Reports.WriteOff
{
    internal abstract class WriteOffPackageReport : IWriteOffPackageReport
    {
        private byte[] _reportBytes;

        public abstract string OutputFileName { get; }

        protected abstract string TemplatePath { get; }

        public bool TryCreate(IWriteOffPackageParameters reportParameters)
        {
            try
            {
                using var document = WordDocument.LoadFromFile(TemplatePath);

                FillReportData(reportParameters, document);

                _reportBytes = document.GetBytes();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public byte[] GetReportBytes()
        {
            return _reportBytes;
        }

        protected abstract void FillReportData(IWriteOffPackageParameters reportParameters, WordDocument document);
    }
}
