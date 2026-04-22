using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;

using System;
using System.Collections.Generic;
using System.Linq;

using EF = DRL.Model.Models;

namespace DRL.Model.Repository.Implementation
{
    public class TerritoryRepository : GenericRepository<EF.TerritoryMaster>, ITerritoryRepository
    {
        private readonly ILogger logger;

        public TerritoryRepository(IUnitOfWork unitOfWork, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IRoleRepository));
        }

        public List<EF.TerritoryMaster> GetAllTerritory()
        {
            var result = new List<EF.TerritoryMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "TerritoryRepository.GetAllTerritory");
                //result = base.GetAll().Where(x => (x.IsDeleted == false && x.IsActive == true && x.RegionId != 0)).OrderBy(x => x.TerritoryName).ToList();//Updated by Senthil Ramadoss on 5/13/2020
                result = base.GetAllNoTracking().Where(x => (x.IsDeleted == false && x.IsActive == true)).OrderBy(x => x.TerritoryName).ToList();
                logger.Info(Constants.ACTION_EXIT, "TerritoryRepository.GetAllTerritory");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }


        public List<EF.TerritoryMaster> GetAllTerritoryByUserId(long UserId)
        {
            var result = new List<EF.TerritoryMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "TerritoryRepository.GetAllTerritoryByUserId");
                var user = _uow.DbContext.UserMaster.Where(u => u.UserId == UserId).FirstOrDefault();
                if (user != null)
                {
                    List<string> teams = user.TerritoryId.Split(',').ToList();

                    result = (from t in _uow.DbContext.TerritoryMaster
                              join u in teams on t.TerritoryId.ToString() equals u.Trim()
                              select t).ToList();
                }
                logger.Info(Constants.ACTION_EXIT, "TerritoryRepository.GetAllTerritoryByUserId");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public List<EF.TerritoryMaster> GetAllTerritories()
        {
            var result = new List<EF.TerritoryMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "TerritoryRepository.GetAllTerritories");
                result = base.GetAll().Where(x => x.IsActive && x.IsDeleted == false).OrderByDescending(x => x.UpdateDate).ToList();
                logger.Info(Constants.ACTION_EXIT, "TerritoryRepository.GetAllTerritories");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public EF.TerritoryMaster GetTerritory(long TeamId)
        {
            var result = new EF.TerritoryMaster();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "TerritoryRepository.GetTerritory");
                result = base.FindBy(f => f.TerritoryId == TeamId).SingleOrDefault();
                logger.Info(Constants.ACTION_EXIT, "TerritoryRepository.GetTerritory");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public List<EF.TerritoryMaster> GetCustReassignTerritoriesByRoleIds(string[] RoleId)
        {
            var result = new List<EF.TerritoryMaster>();
            try
            {
                bool isRoleIdEmpty =  (RoleId?.Length == 1 && RoleId[0].ToLower().Contains("null"));
                string RoleIds = String.Join(',', RoleId);
                logger.Info(Constants.ACTION_ENTRY, "TerritoryRepository.GetCustReassignTerritoriesByRoleIds");
                result = (from t in _uow.DbContext.TerritoryMaster
                          join u in _uow.DbContext.UserMaster
                          on t.TerritoryId equals u.DefTerritoryId
                          join r in _uow.DbContext.RoleMaster
                          on u.RoleId equals r.RoleId
                          where (u.IsDeleted == false && u.IsInActive == false && u.DefTerritoryId > 0)
                          && (r.IsDeleted == false && r.IsActive == true)
                          && (t.IsDeleted == false && t.IsActive == true)
                          //&& (("," + RoleIds + ",").IndexOf("," + r.RoleId + ",") > -1)
                          && (!isRoleIdEmpty ? (("," + RoleIds + ",").IndexOf("," + r.RoleId + ",") > -1) : true)
                          select t).Distinct().OrderBy(x => x.TerritoryName).ToList();


                logger.Info(Constants.ACTION_EXIT, "TerritoryRepository.GetCustReassignTerritoriesByRoleIds");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public List<EF.TerritoryMaster> GetCustReassignTeamByRoleIds(string[] RoleId)
        {
            var result = new List<EF.TerritoryMaster>();
            try
            {
                string RoleIds = String.Join(',', RoleId);
                logger.Info(Constants.ACTION_ENTRY, "TerritoryRepository.GetCustReassignTerritoriesByRoleIds");
                result = (from t in _uow.DbContext.TerritoryMaster
                          join u in _uow.DbContext.UserMaster
                          on t.TerritoryId equals u.DefTerritoryId
                          join r in _uow.DbContext.RoleMaster
                          on u.RoleId equals r.RoleId
                          where u.IsDeleted == false && r.IsDeleted == false && r.IsActive == true && t.IsDeleted == false && t.IsActive == true && u.IsInActive == false
                          && !r.RoleName.Equals("Broker", StringComparison.CurrentCultureIgnoreCase)
                          && !r.RoleName.Equals("Inside Sales", StringComparison.CurrentCultureIgnoreCase)
                          && ((("," + RoleIds + ",").IndexOf("," + r.RoleId + ",") > -1) || RoleId[0] == "null")
                          //&& RoleId.ToList().Contains(r.RoleId.ToString())
                          select t).Distinct().OrderBy(x => x.TerritoryName).ToList();


                logger.Info(Constants.ACTION_EXIT, "TerritoryRepository.GetCustReassignTerritoriesByRoleIds");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}
