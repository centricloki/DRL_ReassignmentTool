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
    public class BDRepository : GenericRepository<EF.BDMaster>, IBDRepository
    {
        private readonly ILogger logger;

        public BDRepository(IUnitOfWork unitOfWork, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IBDRepository));
        }

        public List<EF.BDMaster> GetAllBD()
        {
            var result = new List<EF.BDMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "BDRepository.GetAllBD");
                result = base.GetAllNoTracking().OrderBy(x=>x.BDName).ToList();
                logger.Info(Constants.ACTION_EXIT, "BDRepository.GetAllBD");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public EF.BDMaster GetBDFindById(int BDId)
        {
            var result = new EF.BDMaster();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "BDRepository.GetBD");
                result = base.FindByNoTracking(f => f.BDID == BDId).SingleOrDefault();
                logger.Info(Constants.ACTION_EXIT, "BDRepository.GetBD");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}
