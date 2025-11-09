namespace Mil.Paperwork.Infrastructure.DataModels.Configuration
{
    public class CommissionDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        // head of a commission is the first person by default?
        public List<CommissionerDTO> Squad { get; set; }

        public CommissionDTO()
        {
            Squad = [];
        }
    }
}
