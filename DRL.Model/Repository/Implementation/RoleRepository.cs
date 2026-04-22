using DRL.Framework.Log.Interface;
using EF = DRL.Model.Models;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;
using DRL.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DRL.Model.Repository.Implementation
{
    public class RoleRepository : GenericRepository<EF.RoleMaster>, IRoleRepository
    {
        private readonly ILogger logger;

        public RoleRepository(IUnitOfWork unitOfWork, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IRoleRepository));
        }

        public EF.RoleMaster GetRole(long RoleId)
        {
            var result = new EF.RoleMaster();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "RoleRepository.GetRole");
                result = base.FindByNoTracking(f => f.RoleId == RoleId).SingleOrDefault();
                logger.Info(Constants.ACTION_EXIT, "RoleRepository.GetRole");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public List<EF.RoleMaster> GetAllRoles()
        {
            var result = new List<EF.RoleMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "RoleRepository.GetAllRoles");
                result = base.GetAllNoTracking().Where(x => x.IsDeleted == false).ToList();
                logger.Info(Constants.ACTION_EXIT, "RoleRepository.GetAllRoles");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public List<EF.RoleMaster> GetActiveRoles()
        {
            var result = new List<EF.RoleMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "RoleRepository.GetActiveRoles");
                result = base.GetAllNoTracking().Where(x => x.IsActive == true && x.IsDeleted == false).OrderBy(x=>x.RoleName).ToList();
                logger.Info(Constants.ACTION_EXIT, "RoleRepository.GetActiveRoles");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

    }
}