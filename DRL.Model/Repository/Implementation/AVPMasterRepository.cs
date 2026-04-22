using System.Collections.Generic;
using System.Linq;
using System;
using DRL.Framework.Log.Interface;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;
using EF = DRL.Model.Models;
using DRL.Framework.Log;


namespace DRL.Model.Repository.Implementation
{
    public class AVPMasterRepository : GenericRepository<EF.AVPMaster>, IAVPMasterRepository
    {
        private readonly ILogger logger;

        public AVPMasterRepository(IUnitOfWork unitOfWork, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IAVPMasterRepository));
        }

        public List<EF.AVPMaster> GetAllAVPs()
        {
            var result = new List<EF.AVPMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "AVPMasterRepository.GetAllAVPs");
                result = base.GetAllNoTracking().Where(x => !x.IsDeleted && x.IsActive).ToList();
                logger.Info(Constants.ACTION_EXIT, "AVPMasterRepository.GetAllAVPs");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

    }
}