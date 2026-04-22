using DRL.Core.Interface;
using DRL.Core.Mapper;
using DRL.Entity;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DRL.Core.Service
{
    public class ZoneService : IZoneService
    {
        private readonly IZoneRepository _zoneRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper CommonHelper;

        public ZoneService(IUnitOfWork unitofwork, IZoneRepository zoneRepository, ILogManager logManager, IUserRepository userRepository)
        {
            _zoneRepository = zoneRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            CommonHelper = new CommonHelper();
            _userRepository = userRepository;
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

        public bool SyncAVPZones(int AVPId,List<int> zoneIds)
        {
            try
            {
                var existingZoneIds = _zoneRepository.GetAll().Where(x => x.AVPID == AVPId).Select(x => x.ZoneId).ToList();

                var zonesToAdd = zoneIds.Except(existingZoneIds).ToList();
                var zonesToRemove = existingZoneIds.Except(zoneIds).ToList();

                AssignAVPToZones(AVPId, zonesToAdd);
                RemoveAVPFromZones(zonesToRemove);

            }
            catch(Exception ex)
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
    }
}
