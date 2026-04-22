using DRL.Core.Interface;
using DRL.Core.Mapper;
using DRL.Entity;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using DRL.Model.DataBase;
using DRL.Model.Repository.Implementation;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;

using EF = DRL.Model.Models;

namespace DRL.Core.Service
{
    public class TerritoryService : ITerritoryService
    {
        private readonly ITerritoryRepository _territoryRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper CommonHelper;
        private readonly IConfiguration _configuration;

        public TerritoryService(IUnitOfWork unitofwork, ITerritoryRepository territoryRepository, ILogManager logManager, IConfiguration configuration)
        {
            _territoryRepository = territoryRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            CommonHelper = new CommonHelper();
            _configuration = configuration;
        }

        public List<ENTLookUpItem> GetAllTerritoryLookup()
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            try
            {
                result = _territoryRepository.GetAllTerritory().Select(p => Configuration.Mapper.Map<ENTLookUpItem>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.GetAllTerritoryLookup" + ex);
            }
            return result;
        }

        public List<ENTLookUpItem> GetAllTerritoryLookupByUserId(long UserId)
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            try
            {
                result = _territoryRepository.GetAllTerritoryByUserId(UserId).Select(p => Configuration.Mapper.Map<ENTLookUpItem>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.GetAllTerritoryLookupByUserId" + ex);
            }
            return result;
        }

