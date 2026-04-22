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
    public class BDMasterRepository : GenericRepository<EF.BDMaster>, IBDMasterRepository
    {
        private readonly ILogger logger;

        public BDMasterRepository(IUnitOfWork unitOfWork, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IBDMasterRepository));
        }

        public List<EF.BDMaster> GetAllBDs()
        {
            var result = new List<EF.BDMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "BDMasterRepository.GetAllBDs");
                result = base.GetAllNoTracking().Where(x => !x.IsDeleted && x.IsActive).ToList();
                logger.Info(Constants.ACTION_EXIT, "BDMasterRepository.GetAllBDs");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

    }
}