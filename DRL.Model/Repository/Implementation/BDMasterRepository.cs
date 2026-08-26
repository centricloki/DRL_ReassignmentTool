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
        private readonly ITerritoryRepository _territoryRepository;
        public BDMasterRepository(IUnitOfWork unitOfWork, ITerritoryRepository territoryRepository, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IBDMasterRepository));
            this._territoryRepository = territoryRepository;
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

        public EF.BDMaster GetBDFindById(int BDId)
        {
            var result = new EF.BDMaster();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "BDMasterRepository.GetBD");
                result = base.FindByNoTracking(f => f.BDID == BDId).SingleOrDefault();
                logger.Info(Constants.ACTION_EXIT, "BDMasterRepository.GetBD");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public List<EF.BDMaster> GetBDFindByRegionId(int regionId)
        {
            var result = new List<EF.BDMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "BDRepository.GetBDFindByRegionId");

                // Get distinct BD IDs from territories that belong to the specified region
                var territoryBDIds = _territoryRepository.FindByNoTracking(t => t.RegionId == regionId && t.BDID.HasValue)
                                                         .Select(t => t.BDID.Value)
                                                         .Distinct()
                                                         .ToList();

                // Find all BDMaster records that match these BD IDs
                if (territoryBDIds.Any())
                {
                    result = base.FindByNoTracking(bd => territoryBDIds.Contains(bd.BDID))
                                 .ToList();
                }

                logger.Info(Constants.ACTION_EXIT, "BDRepository.GetBDFindByRegionId");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}