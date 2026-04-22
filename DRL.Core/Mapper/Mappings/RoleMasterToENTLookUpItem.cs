using AutoMapper;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper.Mappings
{
    public class RoleMasterToENTLookUpItem : ITypeConverter<EF.RoleMaster, ENTLookUpItem>
    {
        public ENTLookUpItem Convert(EF.RoleMaster source, ENTLookUpItem destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new ENTLookUpItem();

            destination.RecordId = source.RoleId.ToString();
            destination.Value = source.RoleName;
            
            return destination;
        }
    }
}
