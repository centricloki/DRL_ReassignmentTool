using DRL.Entity;
using EF = DRL.Model.Models;
using AutoMapper;

namespace DRL.Core.Mapper.Mappings
{
    public class TerritoryMasterToENTTeam : ITypeConverter<EF.TerritoryMaster, ENTTeam>
    {
        public ENTTeam Convert(EF.TerritoryMaster source, ENTTeam destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new ENTTeam();

            destination.TeamId = source.TerritoryId;
            destination.Name = source.TerritoryName;
            destination.RegionId = source.RegionId;
            destination.CreatedDate = source.CreatedDate;
            destination.UpdateDate = source.UpdateDate;
            destination.IsActive = source.IsActive;
            destination.IsDeleted = source.IsDeleted;
            destination.Description = source.Description;
            destination.CreatedBy = source.CreatedBy;
            destination.UpdatedBy = source.UpdatedBy;
            destination.BDID = source.BDID;
            return destination;
        }
    }
}