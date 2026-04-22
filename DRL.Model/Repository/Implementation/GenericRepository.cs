using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DRL.Framework.Log;
using DRL.Framework.Log.Interface;
using DRL.Library;
using DRL.Model.Repository.Interface;
using DRL.Model.UnitOfWork.Interface;
using Microsoft.EntityFrameworkCore;

namespace DRL.Model.Repository.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class //, IEntityBase
    {
        public GenericRepository(IUnitOfWork unitOfWork, ILogManager logManager)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("UnitOfWork cannot be null.");
            if (logManager == null)
                throw new ArgumentNullException("LogManager cannot be null");

            _uow = unitOfWork;
            _logger = logManager.GetLogger(typeof(GenericRepository<T>));
        }

        private DbSet<T> Entities
        {
            get
            {
                if (_entities == null) _entities = _uow.DbContext.Set<T>();
                return _entities;
            }
        }

        public virtual void SetModified<K>(K entity) where K : class
        {
            _uow.DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            _logger.Info(Constants.ACTION_ENTRY, "GenericRepository.FindBy");
            var list = Entities.Where(predicate);
            _logger.Info(Constants.ACTION_EXIT, "GenericRepository.FindBy");
            return list;
        }

        // New method for read-only operations with no tracking
        public virtual IQueryable<T> FindByNoTracking(Expression<Func<T, bool>> predicate)
        {
            _logger.Info(Constants.ACTION_ENTRY, "GenericRepository.FindByNoTracking");
            var list = Entities.AsNoTracking().Where(predicate);
            _logger.Info(Constants.ACTION_EXIT, "GenericRepository.FindByNoTracking");
            return list;
        }

        public IQueryable<T> GetAll()
        {
            _logger.Info(Constants.ACTION_ENTRY, "GenericRepository.GetAll");
            IQueryable<T> list = Entities;
            _logger.Info(Constants.ACTION_EXIT, "GenericRepository.GetAll");
            return list;
        }

        // New method for read-only operations with no tracking
        public IQueryable<T> GetAllNoTracking()
        {
            _logger.Info(Constants.ACTION_ENTRY, "GenericRepository.GetAllNoTracking");
            IQueryable<T> list = Entities.AsNoTracking();
            _logger.Info(Constants.ACTION_EXIT, "GenericRepository.GetAllNoTracking");
            return list;
        }

        public T GetById(long id)
        {
            _logger.Info(Constants.ACTION_ENTRY, "GenericRepository.GetById");
            var obj = Entities.Find(id);
            _logger.Info(Constants.ACTION_EXIT, "GenericRepository.GetById");
            return obj;
        }

        // New method for read-only operations with no tracking
        public T GetByIdNoTracking(long id)
        {
            _logger.Info(Constants.ACTION_ENTRY, "GenericRepository.GetByIdNoTracking");
            var obj = Entities.AsNoTracking().FirstOrDefault(e => EF.Property<long>(e, "RecordId") == id);
            _logger.Info(Constants.ACTION_EXIT, "GenericRepository.GetByIdNoTracking");
            return obj;
        }

        //public T GetByUniqueId(Guid uniqueId)
        //{
        //    // return FindBy(x => x.RefId.Equals(uniqueId)).FirstOrDefault();
        //    return null;
        //}

        public ActionStatus Insert(T entity)
        {
            _logger.Info(Constants.ACTION_ENTRY, "GenericRepository.Insert");
            if (entity == null) throw new ArgumentNullException("entity");
            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            var _actionStatus = new ActionStatus();
            try
            {
                // entity.RefId = entity.RefId == Guid.Empty ? Guid.NewGuid() : entity.RefId;
                Entities.Add(entity);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
                // _actionStatus.Result = entity.RecordId;
                _actionStatus.Result = entity;
                _logger.Info(Constants.ACTION_EXIT, "GenericRepository.Insert");
            }
            catch (Exception ex)
            {
                _logger.Error("GenericRepository.Insert", ex);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.Info("GenericRepository.Insert", "Entity Inserted successfully,Committing Transaction");
                        var _tactionStatus = _uow.EndTransaction();
                        if (!_tactionStatus.Success) _actionStatus = _tactionStatus;
                    }
                    else
                    {
                        _logger.Info("GenericRepository.Insert",
                            "Having issues while Inserting entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            return _actionStatus;
        }

        public virtual ActionStatus Update(T entity)
        {
            _logger.Info(Constants.ACTION_ENTRY, "GenericRepository.Update");
            if (entity == null) throw new ArgumentNullException("entity");
            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            var _actionStatus = new ActionStatus();
            try
            {
                SetModified(entity);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
                //_actionStatus.Result = entity.RecordId;
                _actionStatus.Result = entity;
            }
            catch (Exception ex)
            {
                _logger.Error("GenericRepository.Update", ex);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.Info("GenericRepository.Update", "Entity Updated successfully,Committing Transaction");
                        var _tactionStatus = _uow.EndTransaction();
                        if (!_tactionStatus.Success) _actionStatus = _tactionStatus;
                    }
                    else
                    {
                        _logger.Info("GenericRepository.Update",
                            "Having issues while Updating entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            _logger.Info(Constants.ACTION_EXIT, "GenericRepository.Update");
            return _actionStatus;
        }

        public ActionStatus Delete(T entity)
        {
            _logger.Info(Constants.ACTION_ENTRY, "GenericRepository.Delete");

            if (entity == null) throw new ArgumentNullException("entity");

            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            var _actionStatus = new ActionStatus();
            try
            {
                Entities.Remove(entity);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("GenericRepository.Delete", ex);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.Info("GenericRepository.Delete",
                            "Operation executed successfully,Committing Transaction");
                        _actionStatus = _uow.EndTransaction();
                    }
                    else
                    {
                        _logger.Info("GenericRepository.Delete",
                            "Having issues while deleting entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            _logger.Info(Constants.ACTION_EXIT, "GenericRepository.Delete");
            return _actionStatus;
        }

        public ActionStatus RemoveRange(Expression<Func<T, bool>> predicate)
        {
            _logger.Info(Constants.ACTION_ENTRY, "GenericRepository.RemoveRange");
            var _actionStatus = new ActionStatus(false, string.Empty, string.Empty, null);
            var entityList = Entities.Where(predicate);
            if (entityList != null && entityList.Count() > 0)
            {
                _actionStatus.Success = true;
                return _actionStatus;
            }

            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            try
            {
                _entities.RemoveRange(entityList);
                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("GenericRepository.RemoveRange", ex);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.Info("GenericRepository.RemoveRange",
                            "Operation executed successfully,Committing Transaction");
                        _actionStatus = _uow.EndTransaction();
                    }
                    else
                    {
                        _logger.Info("GenericRepository.RemoveRange",
                            "Having issues while deleting entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }

            return _actionStatus;
        }

      
        #region Private Methods

        private ActionStatus ApplyChanges()
        {
            var result = new ActionStatus();
            try
            {
                result = _uow.SaveAndContinue();
                if (!result.Success) throw new Exception(result.Message);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.Error("GenericRepository.SaveChanges", ex);
                result.Message = ex.Message;
                _logger.Error(Constants.ACTION_EXCEPTION, ex, "GenericRepository.SaveChanges");
            }
            catch (DbUpdateException ese)
            {
                _logger.Error("GenericRepository.SaveChanges", ese);

                result.Message = ese.Message;
                _logger.Error(Constants.ACTION_EXCEPTION, ese, "GenericRepository.SaveChanges");
            }


            catch (Exception ex)
            {
                _logger.Error(Constants.ACTION_EXCEPTION, ex, "GenericRepository.SaveChanges");
                result.Message = ex.Message;
            }

            return result;
        }

        #endregion

        #region Variable Declaration

        protected readonly ILogger _logger;
        protected IUnitOfWork _uow;
        private DbSet<T> _entities;

        #endregion


        public ActionStatus UpdateRange(List<T> entities)
        {
            _logger.Info(Constants.ACTION_ENTRY, "GenericRepository.UpdateRange");
            if (entities == null) throw new ArgumentNullException("entity");
            var selfTran = false;
            if (!_uow.InTransaction)
            {
                _uow.BeginTransaction();
                selfTran = true;
            }

            var _actionStatus = new ActionStatus();
            try
            {
                foreach (var entity in entities)
                {
                    _uow.DbContext.Entry(entity).State = EntityState.Modified;
                }

                _actionStatus = ApplyChanges();

                if (!_actionStatus.Success) throw new Exception(_actionStatus.Message);

                _actionStatus.Result = entities;
            }
            catch (Exception ex)
            {
                _logger.Error("GenericRepository.UpdateRange", ex);
                _actionStatus.Success = false;
                _actionStatus.Message = ex.Message;
            }
            finally
            {
                if (selfTran)
                {
                    if (_actionStatus.Success)
                    {
                        _logger.Info("GenericRepository.UpdateRange", "Entity Updated successfully,Committing Transaction");
                        var _tactionStatus = _uow.EndTransaction();
                        if (!_tactionStatus.Success) _actionStatus = _tactionStatus;
                    }
                    else
                    {
                        _logger.Info("GenericRepository.UpdateRange",
                            "Having issues while Inserting entity,Rollbaking transaction");
                        _uow.RollBack();
                    }
                }
            }
            return _actionStatus;
        }

        public IQueryable<T> GetByWhere(Expression<Func<T, bool>> predicate)
        {
            return Entities.AsQueryable<T>().Where(predicate);
        }

        // New method for read-only operations with no tracking
        public IQueryable<T> GetByWhereNoTracking(Expression<Func<T, bool>> predicate)
        {
            return Entities.AsNoTracking().Where(predicate);
        }
    }
}