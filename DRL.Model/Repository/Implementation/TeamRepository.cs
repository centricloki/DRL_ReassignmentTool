using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using EF = DRL.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace DRL.Model.Repository.Implementation
{
    public class TeamRepository : GenericRepository<EF.TerritoryMaster>, ITeamRepository
    {
        private readonly ILogger logger;

        public TeamRepository(IUnitOfWork unitOfWork, ILogManager logManager) : base(unitOfWork, logManager)
        {
            _uow = unitOfWork;
            logger = logManager.GetLogger(typeof(IRoleRepository));
        }

        public List<EF.TerritoryMaster> GetUserTeams(long userId)
        {
            var result = new List<EF.TerritoryMaster>();
            try
            {
                logger.Info(Constants.ACTION_ENTRY, "TeamRepository.GetUserTeams");
                var user = _uow.DbContext.UserMaster.AsNoTracking().FirstOrDefault(u => u.UserId == userId);
                var bdRole = _uow.DbContext.RoleMaster.AsNoTracking().FirstOrDefault(x => x.RoleName.ToLower().Contains("BD Manager".ToLower()));
                if (user != null && user.RoleId == bdRole?.RoleId)
                {
                    result = _uow.DbContext.TerritoryMaster.AsNoTracking().Where(x => x.BDID.HasValue && x.BDID.Value == user.BDID).ToList();
                }
                else
                {
                    var userTerritory = _uow.DbContext.UserMaster.AsNoTracking().Where(u => u.UserId == userId).Select(u => u.TerritoryId).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(userTerritory))
                    {
                        var lstuserTerritory = userTerritory.Split(',').ToList();
                        result = _uow.DbContext.TerritoryMaster.AsNoTracking().Where(t => lstuserTerritory.Contains(t.TerritoryId.ToString())).ToList();
                    }
                    logger.Info(Constants.ACTION_EXIT, "TeamRepository.GetUserTeams");
                } 
            }
            catch (Exception ex)
            {
                logger.Error(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
    }
}