using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using DRL.Library;
using EF = DRL.Model.Models;

namespace DRL.Model.UnitOfWork.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        #region Context

        EF.DRLNewContext DbContext { get; }

        #endregion

        #region Properties
        bool InTransaction { get; }
        #endregion

        #region Methods
        void BeginTransaction();
        ActionStatus SaveAndContinue();
        ActionStatus EndTransaction();
        void RollBack();
        #endregion      
    }
}
