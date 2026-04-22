using AutoMapper;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper.Mappings
{
    public class ZoneMasterToENTLookUpItem : ITypeConverter<EF.ZoneMaster, ENTLookUpItem>
    {
        public ENTLookUpItem Convert(EF.ZoneMaster source, ENTLookUpItem destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new ENTLookUpItem();

            destination.RecordId = source.ZoneId.ToString();
            destination.Value = source.ZoneName;

            return destination;
        }
    }
}
