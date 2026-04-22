using DRL.Entity;
using EF = DRL.Model.Models;
using AutoMapper;

namespace DRL.Core.Mapper.Mappings
{
    public class ENTUserToUser : ITypeConverter<ENTUser, EF.UserMaster>
    {
        public EF.UserMaster Convert(ENTUser source, EF.UserMaster destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new EF.UserMaster();

            destination.UserId = source.UserId??0;
            destination.EmailId = source.Email;
            destination.FirstName = source.FirstName;
            destination.LastName = source.LastName;
            destination.UserName = source.UserName;
            destination.Pin = source.Pin;
            destination.RoleId = source.RoleId;
            destination.ManagerId = source.ManagerId;
            destination.DefTerritoryId = source.DefaultTeamId;
            destination.TerritoryId = source.TerritoryId;
            destination.IsInActive = source.IsActive;
            destination.IsDeleted = source.IsDeleted;
            destination.CreatedBy = source.CreatedBy;
            destination.CreatedDate = source.CreatedDate;
            destination.UpdatedBy = source.UpdatedBy;
            destination.UpdatedDate = source.UpdatedDate.HasValue ? source.UpdatedDate.Value : System.DateTime.UtcNow;
            destination.AVPID = source.AVPID;
            destination.BDID = source.BDID;
            return destination;
        }
    }
}