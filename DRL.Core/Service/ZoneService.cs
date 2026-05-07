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
    public class ZoneService : IZoneService
    {
        private readonly IZoneRepository _zoneRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper CommonHelper;
        private readonly IConfiguration _configuration;

        public ZoneService(IUnitOfWork unitofwork, IZoneRepository zoneRepository, ILogManager logManager, IUserRepository userRepository, IConfiguration configuration)
        {
            _zoneRepository = zoneRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            CommonHelper = new CommonHelper();
            _userRepository = userRepository;
            _configuration = configuration;
        }
        public List<ENTLookUpItem> GetAllZoneLookup()
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            try
            {
                result = _zoneRepository.GetAllZone().Select(p => Configuration.Mapper.Map<ENTLookUpItem>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ZoneService.GetAllZoneLookup" + ex);
            }
            return result;
        }

        public List<ENTLookUpItem> GetAllZoneLookupByAVP(long userId)
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            try
            {
                var user = _userRepository.GetUser(userId);
                result = _zoneRepository.GetAllZone().Where(x => x.IsActive && !x.IsDeleted && x.AVPID == user.AVPID).Select(p => Configuration.Mapper.Map<ENTLookUpItem>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ZoneService.GetAllZoneLookupByAVP" + ex);
            }
            return result;
        }

        public List<ENTZone> GetAllZones()
        {
            List<ENTZone> result = new List<ENTZone>();
            try
            {
                result = _zoneRepository.GetAll().Where(x => x.IsActive && !x.IsDeleted).Select(p => Configuration.Mapper.Map<ENTZone>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ZoneService.GetAllZones" + ex);
            }
            return result;
        }

        public List<ENTZone> GetAllZoneByAVP(int avpId)
        {
            List<ENTZone> result = new List<ENTZone>();
            try
            {
                result = _zoneRepository.GetAllZone().Where(x => x.IsActive && !x.IsDeleted && x.AVPID == avpId).Select(p => Configuration.Mapper.Map<ENTZone>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ZoneService.GetAllZoneByAVP" + ex);
            }
            return result;
        }

        public bool SyncAVPZones(int AVPId, List<int> zoneIds)
        {
            try
            {
                var existingZoneIds = _zoneRepository.GetAll().Where(x => x.AVPID == AVPId).Select(x => x.ZoneId).ToList();

                var zonesToAdd = zoneIds.Except(existingZoneIds).ToList();
                var zonesToRemove = existingZoneIds.Except(zoneIds).ToList();

                AssignAVPToZones(AVPId, zonesToAdd);
                RemoveAVPFromZones(zonesToRemove);

            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ZoneService.SyncAVPZones" + ex);
                return false;
            }
            return true;
        }

        public bool AssignAVPToZones(int AVPId, List<int> zoneIds)
        {
            try
            {
                var zones = _unitofwork.DbContext.ZoneMaster.Where(x => zoneIds.Contains(x.ZoneId));
                foreach (var zone in zones)
                {
                    zone.UpdateDate = DateTime.UtcNow;
                    zone.AVPID = AVPId;
                    var dbUsers = _unitofwork.DbContext.UserMaster
                       .Where(x => x.ZoneId == zone.ZoneId &&
                                  x.IsInActive == false &&
                                  x.IsDeleted == false)
                       .ToList();

                    foreach (var eachUser in dbUsers)
                    {
                        eachUser.AVPID = AVPId;
                        eachUser.UpdatedDate = DateTime.UtcNow;
                    }
                }
                _unitofwork.SaveAndContinue();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, $"{nameof(ZoneService)}.{nameof(AssignAVPToZones)} {ex}");
                return false;
            }
            return true;
        }

        public bool RemoveAVPFromZones(List<int> zoneIds)
        {
            try
            {
                var zones = _unitofwork.DbContext.ZoneMaster.Where(x => zoneIds.Contains(x.ZoneId));
                foreach (var zone in zones)
                {
                    zone.UpdateDate = DateTime.UtcNow;
                    zone.AVPID = null;
                    var dbUsers = _unitofwork.DbContext.UserMaster
                       .Where(x => x.ZoneId == zone.ZoneId &&
                                  x.IsInActive == false &&
                                  x.IsDeleted == false)
                       .ToList();

                    foreach (var eachUser in dbUsers)
                    {
                        eachUser.AVPID = 0;
                        eachUser.UpdatedDate = DateTime.UtcNow;
                    }
                }
                _unitofwork.SaveAndContinue();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, $"{nameof(ZoneService)}.{nameof(RemoveAVPFromZones)} {ex}");
                return false;
            }
            return true;
        }

        public ENTZone GetZone(int zoneId)
        {
            try
            {
                var zone = _zoneRepository.GetById(zoneId);
                if (zone != null)
                {
                    return Configuration.Mapper.Map<ENTZone>(zone);
                }
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ZoneService.GetZone" + ex);
            }
            return null;
        }

        public List<ENTZoneResponse> GetZoneList()
        {
            List<ENTZoneResponse> result = new List<ENTZoneResponse>();
            try
            {
                var zones = _zoneRepository.GetAllZone().Where(x => x.IsActive && !x.IsDeleted).ToList();
                foreach (var zone in zones)
                {
                    var zoneResponse = new ENTZoneResponse
                    {
                        ZoneId = zone.ZoneId,
                        ZoneName = zone.ZoneName,
                        IsActive = zone.IsActive,
                        UpdatedDate = zone.UpdateDate
                    };

                    // Get AVP details if available
                    if (zone.AVPID.HasValue && zone.AVPID > 0)
                    {
                        var avpUser = _userRepository.GetUser(zone.AVPID.Value);
                        if (avpUser != null)
                        {
                            zoneResponse.AVPID = zone.AVPID;
                            zoneResponse.AVPName = $"{avpUser.FirstName ?? ""} {avpUser.LastName ?? ""}";
                        }
                    }
                    result.Add(zoneResponse);
                }
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ZoneService.GetZoneList" + ex);
            }
            return result;
        }

        public ActionStatus CheckZoneNameExists(string zoneName, int zoneId)
        {
            var actionStatus = new ActionStatus();
            try
            {
                var existingZone = _zoneRepository.GetAllZone()
                    .FirstOrDefault(x => x.ZoneName.Equals(zoneName, StringComparison.OrdinalIgnoreCase) &&
                                       x.ZoneId != zoneId &&
                                       x.IsActive &&
                                       !x.IsDeleted);

                if (existingZone != null)
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "Zone already exists with same zone name.";
                }
                else
                {
                    actionStatus.Success = true;
                }
            }
            catch (Exception ex)
            {
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
                logger.Error(Constants.ACTION_EXCEPTION, "ZoneService.CheckZoneNameExists" + ex);
            }
            return actionStatus;
        }

        public ActionStatus Insert(ENTZone zone)
        {
            var actionStatus = new ActionStatus();
            try
            {
                var zoneMaster = new EF.ZoneMaster
                {
                    ZoneName = zone.ZoneName,
                    SugarZoneId = zone.SugarZoneId,
                    ImportedFrom = zone.ImportedFrom,
                    AVPID = zone.AVPID,
                    IsActive = zone.IsActive,
                    IsDeleted = zone.IsDeleted,
                    UpdateDate = DateTime.UtcNow,
                };

                _zoneRepository.Insert(zoneMaster);
                _unitofwork.SaveAndContinue();

                actionStatus.Success = true;
                actionStatus.Result = Configuration.Mapper.Map<ENTZone>(zoneMaster);
            }
            catch (Exception ex)
            {
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
                logger.Error(Constants.ACTION_EXCEPTION, "ZoneService.Insert" + ex);
            }
            return actionStatus;
        }

        public ActionStatus Update(ENTZone zone)
        {
            var actionStatus = new ActionStatus();
            try
            {
                var existingZone = _zoneRepository.GetById(zone.ZoneId);
                if (existingZone == null)
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "Zone not found.";
                    return actionStatus;
                }

                bool avpChanged = existingZone.AVPID != zone.AVPID;

                if (avpChanged)
                {
                    actionStatus = UpdateZoneAndMappingTables(zone);
                }
                else
                {
                    existingZone.ZoneName = zone.ZoneName;
                    existingZone.SugarZoneId = zone.SugarZoneId;
                    existingZone.ImportedFrom = zone.ImportedFrom;
                    existingZone.IsActive = zone.IsActive;
                    existingZone.IsDeleted = zone.IsDeleted;
                    existingZone.UpdateDate = DateTime.UtcNow;

                    _zoneRepository.Update(existingZone);
                    _unitofwork.SaveAndContinue();

                    actionStatus.Success = true;
                }

                actionStatus.Result = Configuration.Mapper.Map<ENTZone>(existingZone);
            }
            catch (Exception ex)
            {
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
                logger.Error(Constants.ACTION_EXCEPTION, "ZoneService.Update" + ex);
            }
            return actionStatus;
        }

        public ActionStatus UpdateZoneAndMappingTables(ENTZone zone)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                string connString = _configuration.GetConnectionString("DefaultConnection");
                List<SqlParameter> sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@UpdateZoneId", zone.ZoneId),
                    new SqlParameter("@UpdateName", zone.ZoneName),
                    new SqlParameter("@UpdateAVPID", (object)zone.AVPID ?? DBNull.Value),
                    new SqlParameter("@UpdateIsActive", zone.IsActive),
                    new SqlParameter("@UpdatedBy", 0)
                };

                string errorMsg;
                bool bSuccess = SqlDBHelper.ExecuteNonQueryWithErrorHandling(
                    "sp_DSD_ZoneUpdateAndCorrectMappingTables",
                    ref sqlParameters,
                    connString,
                    out errorMsg);

                if (bSuccess)
                {
                    result.Success = true;
                    result.Message = "";
                }
                else
                {
                    throw new Exception(errorMsg);
                }
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, nameof(UpdateZoneAndMappingTables) + ex);
                result = new ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return result;
        }

        public ActionStatus DeleteZone(ENTPatchRequest activeStatus)
        {
            var actionStatus = new ActionStatus();
            try
            {
                var zone = _zoneRepository.GetById(activeStatus.Id);
                if (zone != null)
                {
                    zone.IsDeleted = true;
                    zone.UpdateDate = DateTime.UtcNow;

                    _zoneRepository.Update(zone);
                    _unitofwork.SaveAndContinue();

                    actionStatus.Success = true;
                }
                else
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "Zone not found.";
                }
            }
            catch (Exception ex)
            {
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
                logger.Error(Constants.ACTION_EXCEPTION, "ZoneService.DeleteZone" + ex);
            }
            return actionStatus;
        }

        public ActionStatus ManageZoneStatus(ENTPatchRequest activeStatus)
        {
            var actionStatus = new ActionStatus();
            try
            {
                var zone = _zoneRepository.GetById(activeStatus.Id);
                if (zone != null)
                {
                    zone.IsActive = activeStatus.status;
                    zone.UpdateDate = DateTime.UtcNow;

                    _zoneRepository.Update(zone);
                    _unitofwork.SaveAndContinue();

                    actionStatus.Success = true;
                }
                else
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "Zone not found.";
                }
            }
            catch (Exception ex)
            {
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
                logger.Error(Constants.ACTION_EXCEPTION, "ZoneService.ManageZoneStatus" + ex);
            }
            return actionStatus;
        }
    }
}
