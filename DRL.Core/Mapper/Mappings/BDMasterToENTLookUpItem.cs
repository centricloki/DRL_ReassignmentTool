using AutoMapper;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper.Mappings
{
    public class BDMasterToENTLookUpItem : ITypeConverter<EF.BDMaster, ENTLookUpItem>
    {
        public ENTLookUpItem Convert(EF.BDMaster source, ENTLookUpItem destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new ENTLookUpItem();

            destination.RecordId = source.BDID.ToString();
            destination.Value = source.BDName;

            return destination;
        }
    }
}
