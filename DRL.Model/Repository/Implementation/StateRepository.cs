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
    public class StateRepository : GenericRepository<EF.StateMaster>, IStateRepository
    {
        private readonly ILogger logger;

        public StateRepository(IUnitOfWork unitOfWork, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IRoleRepository));
        }

        public List<EF.StateMaster> GetStates()
        {
            var result = new List<EF.StateMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "StateRepository.GetStates");
                result = base.GetAllNoTracking().OrderBy(x=>x.StateName).ToList();
                logger.Info(Constants.ACTION_EXIT, "StateRepository.GetStates");
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}