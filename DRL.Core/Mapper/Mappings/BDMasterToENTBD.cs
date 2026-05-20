using AutoMapper;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper.Mappings
{
    public class BDMasterToENTBD : ITypeConverter<EF.BDMaster, ENTBD>
    {
        public ENTBD Convert(EF.BDMaster source, ENTBD destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new ENTBD();

            destination.BDID = source.BDID;
            destination.BDName = source.BDName;
            destination.UpdateDate = source.UpdateDate;
            destination.CreatedDate = source.CreatedDate;
            destination.CreatedBy = source.CreatedBy;
            destination.UpdatedBy = source.UpdatedBy;
            destination.IsActive = source.IsActive;
            destination.IsDeleted = source.IsDeleted;
            destination.Approver = source.Approver;

            return destination;
        }
    }
}
