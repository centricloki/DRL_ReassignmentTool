using DRL.Entity;
using EF = DRL.Model.Models;
using AutoMapper;

namespace DRL.Core.Mapper.Mappings
{
    public class UserToENTUser : ITypeConverter<EF.UserMaster, ENTUser>
    {
        public ENTUser Convert(EF.UserMaster source, ENTUser destination, ResolutionContext context)
        {
            if (source == null) return null;

            if (destination == null) destination = new ENTUser();

            destination.UserId = source.UserId;
            destination.FirstName = source.FirstName;
            destination.LastName = source.LastName;
            destination.Email = source.EmailId;
            destination.UserName = source.UserName;
            destination.Pin = source.Pin;
            destination.RoleId = source.RoleId;
            destination.DefaultTeamId = source.DefTerritoryId;
            destination.ManagerId = source.ManagerId;
            //destination.RoleName = source.Role.RoleName;
            destination.IsActive = !source.IsInActive;
            destination.IsDeleted = source.IsDeleted;
            destination.CreatedBy = source.CreatedBy;
            destination.CreatedDate = source.CreatedDate;
            destination.UpdatedBy = source.UpdatedBy;
            destination.UpdatedDate = source.UpdatedDate;
            destination.TerritoryId = source.TerritoryId;
            destination.AVPID = source.AVPID;
            destination.BDID = source.BDID;
            return destination;
        }
    }
}