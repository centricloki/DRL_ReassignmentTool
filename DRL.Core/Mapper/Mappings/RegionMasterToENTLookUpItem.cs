using AutoMapper;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper.Mappings
{
    public class RegionMasterToENTLookUpItem : ITypeConverter<EF.RegionMaster, ENTLookUpItem>
    {
        public ENTLookUpItem Convert(EF.RegionMaster source, ENTLookUpItem destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new ENTLookUpItem();

            destination.RecordId = source.RegionId.ToString();
            destination.Value = source.Regioname;
            destination.Code = source.ZoneId.ToString();

            return destination;
        }
    }
}
