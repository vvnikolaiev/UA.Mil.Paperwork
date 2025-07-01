﻿using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.DataModels;
using Mil.Paperwork.WriteOff.Helpers;

namespace Mil.Paperwork.WriteOff.Strategies
{
    internal class PeopleImportStrategy : ImportStrategy<PersonDTO>
    {
        private readonly IDataService _dataService;

        public PeopleImportStrategy(IDataService dataService)
        {
            _dataService = dataService;
        }

        public override ImportDataResult ImportData(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows)
        {
            ImportDataResult result;
            try
            {
                var validColumns = FilterValidColumns(columns);
                var dtos = ImportHelper.MapRowsToDtos<PersonDTO>(validColumns, rows);
                _dataService.AlterPeople(dtos);

                result = new ImportDataResult(true, rows.Count)
                {
                    InvalidRowsCount = rows.Count - dtos.Count
                };
            }
            catch (Exception ex)
            {
                result = new ImportDataResult(false)
                {
                    ErrorMessage = ex.Message
                };
            }

            return result;
        }
    }
}
