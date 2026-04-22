using DRL.Entity;
using EF = DRL.Model.Models;
using AutoMapper;

namespace DRL.Core.Mapper.Mappings
{
    public class ENTTeamToTerritoryMaster : ITypeConverter<ENTTeam, EF.TerritoryMaster>
    {
        public EF.TerritoryMaster Convert(ENTTeam source, EF.TerritoryMaster destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new EF.TerritoryMaster();

            destination.TerritoryId = source.TeamId ?? 0;
            destination.TerritoryName = source.Name;
            destination.RegionId = source.RegionId ?? 0;
            destination.Description = source.Description;
            destination.IsActive = source.IsActive;
            destination.IsDeleted = source.IsDeleted;
            destination.CreatedBy = source.CreatedBy;
            destination.UpdatedBy = source.UpdatedBy;
            destination.CreatedDate = source.CreatedDate;
            destination.UpdateDate = source.UpdateDate.HasValue ? source.UpdateDate.Value : System.DateTime.UtcNow;
            destination.BDID = source.BDID;
            return destination;
        }
    }
}