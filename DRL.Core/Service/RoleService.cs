using DRL.Core.Interface;
using DRL.Core.Mapper;
using DRL.Entity;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using EF = DRL.Model.Models;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRL.Model.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DRL.Core.Manager
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _RoleRepository;
        private readonly IUserRepository _UserRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper CommonHelper;
        private readonly IConfiguration _configuration;

        public RoleService(IUnitOfWork unitofwork, IRoleRepository RoleRepository, IUserRepository UserRepository, ILogManager logManager, IConfiguration configuration)
        {
            _RoleRepository = RoleRepository;
            _UserRepository = UserRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            CommonHelper = new CommonHelper();
            _configuration = configuration;
        }

        public ENTRole GetRole(long RoleId)
        {
            ENTRole result = new ENTRole();
            try
            {
                result = Configuration.Mapper.Map<ENTRole>(_RoleRepository.GetRole(RoleId));
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ProgramManager.GetRole" + ex);
            }
            return result;
        }

        public ENTRole GetRole(string roleName)
        {
            ENTRole result = new ENTRole();
            try
            {
                result = Configuration.Mapper.Map<ENTRole>(_RoleRepository.GetByWhere(x => x.IsActive && !x.IsDeleted && x.RoleName.ToLower().Contains(roleName.ToLower())).FirstOrDefault());
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ProgramManager.GetRole" + ex);
            }
            return result;
        }

        public List<ENTRole> GetAllRoles()
        {
            List<ENTRole> result = new List<ENTRole>();
            try
            {
                result = _RoleRepository.GetAllRoles().Select(p => Configuration.Mapper.Map<ENTRole>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ProgramManager.GetPrograms" + ex);
            }
            return result;
        }

        public List<ENTLookUpItem> GetActiveRoles()
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            try
            {
                result = _RoleRepository.GetActiveRoles().Select(p => Configuration.Mapper.Map<ENTLookUpItem>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ProgramManager.GetPrograms" + ex);
            }
            return result;
        }

        public ActionStatus Insert(ENTRole Role)
        {
            try
            {
                var dbRole = Configuration.Mapper.Map<ENTRole, EF.RoleMaster>(Role);
                dbRole.CreatedDate = GetDateTime.getDate();
                var resp = _RoleRepository.Insert(dbRole);
                resp.Result = Configuration.Mapper.Map(resp.Result, Role);
                return resp;
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "RoleService.Insert" + ex);
                return new ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public ActionStatus Update(ENTRole Role)
        {
            try
            {
                var dbRole = _RoleRepository.GetRole(Role.RoleId ?? 0);
                if (dbRole == null)
                    return new ActionStatus
                    {
                        Success = false,
                        Message = "Role not exist!"
                    };

                dbRole = Configuration.Mapper.Map(Role, dbRole);
                dbRole.UpdatedDate = GetDateTime.getDate();
                var result = _RoleRepository.Update(dbRole);
                result.Result = Configuration.Mapper.Map(result.Result, Role);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "RoleService.Update" + ex);
                return new ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public ActionStatus CheckRoleNameExists(string roleName, int roleID)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                result.Success = (_RoleRepository.FindBy(r => r.RoleName.Equals(roleName, StringComparison.CurrentCultureIgnoreCase) && r.RoleId != roleID && r.IsDeleted == false).Count() > 0);
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ProgramManager.CheckRoleNameExists" + ex);
            }
            return result;
        }

        public ActionStatus DeleteRole(ENTPatchRequest activeStatus)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                var dbRole = _RoleRepository.FindBy(x => x.RoleId == activeStatus.Id).FirstOrDefault();
                if (dbRole != null)
                {
                    dbRole.IsDeleted = activeStatus.status;
                    dbRole.UpdatedBy = activeStatus.UpdatedBy;
                    dbRole.UpdatedDate = GetDateTime.getDate();
                    var response = _RoleRepository.Update(dbRole);
                    return response;
                }

                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "RoleService.DeleteRole" + ex);
            }
            return result;
        }

        public List<ENTLookUpItem> GetCustomerReassignmentRoles()
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            string connString = _configuration.GetConnectionString("DefaultConnection");;
            try
            {
                result = SqlDBHelper.RawSqlQuery($"EXEC [sp_DSD_GetCustReassignRoles] ", x => new ENTLookUpItem
                {
                    RecordId = x["RoleID"].ToString(),
                    Value = x["RoleShortName"].ToString(),
                }, connString).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "RoleService.GetReassignUsers", ex);
            }
            return result;
        }

    }
}
