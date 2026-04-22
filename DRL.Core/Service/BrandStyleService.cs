using DRL.Core.Interface;
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
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace DRL.Core.Service
{
    public class BrandStyleService : IBrandStyleService
    {
        private readonly IBrandStyleRepository _brandStyleRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger logger;
        private readonly CommonHelper CommonHelper;
        private readonly IConfiguration _configuration;

        public BrandStyleService(IUnitOfWork unitofwork, IBrandStyleRepository brandStyleRepository, ILogManager logManager, IConfiguration configuration)
        {
            _brandStyleRepository = brandStyleRepository;
            _unitofwork = unitofwork;
            logger = logManager.GetLogger(this.GetType());
                CommonHelper = new CommonHelper();
            _configuration = configuration;
        }

        public List<ENTBrandStyleMaster> GetBrandStyleMaster()
        {
            List<ENTBrandStyleMaster> result = new List<ENTBrandStyleMaster>();
            string connString = _configuration.GetConnectionString("DefaultConnection");;
            try
            {

                #region query
                string strQuery = string.Format("EXEC [sp_DSD_GetBrandStyleMasterDetails]");

                result = SqlDBHelper.RawSqlQuery(strQuery, x => new ENTBrandStyleMaster
                {
                    BrandIStyleID = Convert.ToInt32(x["BrandIStyleID"]),
                    BrandStyleName = x["BrandStyleName"].ToString(),
                    Description = x["Description"].ToString(),
                    ImageFilePath = x["ImageFilePath"].ToString(),
                    ParentID = Convert.ToInt32(x["ParentID"]),
                    SortOrder = Convert.ToInt32(x["SortOrder"])
                }, connString).ToList();
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "BrandStyleService.GetBrandStyleMaster", ex);
            }
            return result;
        }

        public ActionStatus UpdateBrandStyleMaster(int brandStyleId, int sortOrder)
        {
            ActionStatus result = new ActionStatus();
            try
            {
                string connString = _configuration.GetConnectionString("DefaultConnection");;
                if (brandStyleId > 0)
                {
                    List<SqlParameter> sqlParameters = new List<SqlParameter>()
                    {
                        new SqlParameter("@BrandStyleId", brandStyleId),
                        new SqlParameter("@SortOrder", sortOrder),

                    };

                    int count = SqlDBHelper.ExecuteNonQuery("sp_DSD_UpdateBrandStyleMasterSortOrder", ref sqlParameters, connString);
                    if (count > 0)
                    {
                        return new ActionStatus
                        {
                            Success = true,
                            Message = ""
                        };
                    }
                }
                return new ActionStatus
                {
                    Success = false,
                    Message = "Record Not Found"
                };
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, "BrandStyleService.UpdateBrandStyleMaster" + ex);
            }
            return result;
        }
    }
}
