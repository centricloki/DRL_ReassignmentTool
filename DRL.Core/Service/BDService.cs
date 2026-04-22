using DRL.Core.Interface;
using DRL.Core.Mapper;
using DRL.Entity;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using DRL.Model.DataBase;
using DRL.Model.Repository.Implementation;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRL.Core.Service
{
    public class BDService : IBDService
    {
        private readonly IBDMasterRepository _bdRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper CommonHelper;

        public BDService(IUnitOfWork unitofwork, IBDMasterRepository bdRepository, ILogManager logManager)
        {
            _bdRepository = bdRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            CommonHelper = new CommonHelper();
        }
        public List<ENTLookUpItem> GetBDsLookup()
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            try
            {
                result = _bdRepository.GetAllBDs().Select(p => Configuration.Mapper.Map<ENTLookUpItem>(p)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "ProgramManager.GetPrograms" + ex);
            }
            return result;
        }
    }
}
