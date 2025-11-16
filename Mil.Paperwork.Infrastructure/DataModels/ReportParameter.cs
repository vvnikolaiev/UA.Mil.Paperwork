namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class ReportParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }

        public ReportParameter()
        {
        }

        public ReportParameter(string name) : this()
        {
            Name = name;
        }
    }
}
