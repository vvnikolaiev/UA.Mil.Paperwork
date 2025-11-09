using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Domain.Helpers
{
    public static class ReportParametersHelper
    {
        private const string PARAM_NAME = "NAME";
        private const string PARAM_RANK = "RANK";
        private const string PARAM_POSITION = "POSITION";
        private const string PARAM_NAME_ACCUSATION = "ACCUS";
        private const string DEFAULT_CommissionHeadPattern = "COMMISSION_HEAD_{0}";
        private const string DEFAULT_CommissionPersonPattern = "COMMISSION_PERSON{0}_{1}";
        
        public static Dictionary<string, string> GetFullParametersDictionary(ReportType reportType, IReportDataService reportDataService)
        {
            var reportConfig = reportDataService.GetReportParametersDictionary(reportType);
            var commonConfig = reportDataService.GetReportParametersDictionary(ReportType.Common);

            var fullDict = new Dictionary<string, string>(reportConfig);
            foreach (var commonRow in commonConfig)
            {
                if (!fullDict.ContainsKey(commonRow.Key))
                {
                    fullDict.Add(commonRow.Key, commonRow.Value);
                }
            }

            return fullDict;
        }

        public static Dictionary<string, string> GetCommission(ReportType reportType, IReportDataService reportDataService)
        {
            var commission = reportDataService.GetCommission(reportType);
            var config = reportDataService.GetCommissionsConfig();

            var commHeadPattern = config?.HeadOfCommissionPattern ?? DEFAULT_CommissionHeadPattern;
            var commPersonPattern = config?.CommissionPersonPattern ?? DEFAULT_CommissionPersonPattern;

            var dictCommissionFields = new Dictionary<string, string>();

            var commCount = commission.Squad?.Count ?? 0;
            if (commCount == 0)
                return [];

            for (int i = 0; i < commCount; i++)
            {
                var person = commission.Squad[i];

                if (i == 0)
                {
                    dictCommissionFields.Add(string.Format(commHeadPattern, PARAM_NAME), person.Name);
                    dictCommissionFields.Add(string.Format(commHeadPattern, PARAM_POSITION), person.Position);
                    dictCommissionFields.Add(string.Format(commHeadPattern, PARAM_RANK), person.Rank);
                    dictCommissionFields.Add(string.Format(commHeadPattern, PARAM_NAME_ACCUSATION), person.NameAccusationForm);
                }
                else
                {
                    dictCommissionFields.Add(string.Format(commPersonPattern, i, PARAM_NAME), person.Name);
                    dictCommissionFields.Add(string.Format(commPersonPattern, i, PARAM_POSITION), person.Position);
                    dictCommissionFields.Add(string.Format(commPersonPattern, i, PARAM_RANK), person.Rank);
                    dictCommissionFields.Add(string.Format(commPersonPattern, i, PARAM_NAME_ACCUSATION), person.NameAccusationForm);
                }
            }

            return dictCommissionFields;
        }
    }
}
