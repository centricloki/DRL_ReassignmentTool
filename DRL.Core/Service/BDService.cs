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
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        private readonly IConfiguration _configuration;
        public BDService(IUnitOfWork unitofwork, IBDMasterRepository bdRepository, ILogManager logManager, IConfiguration configuration)
        {
            _bdRepository = bdRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            CommonHelper = new CommonHelper();
            _configuration = configuration;
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
            return UpdateBDAndMappingTables(bd);
        }

        public ActionStatus Update(ENTBD bd)
        {
            return UpdateBDAndMappingTables(bd);
        }

        private ActionStatus UpdateBDAndMappingTables(ENTBD bd)
        {
            ActionStatus result = new ActionStatus();
            
            // Add validation
            if (string.IsNullOrWhiteSpace(bd.BDName))
            {
                return new ActionStatus
                {
                    Success = false,
                    Message = "BD Name is required."
                };
            }
            
            try
            {
                string territoryIdsCsv = null;
                if (bd.Teams != null && bd.Teams.Any())
                {
                    var ids = bd.Teams.Where(x => x.TeamId.HasValue).Select(x => x.TeamId.Value.ToString());
                    territoryIdsCsv = string.Join(",", ids);
                }

                string connString = _configuration.GetConnectionString("DefaultConnection");
                var sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@UpdateBdId", bd.BDID) { Direction = ParameterDirection.InputOutput },
                    new SqlParameter("@UpdateName", bd.BDName ?? (object)DBNull.Value),
                    new SqlParameter("@UpdateIsActive", bd.IsActive),
                    new SqlParameter("@UpdateApprover", bd.Approver),
                    new SqlParameter("@TerritoryIds", string.IsNullOrEmpty(territoryIdsCsv) ? (object)DBNull.Value : territoryIdsCsv),
                    new SqlParameter("@UpdatedBy", bd.UpdatedBy.HasValue && bd.UpdatedBy.Value > 0 ? bd.UpdatedBy.Value : bd.CreatedBy)
                };

                string errorMsg;
                bool bSuccess = SqlDBHelper.ExecuteNonQueryWithErrorHandling("sp_DSD_BDUpdateAndCorrectMappingTables", ref sqlParameters, connString, out errorMsg);

                if (bSuccess)
                {
                    var newIdParam = sqlParameters.FirstOrDefault(p => p.ParameterName == "@UpdateBdId");
                    if (newIdParam != null && newIdParam.Value != null && newIdParam.Value != DBNull.Value)
                    {
                        int savedBdId = Convert.ToInt32(newIdParam.Value);
                        bd.BDID = savedBdId;
                    }

                    return new ActionStatus
                    {
                        Success = true,
                        Message = "",
                        Result = bd
                    };
                }
                else
                {
                    throw new Exception(errorMsg);
                }
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, nameof(UpdateBDAndMappingTables) + ex);
                result = new ActionStatus
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return result;
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

        public List<ENTLookUpItem> GetBDsByRegionIdLookup(int regionId)
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            try
            {
                // Get all BDs associated with the given region via territories
                result = _bdRepository.GetBDFindByRegionId(regionId)
                    .Select(p => Configuration.Mapper.Map<ENTLookUpItem>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "BDService.GetBDsByRegionIdLookup" + ex);
            }
            return result;
        }
    }
}
