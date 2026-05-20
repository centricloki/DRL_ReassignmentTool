using DRL.Core.Interface;
using DRL.Core.Mapper;
using DRL.Entity;
using DRL.Entity.Response;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using DRL.Model.DataBase;
using DRL.Model.Repository.Implementation;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EF = DRL.Model.Models;

namespace DRL.Core.Service
{
    public class BDService : IBDService
    {
        private readonly IBDMasterRepository _bdRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper CommonHelper;

        public BDService(IUnitOfWork unitofwork, IBDMasterRepository bdRepository, ILogManager logManager)
        {
            _bdRepository = bdRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            CommonHelper = new CommonHelper();
        }
        public List<ENTLookUpItem> GetBDsLookup()
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            try
            {
                result = _bdRepository.GetAllBDs().Select(p => Configuration.Mapper.Map<ENTLookUpItem>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ProgramManager.GetPrograms" + ex);
            }
            return result;
        }

        public ENTBD GetBD(int bdId)
        {
            try
            {
                var bd = _bdRepository.GetBDFindById(bdId);
                if (bd != null)
                {
                    return Configuration.Mapper.Map<ENTBD>(bd);
                }
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "BDService.GetBD" + ex);
            }
            return null;
        }

        public List<ENTBDResponse> GetBDList()
        {
            List<ENTBDResponse> result = new List<ENTBDResponse>();
            try
            {
                var bds = _bdRepository.GetAllBDs();
                foreach (var bd in bds)
                {
                    result.Add(new ENTBDResponse
                    {
                        BDID = bd.BDID,
                        BDName = bd.BDName,
                        IsActive = bd.IsActive,
                        Approver = bd.Approver,
                        UpdatedDate = bd.UpdateDate
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "BDService.GetBDList" + ex);
            }
            return result;
        }

        public ActionStatus CheckBDNameExists(string bdName, int bdId)
        {
            var actionStatus = new ActionStatus();
            try
            {
                var existingBD = _bdRepository.FindBy(x => 
                    x.BDName.Equals(bdName, StringComparison.CurrentCultureIgnoreCase) &&
                    x.BDID != bdId &&
                    !x.IsDeleted)
                    .FirstOrDefault();

                if (existingBD != null)
                {
                    actionStatus.Success = true;
                    actionStatus.Message = "BD already exists with same name.";
                }
                else
                {
                    actionStatus.Success = false;
                }
            }
            catch (Exception ex)
            {
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
                logger.Error(Constants.ACTION_EXCEPTION, "BDService.CheckBDNameExists" + ex);
            }
            return actionStatus;
        }

        public ActionStatus Insert(ENTBD bd)
        {
            var actionStatus = new ActionStatus();
            try
            {
                var bdMaster = new EF.BDMaster
                {
                    BDName = bd.BDName,
                    IsActive = bd.IsActive,
                    IsDeleted = bd.IsDeleted,
                    Approver = bd.Approver,
                    CreatedBy = bd.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                };

                _bdRepository.Insert(bdMaster);
                _unitofwork.SaveAndContinue();

                actionStatus.Success = true;
                actionStatus.Result = Configuration.Mapper.Map<ENTBD>(bdMaster);
            }
            catch (Exception ex)
            {
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
                logger.Error(Constants.ACTION_EXCEPTION, "BDService.Insert" + ex);
            }
            return actionStatus;
        }

        public ActionStatus Update(ENTBD bd)
        {
            var actionStatus = new ActionStatus();
            try
            {
                var existingBD = _bdRepository.GetBDFindById(bd.BDID);
                if (existingBD == null)
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "BD not found.";
                    return actionStatus;
                }

                existingBD.BDName = bd.BDName;
                existingBD.IsActive = bd.IsActive;
                existingBD.IsDeleted = bd.IsDeleted;
                existingBD.Approver = bd.Approver;
                existingBD.UpdatedBy = bd.UpdatedBy;
                existingBD.UpdateDate = DateTime.UtcNow;

                _bdRepository.Update(existingBD);
                _unitofwork.SaveAndContinue();

                actionStatus.Success = true;
                actionStatus.Result = Configuration.Mapper.Map<ENTBD>(existingBD);
            }
            catch (Exception ex)
            {
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
                logger.Error(Constants.ACTION_EXCEPTION, "BDService.Update" + ex);
            }
            return actionStatus;
        }

        public ActionStatus DeleteBD(ENTPatchRequest activeStatus)
        {
            var actionStatus = new ActionStatus();
            try
            {
                var bd = _bdRepository.GetBDFindById(activeStatus.Id);
                if (bd != null)
                {
                    bd.IsDeleted = true;
                    bd.UpdatedBy = activeStatus.UpdatedBy;
                    bd.UpdateDate = DateTime.UtcNow;

                    _bdRepository.Update(bd);
                    _unitofwork.SaveAndContinue();

                    actionStatus.Success = true;
                }
                else
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "BD not found.";
                }
            }
            catch (Exception ex)
            {
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
                logger.Error(Constants.ACTION_EXCEPTION, "BDService.DeleteBD" + ex);
            }
            return actionStatus;
        }

        public ActionStatus ManageBDStatus(ENTPatchRequest activeStatus)
        {
            var actionStatus = new ActionStatus();
            try
            {
                var bd = _bdRepository.GetBDFindById(activeStatus.Id);
                if (bd != null)
                {
                    bd.IsActive = activeStatus.status;
                    bd.UpdatedBy = activeStatus.UpdatedBy;
                    bd.UpdateDate = DateTime.UtcNow;

                    _bdRepository.Update(bd);
                    _unitofwork.SaveAndContinue();

                    actionStatus.Success = true;
                }
                else
                {
                    actionStatus.Success = false;
                    actionStatus.Message = "BD not found.";
                }
            }
            catch (Exception ex)
            {
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
                logger.Error(Constants.ACTION_EXCEPTION, "BDService.ManageBDStatus" + ex);
            }
            return actionStatus;
        }
    }
}
