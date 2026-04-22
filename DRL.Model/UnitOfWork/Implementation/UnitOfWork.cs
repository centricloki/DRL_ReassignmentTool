using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using DRL.Model.UnitOfWork.Interface;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using EF = DRL.Model.Models;

namespace DRL.Model.UnitOfWork.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Constructor

        public UnitOfWork(DbContext dbContext, ILogManager logManager)
        {
            if (dbContext == null)
                throw new ArgumentNullException("DBContext cannot be null.");
            if (logManager == null)
                throw new ArgumentNullException("LogManager cannot be null");

            DbContext = (EF.DRLNewContext)dbContext;
            // Removed global NoTracking - will apply NoTracking only to read-only operations

            _logger = logManager.GetLogger(typeof(UnitOfWork));
        }

        #endregion

        #region Variable Declaration

        private bool _disposed;
        private readonly ILogger _logger;
        private IDbContextTransaction _transaction { get; set; }

        #endregion

        #region Properties

        public bool InTransaction { get; private set; }

        public EF.DRLNewContext DbContext { get; }

        #endregion

        #region Methods

        public virtual void BeginTransaction()
        {
            _logger.Info(Constants.ACTION_ENTRY, "UnitOfWork.BeginTransaction");
            try
            {
                InTransaction = true;
                _transaction = DbContext.Database.BeginTransaction();
                _logger.Info(Constants.ACTION_EXIT, "UnitOfWork.BeginTransaction");
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.BeginTransaction", ex);
                throw;
            }
        }

        public virtual ActionStatus EndTransaction()
        {
            _logger.Info(Constants.ACTION_ENTRY, "UnitOfWork.EndTransaction");
            var status = new ActionStatus();
            try
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                DbContext.SaveChanges();
                _transaction.Commit();
                InTransaction = false;
                status.Success = true;
            }
            //catch (DbEntityValidationException dbEx)
            //{
            //    _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.EndTransaction", dbEx);
            //    status.Message = dbEx.Message;
            //    status.Success = false;
            //    _inTransaction = false;
            //}
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.SaveAndContinue", ex);
                status.Message = ex.Message;
                status.Success = false;
            }

            _logger.Info(Constants.ACTION_EXIT, "UnitOfWork.EndTransaction");
            return status;
        }

        public virtual void RollBack()
        {
            _logger.Info(Constants.ACTION_ENTRY, "UnitOfWork.RollBack");
            try
            {
                _transaction.Rollback();
                _transaction.Dispose();
                InTransaction = false;
            }
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.BeginTransaction", ex);
                throw;
            }

            _logger.Info(Constants.ACTION_EXIT, "UnitOfWork.RollBack");
        }

        public virtual ActionStatus SaveAndContinue()
        {
            _logger.Info(Constants.ACTION_ENTRY, "UnitOfWork.SaveAndContinue");
            var status = new ActionStatus();
            try
            {
                DbContext.SaveChanges();
                status.Success = true;
            }
            //catch (DbEntityValidationException dbEx)
            //{
            //    _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.SaveAndContinue", dbEx);
            //    var errorMessages = dbEx.EntityValidationErrors
            //        .SelectMany(x => x.ValidationErrors)
            //        .Select(x => x.ErrorMessage);

            //    status.Message = string.Join("; ", errorMessages);
            //    status.Success = false;
            //}
            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION + ":UnitofWork.SaveAndContinue", ex);
                status.Message = ex.InnerException.Message;
                status.Success = false;
            }

            _logger.Info(Constants.ACTION_EXIT, "UnitOfWork.SaveAndContinue");
            return status;
        }

        #endregion

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing && DbContext != null && InTransaction) _transaction.Dispose();
            if (disposing && DbContext != null) DbContext.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            _logger.Info(Constants.ACTION_ENTRY, "UnitOfWork.Dispose");
            Dispose(true);
            GC.SuppressFinalize(this);
            _logger.Info(Constants.ACTION_EXIT, "UnitOfWork.Dispose");
        }

        #endregion
    }
}