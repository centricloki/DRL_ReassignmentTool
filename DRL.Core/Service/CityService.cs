using DRL.Core.Interface;
using DRL.Core.Mapper;
using DRL.Entity;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using DRL.Model.DataBase;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace DRL.Core.Service
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper CommonHelper;
        private readonly IConfiguration _configuration;

        public CityService(IUnitOfWork unitofwork, ICityRepository cityRepository, ILogManager logManager, IConfiguration configuration)
        {
            _cityRepository = cityRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
            CommonHelper = new CommonHelper();
            _configuration = configuration;
        }
        public List<ENTLookUpItem> GetCitiesLookup(string state)
        {
            List<ENTLookUpItem> result = new List<ENTLookUpItem>();
            string connString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                string strQuery = string.Format("EXEC [SP_DSD_GetCityFromStateId] @State='{0}'", state);

                result = SqlDBHelper.RawSqlQuery(strQuery, x => new ENTLookUpItem
                {
                    Value = x[0].ToString(),
                    RecordId = x[1].ToString()
                }, connString).OrderBy(x=>x.Value).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "CityService.GetCitiesLookup" + ex);
            }
            return result;
        }
    }
}
