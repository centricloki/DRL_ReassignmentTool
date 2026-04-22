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
    public class CityRepository : GenericRepository<EF.CityMaster>, ICityRepository
    {
        private readonly ILogger logger;

        public CityRepository(IUnitOfWork unitOfWork, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IRoleRepository));
        }

    }
}