        public List<ENTTeam> GetAllTerritories()
        {
            List<ENTTeam> result = new List<ENTTeam>();
            try
            {
                result = _territoryRepository.GetAllTerritories().Select(p => Configuration.Mapper.Map<ENTTeam>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.GetAllTerritories" + ex);
            }
            return result;
        }

        public List<ENTTeam> GetAllUserTerritories(long userId)
        {
            List<ENTTeam> result = new List<ENTTeam>();
            try
            {
                result = _territoryRepository.GetAllTerritoryByUserId(userId).Select(p => Configuration.Mapper.Map<ENTTeam>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.GetAllTerritories" + ex);
            }
            return result;
        }

        public List<ENTTeam> GetAllBDTerritories(int bdId)
        {
            List<ENTTeam> result = new List<ENTTeam>();
            try
            {
                result = _territoryRepository.GetAll().Where(x => x.IsActive && !x.IsDeleted && x.BDID.HasValue && x.BDID.Value == bdId).Select(p => Configuration.Mapper.Map<ENTTeam>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.GetAllTerritories" + ex);
            }
            return result;
        }

        public ActionStatus Insert(ENTTeam Team)
        {
            try
            {
                var dbTeam = Configuration.Mapper.Map<ENTTeam, EF.TerritoryMaster>(Team);
                dbTeam.CreatedDate = GetDateTime.getDate();
                var resp = _territoryRepository.Insert(dbTeam);
                resp.Result = Configuration.Mapper.Map(resp.Result, Team);
                return resp;
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.Insert" + ex);
                return new ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public ENTTeam GetTerritory(long TeamId)
        {
            ENTTeam result = new ENTTeam();
            try
            {
                result = Configuration.Mapper.Map<ENTTeam>(_territoryRepository.GetTerritory(TeamId));
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.GetTerritory" + ex);
            }
            return result;
        }

        private ActionStatus TerritoryUpdateAndCorrectInCustomerMaster(ENTTeam Team)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                string connString = _configuration.GetConnectionString("DefaultConnection");
                {
                    List<SqlParameter> sqlParameters = new List<SqlParameter>()
                        {
                            new SqlParameter("@UpdateTerritoryId", Team.TeamId??0),
                            new SqlParameter("@UpdateName", Team.Name),
                            new SqlParameter("@UpdateRegionId", Team.RegionId??0),
                            new SqlParameter("@UpdateDescription", Team.Description),
                            new SqlParameter("@UpdateIsActive", Team.IsActive),
                            new SqlParameter("@UpdatedBy", Team.UpdatedBy)
                        };
                    string errorMsg;
                    bool bSuccess = SqlDBHelper.ExecuteNonQueryWithErrorHandling("sp_DSD_TerritoryUpdateAndCorrectInCustomerMaster", ref sqlParameters, connString, out errorMsg);
                    if (bSuccess)
                    {
                        return new ActionStatus
                        {
                            Success = true,
                            Message = ""
                        };
                    }
                    else
                    {
                        throw new Exception(errorMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.TerritoryUpdateAndCorrectInCustomerMaster" + ex);
                result = new ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return result;
        }

        public ActionStatus Update(ENTTeam Team)
        {
            try
            {
                var dbTerritory = _territoryRepository.GetTerritory(Team.TeamId ?? 0);
                if (dbTerritory == null)
                    return new ActionStatus
                    {
                        Success = false,
                        Message = "Team not exist!"
                    };

                //dbTerritory = Configuration.Mapper.Map(Team, dbTerritory);
                //dbTerritory.UpdateDate = GetDateTime.getDate();
                //var result = _territoryRepository.Update(dbTerritory);
                var result = TerritoryUpdateAndCorrectInCustomerMaster(Team);
                result.Result = Configuration.Mapper.Map(result.Result, Team);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.Update" + ex);
                return new ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public ActionStatus CheckTerritoryNameExists(string territoryName, int territoryID)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                result.Success = (_territoryRepository.FindBy(r => r.TerritoryName.Equals(territoryName, StringComparison.CurrentCultureIgnoreCase) && r.TerritoryId != territoryID && r.IsDeleted == false).Count() > 0);
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.CheckTerritoryNameExists" + ex);
            }
            return result;
        }

        public ActionStatus DeleteTerritory(ENTPatchRequest activeStatus)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                var dbTerritory = _territoryRepository.FindBy(x => x.TerritoryId == activeStatus.Id).FirstOrDefault();
                if (dbTerritory != null)
                {
                    dbTerritory.IsDeleted = activeStatus.status;
                    //dbUser.UpdatedBy = _territoryRepository.FindBy(x => x.UserId == activeStatus.Id).FirstOrDefault().RecordId;
                    dbTerritory.UpdateDate = GetDateTime.getDate();
                    if (dbTerritory != null)
                    {
                        var response = _territoryRepository.Update(dbTerritory);
                        return response;
                    }
                    return result;
                }

                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.DeleteTerritory" + ex);
            }
            return result;
        }


        public List<ENTLookUpItem> GetCustReassignTerritoriesByRoleIds(string[] roleIds)
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            try
            {
                result = _territoryRepository.GetCustReassignTerritoriesByRoleIds(roleIds).Select(p => Configuration.Mapper.Map<ENTLookUpItem>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.GetCustReassignTerritoriesByRoleIds" + ex);
            }
            return result;

            //List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            ////string connString = _unitofwork.DbContext.Database.GetDbConnection().ConnectionString;
            //try
            //{
            //    result
            //    //result = SqlDBHelper.RawSqlQuery($"EXEC [sp_DSD_GetTerritoryLookUpByRoleId] ", x => new ENTLookUpItem
            //    //{
            //    //    RecordId = x["TerritoryId"].ToString(),
            //    //    Value = x["TerritoryName"].ToString(),
            //    //}, connString).ToList();
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(Constants.ACTION_EXCEPTION, "RoleService.GetCustReassignTerritoriesByRoleIds", ex);
            //}
            //return result;
        }

        public List<ENTLookUpItem> GetCustReassignTeamsByRoleIds(string[] roleIds)
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            try
            {
                result = _territoryRepository.GetCustReassignTeamByRoleIds(roleIds).Select(p => Configuration.Mapper.Map<ENTLookUpItem>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.GetCustReassignTeamsByRoleIds" + ex);
            }
            return result;

        }


        public List<ENTTeam> GetTeamListFromRegionId(long regionId)
        {
            List<ENTTeam> result = new List<ENTTeam>();
            string connString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                string strQuery = string.Format("EXEC [sp_DSD_GetTeamListFromRegionId] @RegionId={0}", regionId);

                result = SqlDBHelper.RawSqlQuery(strQuery, x => new ENTTeam
                {
                    Name = x[0].ToString(),
                }, connString).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.GetTeamListFromRegionId", ex);
            }
            return result;
        }

        public bool SyncBDTerritories(int BDId, List<int> territoryIds, long currentUserId)
        {
            try
            {
                var existingTerritoryIds = _territoryRepository.GetAll().Where(x => x.BDID == BDId).Select(x => x.TerritoryId).ToList();

                var territoriesToAdd = territoryIds.Except(existingTerritoryIds).ToList();
                var territoriesToRemove = existingTerritoryIds.Except(territoryIds).ToList();

                AssignBDToTerritories(BDId, territoriesToAdd, currentUserId);
                RemoveBDFromTerritories(territoriesToRemove, currentUserId);
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "TerritoryService.SyncBDTerritories" + ex);
                return false;
            }
            return true;
        }

        public bool AssignBDToTerritories(int BDId, List<int> territoryIds, long currentUserId)
        {
            try
            {
                var territories = _unitofwork.DbContext.TerritoryMaster
                    .Where(x => territoryIds.Contains(x.TerritoryId)).ToList();
                foreach (var territory in territories)
                {
                    territory.UpdateDate = DateTime.UtcNow;
                    territory.UpdatedBy = currentUserId;
                    territory.BDID = BDId;
                    var dbUsers = _unitofwork.DbContext.UserMaster
                    .Where(x => x.DefTerritoryId == territory.TerritoryId &&
                               x.IsInActive == false &&
                               x.IsDeleted == false)
                    .ToList();

                    foreach (var eachUser in dbUsers)
                    {
                        eachUser.BDID = BDId;
                        eachUser.UpdatedDate = DateTime.UtcNow;
                    }
                }
                _unitofwork.SaveAndContinue();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, $"{nameof(TerritoryService)}.{nameof(AssignBDToTerritories)} {ex}");
                return false;
            }
            return true;
        }

        public bool RemoveBDFromTerritories(List<int> territoryIds, long currentUserId)
        {
            try
            {
                var territories = _unitofwork.DbContext.TerritoryMaster
                    .Where(x => territoryIds.Contains(x.TerritoryId)).ToList();
                foreach (var territory in territories)
                {
                    territory.UpdateDate = DateTime.UtcNow;
                    territory.UpdatedBy = currentUserId;
                    territory.BDID = null;
                    var dbUsers = _unitofwork.DbContext.UserMaster
                       .Where(x => x.DefTerritoryId == territory.TerritoryId &&
                                  x.IsInActive == false &&
                                  x.IsDeleted == false)
                       .ToList();

                    foreach (var eachUser in dbUsers)
                    {
                        eachUser.BDID = 0;
                        eachUser.UpdatedDate = DateTime.UtcNow;
                    }
                }
                _unitofwork.SaveAndContinue();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, $"{nameof(TerritoryService)}.{nameof(RemoveBDFromTerritories)} {ex}");
                return false;
            }
            return true;
        }
    }
}

