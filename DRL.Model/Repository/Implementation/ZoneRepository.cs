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
    public class ZoneRepository : GenericRepository<EF.ZoneMaster>, IZoneRepository
    {
        private readonly ILogger logger;

        public ZoneRepository(IUnitOfWork unitOfWork, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IRoleRepository));
        }

        public List<EF.ZoneMaster> GetAllZone()
        {
            var result = new List<EF.ZoneMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "ZoneRepository.GetAllZone");
                result = base.GetAllNoTracking().OrderBy(x=>x.ZoneName).ToList();
                logger.Info(Constants.ACTION_EXIT, "ZoneRepository.GetAllZone");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}