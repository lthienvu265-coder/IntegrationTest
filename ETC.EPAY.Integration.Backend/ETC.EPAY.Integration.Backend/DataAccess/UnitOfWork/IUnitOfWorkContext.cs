using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETC.EPAY.Integration.DataAccess.UnitOfWork
{
    public interface IUnitOfWorkContext : IDisposable
    {
        NpgsqlConnection Context { get; init; }
        NpgsqlTransaction Transaction { get; init; }
    }
}
