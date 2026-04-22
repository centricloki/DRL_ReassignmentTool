using DRL.Framework.Log.Interface;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;
using EF = DRL.Model.Models;


namespace DRL.Model.Repository.Implementation
{
    public class BrandStyleRepository : GenericRepository<EF.BrandStyleMaster>, IBrandStyleRepository
    {
        private readonly ILogger logger;
        public BrandStyleRepository(IUnitOfWork unitOfWork, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IRoleRepository));
        }
    }
}
