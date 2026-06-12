using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.DataAccess.Conversions
{
    internal static class ConversionHelper
    {
        public static IAssetInfo ToAssetInfo(ICommissioningActReportData actData)
        {
            var asset = actData.Asset as IAssetInfo ?? new AssetInfo(actData.Asset);

            if (string.IsNullOrEmpty(asset.SerialNumber))
            {
                asset.SerialNumber = actData.AssetIds?.FirstOrDefault()?.SerialNumber ?? string.Empty;
            }

            asset.Count = actData.Count;

            return asset;
        }

        public static PersonDTO ToPersonDTO(IPerson person)
        {
            if (person is PersonDTO personDTO)
            {
                return personDTO;
            }

            if (person == null)
            {
                return new PersonDTO();
            }

            var result = new PersonDTO
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Patronymic = person.Patronymic,
                Position = person.Position,
                Rank = person.Rank
            };

            return result;
        }
    }
}
