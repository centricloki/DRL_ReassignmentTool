using AutoMapper;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper.Mappings
{
    public class TerritoryMasterToENTLookUpItem : ITypeConverter<EF.TerritoryMaster, ENTLookUpItem>
    {
        public ENTLookUpItem Convert(EF.TerritoryMaster source, ENTLookUpItem destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new ENTLookUpItem();

            destination.RecordId = source.TerritoryId.ToString();
            destination.Value = source.TerritoryName;

            return destination;
        }
    }
}
