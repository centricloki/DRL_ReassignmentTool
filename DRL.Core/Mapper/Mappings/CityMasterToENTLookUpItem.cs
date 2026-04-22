using AutoMapper;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper.Mappings
{
    public class CityMasterToENTLookUpItem : ITypeConverter<EF.CityMaster, ENTLookUpItem>
    {
        public ENTLookUpItem Convert(EF.CityMaster source, ENTLookUpItem destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new ENTLookUpItem();

            destination.RecordId = source.CityId.ToString();
            destination.Value = source.CityName;

            return destination;
        }
    }
}
