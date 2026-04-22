using AutoMapper;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper.Mappings
{
    public class AVPMasterToENTLookUpItem : ITypeConverter<EF.AVPMaster, ENTLookUpItem>
    {
        public ENTLookUpItem Convert(EF.AVPMaster source, ENTLookUpItem destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new ENTLookUpItem();

            destination.RecordId = source.AVPID.ToString();
            destination.Value = source.AVPName;

            return destination;
        }
    }
}
