using AutoMapper;
using DRL.Entity;
using EF = DRL.Model.Models;

namespace DRL.Core.Mapper.Mappings
{
    public class ENTBDToBDMaster : ITypeConverter<ENTBD, EF.BDMaster>
    {
        public EF.BDMaster Convert(ENTBD source, EF.BDMaster destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new EF.BDMaster();

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
