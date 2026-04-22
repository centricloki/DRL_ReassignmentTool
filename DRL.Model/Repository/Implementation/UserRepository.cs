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
    public class UserRepository : GenericRepository<EF.UserMaster>, IUserRepository
    {
        private readonly ILogger logger;

        public UserRepository(IUnitOfWork unitOfWork, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IUserRepository));
        }

        public EF.UserMaster GetUser(long userId)
        {
            var result = new EF.UserMaster();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "UserRepository.GetUser");
                result = base.FindByNoTracking(f => f.UserId == userId).SingleOrDefault();
                logger.Info(Constants.ACTION_EXIT, "UserRepository.GetUser");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public List<EF.UserMaster> GetAllUsers()
        {
            var result = new List<EF.UserMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "UserRepository.GetAllUsers");
                result = base.GetAll().Where(x => x.IsInActive == false && x.IsDeleted == false).OrderBy(x => x.FirstName).ToList();
                logger.Info(Constants.ACTION_EXIT, "UserRepository.GetAllUsers");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public List<EF.UserMaster> GetActiveUsers()
        {
            var result = new List<EF.UserMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "UserRepository.GetActiveUsers");
                result = base.GetAll().Where(x => x.IsInActive == false).ToList();
                logger.Info(Constants.ACTION_EXIT, "UserRepository.GetActiveUsers");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public List<EF.UserMaster> GetAllUsersByRoleId(Int32 RoleId)
        {
            var result = new List<EF.UserMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "UserRepository.GetAllUsers");
                result = base.GetAll().Where(x => x.IsDeleted == false && x.RoleId == RoleId).ToList();
                logger.Info(Constants.ACTION_EXIT, "UserRepository.GetAllUsers");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public List<EF.UserMaster> GetAllUsersByManagerId(Int32 ManagerId)
        {
            var result = new List<EF.UserMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "UserRepository.GetAllUsersByManagerId");
                result = base.GetAll().Where(x => x.IsDeleted == false && x.ManagerId == ManagerId).ToList();
                logger.Info(Constants.ACTION_EXIT, "UserRepository.GetAllUsersByManagerId");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}
