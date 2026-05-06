using AutoMapper;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper.Mappings
{
    public class ENTZoneToZoneMaster : ITypeConverter<ENTZone, EF.ZoneMaster>
    {
        public EF.ZoneMaster Convert(ENTZone source, EF.ZoneMaster destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new EF.ZoneMaster();

            destination.ZoneId = source.ZoneId;
            destination.ZoneName = source.ZoneName;
            destination.UpdateDate = source.UpdateDate;
            destination.SugarZoneId = source.SugarZoneId;
            destination.ImportedFrom = source.ImportedFrom;
            destination.AVPID = source.AVPID;
            destination.IsActive = source.IsActive;
            destination.IsDeleted = source.IsDeleted;
            destination.CreatedDate = source.CreatedDate;
            destination.CreatedBy = source.CreatedBy;
            destination.UpdatedBy = source.UpdatedBy;

            return destination;
        }
    }
}