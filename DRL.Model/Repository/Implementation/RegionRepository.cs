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
    public class RegionRepository : GenericRepository<EF.RegionMaster>, IRegionRepository
    {
        private readonly ILogger logger;

        public RegionRepository(IUnitOfWork unitOfWork, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IRoleRepository));
        }

        public List<EF.RegionMaster> GetAllRegion()
        {
            var result = new List<EF.RegionMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "RegionRepository.GetAllRegion");
                result = base.GetAllNoTracking().Where(x => x.IsDeleted == false).OrderBy(x => x.Regioname).ToList();
                logger.Info(Constants.ACTION_EXIT, "RegionRepository.GetAllRegion");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public EF.RegionMaster GetRegion(long RegionId)
        {
            var result = new EF.RegionMaster();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "RegionRepository.GetRegion");
                result = base.FindByNoTracking(f => f.RegionId == RegionId).SingleOrDefault();
                logger.Info(Constants.ACTION_EXIT, "RegionRepository.GetRegion");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

        public List<EF.RegionMaster> GetAllActiveRegions()
        {
            var result = new List<EF.RegionMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "RegionRepository.GetAllActiveRegions");
                result = base.GetAllNoTracking().Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
                logger.Info(Constants.ACTION_EXIT, "RegionRepository.GetAllActiveRegions");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }

    }
}