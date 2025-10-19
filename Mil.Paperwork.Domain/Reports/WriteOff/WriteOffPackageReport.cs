using Mil.Paperwork.Domain.DataModels.Parameters;
using Spire.Doc;
using System.IO;

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
                var templatePath = TemplatePath;

                var document = new Document();
                document.LoadFromFile(templatePath, FileFormat.Docx);

                FillReportData(reportParameters, document);

                using var reportStream = new MemoryStream();
                document.SaveToStream(reportStream, FileFormat.Docx);
                _reportBytes = reportStream.ToArray();

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

        protected abstract void FillReportData(IWriteOffPackageParameters reportParameters, Document document);
    }
}
