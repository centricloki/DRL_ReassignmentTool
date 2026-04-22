using AutoMapper;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper.Mappings
{
    public class StateMasterToENTLookUpItem : ITypeConverter<EF.StateMaster, ENTLookUpItem>
    {
        public ENTLookUpItem Convert(EF.StateMaster source, ENTLookUpItem destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new ENTLookUpItem();

            destination.RecordId = source.StateId.ToString();
            destination.Value = source.StateName;

            return destination;
        }
    }
}
