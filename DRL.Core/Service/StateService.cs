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
using System.Text;

namespace DRL.Core.Service
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper CommonHelper;

        public StateService(IUnitOfWork unitofwork, IStateRepository stateRepository, ILogManager logManager)
        {
            _stateRepository = stateRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            CommonHelper = new CommonHelper();
        }
        public List<ENTLookUpItem> GetStatesLookup()
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            try
            {
                result = _stateRepository.GetStates().Select(p => Configuration.Mapper.Map<ENTLookUpItem>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "StateService.GetStatesLookup" + ex);
            }
            return result;
        }
    }
}
