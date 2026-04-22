using DRL.Core.Interface;
using DRL.Core.Mapper;
using DRL.Entity;
using DRL.Entity.Response;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using DRL.Model.DataBase;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using EF = DRL.Model.Models;

namespace DRL.Core.Service
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper CommonHelper;
        private readonly IConfiguration _configuration;

        public RegionService(IUnitOfWork unitofwork, IRegionRepository regionRepository, IUserRepository userRepository, ILogManager logManager, IConfiguration configuration)
        {
            _regionRepository = regionRepository;
            _userRepository = userRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            CommonHelper = new CommonHelper();
            _configuration = configuration;
        }
        public List<ENTLookUpItem> GetAllRegionLookup()
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            try
            {
                result = _regionRepository.GetAllActiveRegions().Select(p => Configuration.Mapper.Map<ENTLookUpItem>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "RegionService.GetAllRegionLookup" + ex);
            }
            return result;
        }

        public ENTRegion GetRegion(long RegionId)
        {
            ENTRegion result = new ENTRegion();
            try
            {
                result = Configuration.Mapper.Map<ENTRegion>(_regionRepository.GetRegion(RegionId));
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "RegionService.GetRegion" + ex);
            }
            return result;
        }

        public List<ENTRegion> GetAllRegions()
        {
            List<ENTRegion> result = new List<ENTRegion>();
            try
            {
                result = _regionRepository.GetAllRegion().Select(p => Configuration.Mapper.Map<ENTRegion>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "RegionService.GetAllRegions" + ex);
            }
            return result;
        }

        public ActionStatus CheckRegionNameExists(string regionName, int regionId)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                result.Success = (_regionRepository.FindBy(r => r.Regioname.Equals(regionName, StringComparison.CurrentCultureIgnoreCase) && r.RegionId != regionId && r.IsDeleted == false).Count() > 0);
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "RegionService.CheckRoleNameExists" + ex);
            }
            return result;
        }

        public ActionStatus DeleteRegion(ENTPatchRequest activeStatus)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                var isUserExists = _userRepository.FindBy(x => x.RegionId == activeStatus.Id && x.IsDeleted == false).ToList();
                if (isUserExists != null && isUserExists.Count > 0)
                {
                    return new ActionStatus
                    {
                        Success = false,
                        Message = "Region can't be deleted as it is already associated with atleast one of the user."
                    };
                }
                var dbRegion = _regionRepository.FindBy(x => x.RegionId == activeStatus.Id).FirstOrDefault();
                if (dbRegion != null)
                {
                    dbRegion.IsDeleted = activeStatus.status;
                    dbRegion.UpdatedBy = activeStatus.UpdatedBy;
                    dbRegion.UpdateDate = GetDateTime.getDate();
                    var response = _regionRepository.Update(dbRegion);
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
                logger.Error(Constants.ACTION_EXCEPTION, "RegionService.DeleteRegion" + ex);
            }
            return result;
        }

        public ActionStatus Insert(ENTRegion Region)
        {
            try
            {
                // Validate that ZoneId is provided (not null and greater than 0)
                if (Region.ZoneId == null || Region.ZoneId <= 0)
                {
                    return new ActionStatus
                    {
                        Success = false,
                        Message = "Please select a valid Zone. Region cannot be created without associating it to a Zone."
                    };
                }

                var dbRegion = Configuration.Mapper.Map<ENTRegion, EF.RegionMaster>(Region);
                dbRegion.CreatedDate = GetDateTime.getDate();
                dbRegion.UpdateDate = GetDateTime.getDate();
                var resp = _regionRepository.Insert(dbRegion);
                resp.Result = Configuration.Mapper.Map(resp.Result, Region);
                return resp;
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "RegionService.Insert" + ex);
                return new ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public ActionStatus Update(ENTRegion Region)
        {
            try
            {
                // Validate that ZoneId is provided (not null and greater than 0)
                //if (Region.ZoneId == null || Region.ZoneId <= 0)
                //{
                //    return new ActionStatus
                //    {
                //        Success = false,
                //        Message = "Please select a valid Zone. Region cannot be updated without associating it to a Zone."
                //    };
                //}

                var dbRegion = _regionRepository.GetRegion(Region.RegionId??0);
                if (dbRegion == null)
                    return new ActionStatus
                    {
                        Success = false,
                        Message = "Region not exists!"
                    };
                var result = UpdateRegionAndMappingTables(Region);
                result.Result = Configuration.Mapper.Map(result.Result, Region);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, nameof(Update) + ex);
                return new ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        private ActionStatus UpdateRegionAndMappingTables(ENTRegion Region)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                string connString = _configuration.GetConnectionString("DefaultConnection");;
                {
                    List<SqlParameter> sqlParameters = new List<SqlParameter>()
                        {
                            new SqlParameter("@UpdateRegionId", Region.RegionId??0),
                            new SqlParameter("@UpdateName", Region.Regioname),
                            new SqlParameter("@UpdateZoneId", Region.ZoneId),
                            new SqlParameter("@UpdateIsActive", Region.IsActive),
                            new SqlParameter("@UpdatedBy", Region.UpdatedBy)
                        };
                    string errorMsg;
                    bool bSuccess = SqlDBHelper.ExecuteNonQueryWithErrorHandling("sp_DSD_RegionUpdateAndCorrectMappingTables", ref sqlParameters, connString, out errorMsg);
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
                logger.Error(Constants.ACTION_EXCEPTION, nameof(UpdateRegionAndMappingTables) + ex);
                result = new ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return result;
        }


        public List<ENTRegionResponse> GetRegionList()
        {
            List<ENTRegionResponse> result = new List<ENTRegionResponse>();
            string connString = _configuration.GetConnectionString("DefaultConnection");;
            try
            {
                result = SqlDBHelper.RawSqlQuery("EXEC [sp_DSD_GetRegionList] ", x => new ENTRegionResponse
                {
                    RegionId = Convert.ToInt32(x[0]),
                    Regioname = x[1].ToString(),
                    CreatedBy = x[2].ToString(),
                    UpdatedBy = x[3].ToString(),
                    IsActive = Convert.ToBoolean(x[4]),
                    IsDeleted = Convert.ToBoolean(x[5]),
                    CreatedDate = Convert.ToDateTime(x[6]),
                    ZoneId = Convert.ToInt32(x[7]),
                    ZoneName = x[8].ToString()
                }, connString).OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "RegionService.GetRegionList", ex);
            }
            return result;
        }

        public ActionStatus ManageRegionStatus(ENTPatchRequest activeStatus)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                if (!activeStatus.status)
                {
                    var isUserExists = _userRepository.FindBy(x => x.RegionId == activeStatus.Id && x.IsDeleted == false).ToList();
                    if (isUserExists != null && isUserExists.Count > 0)
                    {
                        return new ActionStatus
                        {
                            Success = false,
                            Message = "Region can't be deactivated as it is already associated with atleast one of the user."
                        };
                    }
                }
                var dbRegion = _regionRepository.FindBy(x => x.RegionId == activeStatus.Id).FirstOrDefault();
                if (dbRegion != null)
                {
                    dbRegion.IsActive = activeStatus.status;
                    dbRegion.UpdateDate = GetDateTime.getDate();
                    dbRegion.UpdatedBy = activeStatus.UpdatedBy;
                    var response = _regionRepository.Update(dbRegion);
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
                logger.Error(Constants.ACTION_EXCEPTION, "UserService.ManageUserStatus" + ex);
            }
            return result;
        }
    }
}
