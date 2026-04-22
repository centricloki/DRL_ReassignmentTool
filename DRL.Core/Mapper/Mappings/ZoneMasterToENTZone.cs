using AutoMapper;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper.Mappings
{
    public class ZoneMasterToENTZone : ITypeConverter<EF.ZoneMaster, ENTZone>
    {
        public ENTZone Convert(EF.ZoneMaster source, ENTZone destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new ENTZone();

            destination.ZoneId = source.ZoneId;
            destination.ZoneName = source.ZoneName;
            destination.UpdateDate = source.UpdateDate;
            destination.SugarZoneId = source.SugarZoneId;
            destination.ImportedFrom = source.ImportedFrom;
            destination.AVPID = source.AVPID;
            destination.IsActive = source.IsActive;
            destination.IsDeleted = source.IsDeleted;

            return destination;
        }
    }
}
