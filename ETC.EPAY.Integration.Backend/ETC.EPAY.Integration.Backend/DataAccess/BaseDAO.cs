using ETC.EPAY.Integration.DataAccess.UnitOfWork;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.DataAccess
{
    public abstract class BaseDAO
    {
        #region Properties
        protected NpgsqlConnection Context { get; init; }
        protected NpgsqlTransaction Transaction { get; init; }
        #endregion

        protected BaseDAO()
        {

        }

        protected BaseDAO(IUnitOfWorkContext unitOfWorkContext)
        {
            Context = unitOfWorkContext.Context;
            Transaction = unitOfWorkContext.Transaction;
        }
    }
}
